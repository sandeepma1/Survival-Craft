using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(ItemManager))]
    [RequireComponent(typeof(InventorySettingsManager))]
    [RequireComponent(typeof(InventoryTriggererManager))]
    [RequireComponent(typeof(InventoryPlayerManager))]
    [RequireComponent(typeof(InventoryInputManager))]
    [AddComponentMenu("InventorySystem/Managers/InventoryManager")]
    public partial class InventoryManager : MonoBehaviour
    {
        #region Variables

        [Header("Windows")]
        public InventoryUI inventory;
        public SkillbarUI skillbar;
        public BankUI bank;
        public CharacterUI character;
        public LootUI loot;
        public VendorUI vendor;
        public NoticeUI notice;
        public CraftingWindowStandardUI craftingStandard;
        public CraftingWindowLayoutUI craftingLayout;
        public SelectableObjectInfoUI selectableObjectInfo;
        public InventoryContextMenu contextMenu;

        [SerializeField]
        private Camera _uiCamera;
        public Camera uiCamera
        {
            get { return _uiCamera != null ? _uiCamera : Camera.main; }
        }

        public AudioMixerGroup uiAudioMixerGroup;
        
        [Header("Triggerers")]
        public ObjectTriggererFPSUI objectTriggererFPSUI;
        public ObjectTriggererRangeUI objectTriggererRangeUI;

        [Header("Dialogs")]
        public ConfirmationDialog confirmationDialog;
        public ItemBuySellDialog buySellDialog;
        public IntValDialog intValDialog;
        public IntValDialog unstackDialog;


        [Header("Databases")]
        [SerializeField]
        [InventoryRequired]
        [FormerlySerializedAs("_lang")]
        private InventoryLangDatabase _sceneLangDatabase;
        public InventoryLangDatabase sceneLangDatabase
        {
            get { return _sceneLangDatabase; }
        }


        private static InventoryDatabaseLookup<InventoryLangDatabase> _langDatabaseLookup;
        public static InventoryDatabaseLookup<InventoryLangDatabase> langDatabaseLookup
        {
            get
            {
                if (_langDatabaseLookup == null)
                {
                    _langDatabaseLookup = new InventoryDatabaseLookup<InventoryLangDatabase>(instance != null ? instance.sceneLangDatabase : null, CurrentLangDBPathKey);
                }

                return _langDatabaseLookup;
            }
        }

        public static InventoryLangDatabase langDatabase
        {
            get
            {
                return langDatabaseLookup.GetDatabase();
            }
            private set { langDatabaseLookup.SetDatabase(value); }
        }
        
        private static string CurrentDBPrefixName
        {
            get
            {
                var path = Application.dataPath;
                if (path.Length > 0)
                {
                    var pathElems = path.Split('/');
                    return pathElems[pathElems.Length - 2];
                }

                return "";
            }
        }
        public static string CurrentLangDBPathKey
        {
            get { return CurrentDBPrefixName + "InventorySystem_CurrentLangDatabasePath"; }
        }        

        /// <summary>
        /// The parent holds all collection's objects to keep the scene clean.
        /// </summary>
        public Transform collectionObjectsParent { get; private set; }

        /// <summary>
        /// Collections such as the Inventory are used to loot items.
        /// When an item is picked up the item will be moved to the inventory. You can create multiple Inventories and limit types per inventory.
        /// </summary>
        private static List<InventoryCollectionLookup<ItemCollectionBase>> _lootToCollections;
        private static List<InventoryCollectionLookup<CharacterUI>> _equipToCollections;
        private static List<ItemCollectionBase> _bankCollections;
        private static ItemCollectionBaseAddCounter _counter;


        #endregion

        private static InventoryManager _instance;
        public static InventoryManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventoryManager>();
                }

//                if (_instance == null)
//                {
//                    throw new ManagerNotFoundException("InventoryManager");
//                }

                return _instance;
            }
        }

        public void Awake()
        {
            _instance = this;
            _counter = new ItemCollectionBaseAddCounter();

            Assert.IsNotNull(langDatabase, "No language database set, can't display notifications.");

            _lootToCollections = new List<InventoryCollectionLookup<ItemCollectionBase>>(4);
            _equipToCollections = new List<InventoryCollectionLookup<CharacterUI>>(4);
            _bankCollections = new List<ItemCollectionBase>(4);


            collectionObjectsParent = new GameObject("__COLLECTION_OBJECTS").transform;
            collectionObjectsParent.transform.SetParent(transform);
        }

        public void Start()
        {
            Assert.IsNotNull(InventoryTriggererManager.instance, "No InventoryTriggererManager set, check the integrations helper to fix.");
            Assert.IsNotNull(InventoryPlayerManager.instance, "No InventoryPlayerManager set, check the integrations helper to fix.");
        }

        public void Update()
        {
            InventoryAudioManager.Update();
        }

        public void OnLevelWasLoaded(int level)
        {
            // Level loaded, reset the cooldowns
            foreach (var category in ItemManager.database.itemCategories)
            {
                category.lastUsageTime = 0.0f;
            }
        }


        #region Collection stuff

        private static void RebuildCounter()
        {
            _counter.LoadFrom(_lootToCollections.ToArray());
        }

        protected virtual InventoryCollectionLookup<ItemCollectionBase> GetBestLootCollectionForItem(InventoryItemBase item, bool rebuildCounter)
        {
            if (rebuildCounter)
            {
                RebuildCounter();
            }

            var lookup = _counter.GetBestCollectionForItem(item);
            if (lookup == null)
                return null;

            return _lootToCollections.First(o => o.collection == lookup.collectionRef);
        }


        /// <summary>
        /// Get the item count of all items in the lootable collections.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns>Item count in all lootable collections.</returns>
        public static uint GetItemCount(uint itemID, bool checkBank)
        {
            uint count = 0;
            foreach (var collection in _lootToCollections)
                count += collection.collection.GetItemCount(itemID);

            if(checkBank)
            {
                foreach (var collection in _bankCollections)
                    count += collection.GetItemCount(itemID);
            }

            return count;
        }

        /// <summary>
        /// Get the first item from all lootable collections.
        /// </summary>
        /// <param name="itemID">ID of the object your searching for</param>
        /// <returns></returns>
        public static InventoryItemBase Find(uint itemID, bool checkBank)
        {
            foreach (var col in _lootToCollections)
            {
                var item = col.collection.Find(itemID);
                if(item != null)
                    return item;   
            }

            if(checkBank)
            {
                foreach (var col in _bankCollections)
                {
                    var item = col.Find(itemID);
                    if (item != null)
                        return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all items with a given ID
        /// </summary>
        /// <param name="itemID">ID of the object your searching for</param>
        /// <returns></returns>
        public static List<InventoryItemBase> FindAll(uint itemID, bool checkBank)
        {
            var list = new List<InventoryItemBase>(8);
            foreach (var col in _lootToCollections)
            {
                // Linq.Concat doesn't seem to work.. :/
                foreach (var item in col.collection.FindAll(itemID))
                {
                    list.Add(item);
                }
            }
        
            if(checkBank)
            {
                foreach (var col in _bankCollections)
                {
                    // Linq.Concat doesn't seem to work.. :/
                    foreach (var item in col.FindAll(itemID))
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }


        /// <summary>
        /// Add an item to an inventory.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <returns></returns>
        public static bool AddItem(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        {
            Assert.IsTrue(item != null, "Given item is null - Can't add.");
//            Assert.IsTrue(item.currentStackSize <= item.maxStackSize, "Item currentStackSize is exceeding the max stack size. This is not allowed, split the item into multiple smaller stacks.");
            Assert.IsTrue(_lootToCollections.Count > 0, "Can't add item, no collectionso attached to the current player");

            var bestCollection = instance.GetBestLootCollectionForItem(item, true);
            if (bestCollection == null)
            {
                langDatabase.collectionFull.Show(item.name, item.description, instance.inventory.collectionName);
                return false;
            }

            var itemArr = InventoryItemUtility.EnforceMaxStackSize(item);
            foreach (var i in itemArr)
            {
                bool added = bestCollection.collection.AddItem(i);
                Assert.IsTrue(added, "Item wasn't added even though check passed!");

                if (storedItems != null)
                {
                    storedItems.Add(item);
                }
            }

            return true;
        }

        /// <summary>
        /// Add items to an inventory.
        /// </summary>
        /// <param name="items">The items to add</param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <param name="repaint">Should items be repainted? True will be fine in most cases</param>
        /// <returns></returns>
        public static bool AddItems(InventoryItemBase[] items, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        {
            items = items.OrderByDescending(o => o.layoutSize).ToArray(); // Order from large to small to make sure large items can always be placed.
            if (CanAddItems(items, true) == false)
            {
                return false;
            }

            if (storedItems == null)
                storedItems = new List<InventoryItemBase>();

            foreach (var item in items)
            {
                bool added = AddItem(item, storedItems, repaint);
                Assert.IsTrue(added, "Couldn't add items even though check passed! Please report this bug + stack trace");
            }

            return true;
        }

        /// <summary>
        /// Add an item to an inventory and remove it from the collection it was previously in.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <returns></returns>
        public static bool AddItemAndRemove(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        {
            if (_lootToCollections.Count == 0)
                Debug.Log("Can't add item, no collectionso attached to this player");

            var best = instance.GetBestLootCollectionForItem(item, true);
            if (best != null)
            {
                return best.collection.AddItemAndRemove(item, storedItems, repaint);
            }

            langDatabase.collectionFull.Show(item.name, item.description, instance.inventory.collectionName);
            return false;
        }


        /// <summary>
        /// Remove items first, then attempt to add them. The first remove could potentially clear up a slot for the adding.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="itemsToRemoveFirst"></param>
        /// <returns></returns>
        public static bool RemoveItemsThenAdd(InventoryItemBase[] itemsToAdd, params InventoryItemAmountRow[] itemsToRemoveFirst)
        {
            RebuildCounter();
            bool removeSucceeded = _counter.TryRemoveItems(itemsToRemoveFirst);
            if (removeSucceeded == false)
            {
                return false;
            }

            var unAdded = _counter.TryAdd(itemsToAdd);
            if (unAdded.Length > 0)
            {
                return false;
            }

            foreach (var item in itemsToRemoveFirst)
            {
                uint removed = RemoveItem(item.item.ID, item.amount, false);
                Assert.IsTrue(removed == item.amount, "Couldn't remove item even though check passed!");
            }

            bool added = AddItems(itemsToAdd);
            Assert.IsTrue(added);

            return true;
        }

        public static bool CanAddItem(InventoryItemBase item)
        {
            return CanAddItem(new InventoryItemAmountRow(item, item.currentStackSize));
        }

        public static bool CanAddItem(InventoryItemAmountRow row)
        {
            foreach (var lookup in _lootToCollections)
            {
                if (lookup.collection.CanAddItemCount(row.item, row.amount) >= row.amount)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanAddItems(InventoryItemAmountRow[] items, bool rebuildCounter = true)
        {
            if (rebuildCounter)
            {
                RebuildCounter();
            }

            var unAdded = _counter.TryAdd(items);
            return unAdded.Length == 0;
        }

        public static bool CanAddItems(InventoryItemBase[] items, bool rebuildCounter = true)
        {
            return CanAddItems(InventoryItemUtility.ItemsToRows(items), rebuildCounter);
        }

        public static bool CanRemoveItems(IList<InventoryItemBase> items)
        {
            return CanRemoveItems(InventoryItemUtility.ItemsToRows(items));
        }

        public static bool CanRemoveItems(IList<InventoryItemAmountRow> items)
        {
            _counter.LoadFrom(_lootToCollections.ToArray());
            return _counter.TryRemoveItems(items);
        }


        /// <summary>
        /// Remove an item from the inventories / bank when checkBank = true.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="amount"></param>
        /// <param name="checkBank">Also search the bankf or items, bank items take priority over items in the inventories</param>
        public static uint RemoveItem(uint itemID, uint amount, bool checkBank)
        {
            Assert.IsTrue(GetItemCount(itemID, checkBank) >= amount, "Tried to remove more items than available, check with FindAll().Count first.");

            uint amountToRemove = amount;
            if (checkBank)
            {
                foreach (var bank in _bankCollections)
                {
                    if (amountToRemove > 0)
                    {
                        amountToRemove -= bank.RemoveItem(itemID, amountToRemove);
                    }
                    else
                        break;
                }
            }

            foreach (var inventory in _lootToCollections)
            {
                //var items = bank.FindAll(itemID);
                if (amountToRemove > 0)
                {
                    amountToRemove -= inventory.collection.RemoveItem(itemID, amountToRemove);
                }
                else
                    break;
            }

            return amount - amountToRemove;
        }


        /// <summary>
        /// Add a collection that functions as an Inventory. Items will be looted to this collection.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        /// <param name="priority">
        /// How important is the collection, if you 2 collections can hold the item, which one should be chosen?
        /// Range of 0 to 100
        /// </param>
        public static void AddInventoryCollection(ItemCollectionBase collection, int priority)
        {
            Assert.IsNotNull(collection, "Added inventory collection to manager that was NULL!");
            Assert.IsTrue(priority >= 0, "Priority has to be higher than 0");

            _lootToCollections.Add(new InventoryCollectionLookup<ItemCollectionBase>(collection, priority));
        }


        public static void RemoveInventoryCollection(ItemCollectionBase collection)
        {
            _lootToCollections.RemoveAll(o => o.collection = collection);
            //var found = lootToCollections.FirstOrDefault(o => o.collection == collection);
            //if (found != null)
                //lootToCollections.Remove(found);

            //lootToCollections.Remove(new InventoryCollectionLookup(collection, priority));
        }


        /// <summary>
        /// Check if a given collection is a loot to collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsInventoryCollection(ItemCollectionBase collection)
        {
            return _lootToCollections.Any(col => col.collection == collection);
        }

        /// <summary>
        /// Check if a given collection is a equip to collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsEquipToCollection(ItemCollectionBase collection)
        {
            return _equipToCollections.Any(col => col.collection == collection);
        }

        /// <summary>
        /// Add a collection that functions as an Equippable collection. Items can be equipped to this collection.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        /// <param name="priority">
        /// How important is the collection, if you 2 collections can hold the item, which one should be chosen?
        /// Range of 0 to 100
        /// 
        /// Note: This method is not used yet, it only registers the Equippable collection, that's it.
        /// </param>
        public static void AddEquipCollection(CharacterUI collection, int priority)
        {
            _equipToCollections.Add(new InventoryCollectionLookup<CharacterUI>(collection, priority));
        }

        public static void RemoveEquipCollection(CharacterUI collection)
        {
            _equipToCollections.RemoveAll(o => o.collection == collection);
            //var found = equipToCollections.FirstOrDefault(o => o.collection == collection);
            //if (found != null)
                //equipToCollections.Remove(found);
        }


        public static void AddBankCollection(ItemCollectionBase collection)
        {
            _bankCollections.Add(collection);
        }

        public static void RemoveBankCollection(ItemCollectionBase collection)
        {
            _bankCollections.Remove(collection);
        }

        /// <summary>
        /// Get all bank collections
        /// I casted it to an array (instead of list) to avoid messing with the internal list.
        /// </summary>
        /// <returns></returns>
        public static ItemCollectionBase[] GetBankCollections()
        {
            return _bankCollections.ToArray();
        }

        public static ItemCollectionBase[] GetLootToCollections()
        {
            return _lootToCollections.Select(o => o.collection).ToArray();
        }

        public static CharacterUI[] GetEquipToCollections()
        {
            return _equipToCollections.Select(o => o.collection).ToArray();
        }

        #endregion

        #region Currencies 

        protected static float CanRemoveCurrencyCountInventories(uint currencyID, bool allowConversions)
        {
            float totalAmount = 0.0f;
            foreach (var col in _lootToCollections)
            {
                if (col.collection.canContainCurrencies == false)
                    continue;

                totalAmount += col.collection.CanRemoveCurrencyCount(currencyID, allowConversions);
            }

            return totalAmount;
        }
        protected static float CanRemoveCurrencyCountBanks(uint currencyID, bool allowConversions)
        {
            float totalAmount = 0.0f;
            foreach (var bankCollection in _bankCollections)
            {
                if (bankCollection.canContainCurrencies == false)
                    continue;

                totalAmount += bankCollection.CanRemoveCurrencyCount(currencyID, allowConversions);
            }

            return totalAmount;
        }

        public static float GetCurrencyCount(uint currencyID, bool checkBank)
        {
            float totalAmount = 0.0f;
            foreach (var col in _lootToCollections)
            {
                var c = col.collection.GetCurrencyByID(currencyID);
                if (c != null)
                {
                    totalAmount += c.amount;
                }
            }

            if (checkBank)
            {
                foreach (var bankCollection in _bankCollections)
                {
                    var c = bankCollection.GetCurrencyByID(currencyID);
                    if (c != null)
                    {
                        totalAmount += c.amount;
                    }
                }
            }

            return totalAmount;
        }


        /// <summary>
        /// Can we remove the amount of given currency?
        /// </summary>
        /// <param name="currencyLookup"></param>
        /// <param name="allowCurrencyConversion">Allow converting a higher currency down to this currency? For example convert gold to silver.</param>
        /// <param name="checkBank"></param>
        /// <returns></returns>
        public static bool CanRemoveCurrency(InventoryCurrencyLookup currencyLookup, bool allowCurrencyConversion, bool checkBank)
        {
            return CanRemoveCurrency(currencyLookup.amount, currencyLookup.currency.ID, allowCurrencyConversion, checkBank);
        }

        /// <summary>
        /// Can we remove the amount of given currency?
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currencyID"></param>
        /// <param name="allowCurrencyConversion">Allow converting a higher currency down to this currency? For example convert gold to silver.</param>
        /// <returns></returns>
        public static bool CanRemoveCurrency(float amount, uint currencyID, bool allowCurrencyConversion)
        {
            return CanRemoveCurrency(amount, currencyID, allowCurrencyConversion, false);
        }

        /// <summary>
        /// Can we remove the amount of given currency?
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currencyID"></param>
        /// <param name="allowCurrencyConversion">Allow converting a higher currency down to this currency? For example convert gold to silver.</param>
        /// <param name="checkBank"></param>
        /// <returns></returns>
        public static bool CanRemoveCurrency(float amount, uint currencyID, bool allowCurrencyConversion, bool checkBank)
        {
            float totalAmount = CanRemoveCurrencyCountInventories(currencyID, allowCurrencyConversion);
            if (checkBank)
            {
                totalAmount += CanRemoveCurrencyCountBanks(currencyID, allowCurrencyConversion);
            }

            return totalAmount >= amount;
        }

        public static bool CanAddCurrency(InventoryCurrencyLookup currencyLookup)
        {
            return CanAddCurrency(currencyLookup.amount, currencyLookup.currency.ID);
        }

        public static bool CanAddCurrency(float amount, uint currencyID)
        {
            float totalAmount = amount;
            foreach (var col in _lootToCollections)
            {
                if (col.collection.canContainCurrencies == false)
                    continue;

                totalAmount += col.collection.CanAddCurrencyCount(currencyID);
            }

            return totalAmount >= amount;
        }

        /// <summary>
        /// Add currency to the loot to collections.
        /// Note: Currency is auto. converted if it's exceeding the conversion restrictions.
        /// </summary>
        /// <param name="lookup"></param>
        /// <param name="amountMultipier"></param>
        /// <returns></returns>
        public static bool AddCurrency(InventoryCurrencyLookup lookup, float amountMultipier = 1.0f)
        {
            return AddCurrency(lookup.amount * amountMultipier, lookup.currency.ID);
        }

        /// <summary>
        /// Add currency to the loot to collections.
        /// Note: Currency is auto. converted if it's exceeding the conversion restrictions.
        /// </summary>
        /// <param name="amount">The amount to add</param>
        /// <param name="currencyID">The currencyID (type) of currency to add.</param>
        /// <returns></returns>
        public static bool AddCurrency(float amount, uint currencyID)
        {
            if (CanAddCurrency(amount, currencyID) == false)
                return false;

            float toAdd = amount;
            foreach (var col in _lootToCollections)
            {
                if (col.collection.canContainCurrencies == false)
                    continue;
                
                float canAdd = col.collection.CanAddCurrencyCount(currencyID);
                if (canAdd >= toAdd)
                {
                    // All currency can be stored in a single collection.
                    col.collection.AddCurrency(toAdd, currencyID);
                    return true;
                }

                // We've got to spit it, and share the currency over multiple collections.
                toAdd -= canAdd;

                // Will eventually reach the canAdd >= toAdd and add the remainer to a collection.
                col.collection.AddCurrency(canAdd, currencyID);
            }

            Assert.IsTrue(false, "Couldn't add currency even though check passed! Please report this error + stack trace");
            return false;
        }

        public static bool RemoveCurrency(InventoryCurrencyLookup lookup, float amountMultipier = 1.0f)
        {
            return RemoveCurrency(lookup.amount * amountMultipier, lookup.currency.ID);
        }

        public static bool RemoveCurrency(float amount, uint currencyID)
        {
            if (CanRemoveCurrency(amount, currencyID, true) == false)
                return false;

            float toRemove = amount;
            foreach (var col in _lootToCollections)
            {
                float canRemove = col.collection.CanRemoveCurrencyCount(currencyID);
                if (canRemove >= toRemove)
                {
                    // All currency can be stored in a single collection.
                    col.collection.RemoveCurrency(toRemove, currencyID, true);
                    return true;
                }

                // We've got to spit it, and share the currency over multiple collections.
                toRemove -= canRemove;

                // Will eventually reach the canAdd >= toAdd and add the remainer to a collection.
                col.collection.RemoveCurrency(canRemove, currencyID, true);
            }

            Assert.IsTrue(false, "Couldn't remove currency even though check passed! Please report this error + stack trace");
            return false;
        }


        #endregion
        
    }
}