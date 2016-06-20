using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/character/")]
    [AddComponentMenu("InventorySystem/Windows/Character")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CharacterUI : ItemCollectionBase, ICollectionPriority, IInventoryDragAccepter
    {
        /// <summary>
        /// The container where the generated stats will be placed.
        /// </summary>
        public RectTransform statsContainer;


        [Range(0, 100)]
        [SerializeField]
        private int _collectionPriority;
        public int collectionPriority
        {
            get { return _collectionPriority; }
            set { _collectionPriority = value; }
        }

        public bool isSharedCollection = false;


        [Header("UI Prefabs")]
        public InventoryEquipStatRowUI statusRowPrefab;
        public InventoryEquipStatCategoryUI statusCategoryPrefab;
        

        public InventoryEquippableField[] equipSlotFields { get; private set; }

        private InventoryStatsContainer _stats = new InventoryStatsContainer();
        public InventoryStatsContainer stats
        {
            get { return _stats; }
            protected set { _stats = value; }
        }


        [NonSerialized]
        protected InventoryPool<InventoryEquipStatRowUI> rowsPool;

        [NonSerialized]
        protected InventoryPool<InventoryEquipStatCategoryUI> categoryPool;


        /// <summary>
        /// The player that this CharacterUI belongs to. (can be null)
        /// </summary>
        public InventoryPlayer player { get; set; }
        

        private UIWindow _window;
        public UIWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindow>();

                return _window;
            }
            protected set { _window = value; }
        }

        public override uint initialCollectionSize
        {
            get
            {
                return (uint)equipSlotFields.Length;
            }
        }

        [Obsolete("Use stats.OnStatChanged instead")]
        public event InventoryStatsContainer.StatChanged OnStatChanged
        {
            add { stats.OnStatChanged += value; }
            remove { stats.OnStatChanged -= value; }
        }

        public override void Awake()
        {
            base.Awake();
            equipSlotFields = new InventoryEquippableField[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                equipSlotFields[i] = items[i].gameObject.GetComponent<InventoryEquippableField>();
                Assert.IsNotNull(equipSlotFields[i], "CharacterUI manually defined collection contains gameObject without InventoryEquippableField component.");

                if (equipSlotFields[i].equipTypes.Any(o => o == null))
                {
                    Debug.LogError("CharacterUI's equipTypes contains null (empty) field.", equipSlotFields[i].gameObject);
                }
            }

            if (equipSlotFields.Any(o => o == null))
            {
                Debug.LogError("This characterUI has an empty reference in the equip slot fields (EquippableFields has empty row)", gameObject);
            }

            SetDefaultDataProviders();
            PrepareCharacterStats();

            if (isSharedCollection)
                InventoryManager.AddEquipCollection(this, collectionPriority);

            if (statusRowPrefab != null)
                rowsPool = new InventoryPool<InventoryEquipStatRowUI>(statusRowPrefab, 32);

            if (statusCategoryPrefab != null)
                categoryPool = new InventoryPool<InventoryEquipStatCategoryUI>(statusCategoryPrefab, 8);


            window.OnShow += RepaintAllStats;
            stats.OnStatChanged += RepaintStat;
        }
        
        public override void Start()
        {
            base.Start();

            RepaintAllStats();
        }


        protected virtual void SetDefaultDataProviders()
        {
            stats.dataProviders.Add(new InventoryStatsPropertiesDataProvider());
            stats.dataProviders.Add(new InventoryStatsDataProvider());
        }

        public bool AcceptsDragItem(InventoryItemBase item)
        {
            var equippable = item as EquippableInventoryItem;
            if (equippable == null)
            {
                return false;
            }

            return equippable.CanEquip(this);
        }

        /// <summary>
        /// Called by the InventoryDragAccepter, when an item is dropped on the window / a specific location, this method is called to add a custom behavior.
        /// </summary>
        /// <param name="item"></param>
        public bool AcceptDragItem(InventoryItemBase item)
        {
            if (AcceptsDragItem(item) == false)
                return false;
            
            var equippable = (EquippableInventoryItem) item;
            var bestSlot = equippable.GetBestEquipSlot(this);
            return EquipItem(bestSlot, equippable);
        }


        /// <summary>
        /// Get all slots where this item can be equipped.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns indices where the item can be equipped. collection[index] ... </returns>
        public InventoryEquippableField[] GetEquippableSlots(EquippableInventoryItem item)
        {
            if (item.equipType == null)
            {
                Debug.LogWarning("The item " + item.name + " you're trying to equip has no equipment type set. Item cannot be equipped and will be ingored.", item.gameObject);
                return new InventoryEquippableField[0];
            }

            if (equipSlotFields.Length == 0)
            {
                Debug.LogWarning("This characterUI has no equipSlotFields, need to define some??", gameObject);
                return new InventoryEquippableField[0];
            }


            var equipSlots = new List<InventoryEquippableField>(4);
            foreach (var field in equipSlotFields)
            {
                foreach (var type in field.equipTypes)
                {
                    if (item.equipType.ID == type.ID)
                    {
                        bool canAdd = true;
                        // Can the item be equipped considering the usage requirement properties?
                        foreach (var prop in item.usageRequirementProperties)
                        {
                            canAdd = prop.CanUse(player);
                            if (canAdd == false)
                            {
                                break;
                            }
                        }

                        if (canAdd)
                        {
                            equipSlots.Add(field);
                        }
                    }
                }
            }

            return equipSlots.ToArray();
        }


        /// <summary>
        /// Convenience method to grab a stat from this character.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete("Use stats.Get() instead")]
        public IInventoryCharacterStat GetStat(string category, string name)
        {
            return stats.Get(category, name);
        }

        [Obsolete("Use stats.Add() instead")]
        public void AddStat(string category,  IInventoryCharacterStat stat)
        {
            stats.Add(category, stat);
        }

        /// <summary>
        /// Gets the categories and stat rows, but doesn't fill them yet.
        /// </summary>
        protected virtual void PrepareCharacterStats()
        {
            stats.Prepare();
        }

        public virtual void RepaintAllStats()
        {
            if (window.isVisible == false || statusRowPrefab == null)
                return;

            // Get rid of the old
            if(statusCategoryPrefab != null)
                categoryPool.DestroyAll();

            rowsPool.DestroyAll();

            // Maybe make a pool for the items? See some spikes...
            foreach (var stat in stats)
            {
                // Maybe make a pool for the items? See some spikes...
                if (stat.Value.Count(o => o.showInUI) == 0)
                    continue; // No items to display in this category.

                // stat.Key is category
                // stat.Value is all items in category
                InventoryEquipStatCategoryUI cat = null;
                if (statusCategoryPrefab != null)
                {
                    cat = categoryPool.Get();
                    cat.SetCategory(stat.Key);
                    cat.transform.SetParent(statsContainer);
                    InventoryUtility.ResetTransform(cat.transform);
                }

                foreach (var s in stat.Value)
                {
                    if (s.showInUI == false)
                        continue;

                    var obj = rowsPool.Get();
                    obj.Repaint(s.statName, s.ToString(), s.color, s.icon);
                    obj.transform.SetParent(cat != null ? cat.container : statsContainer);

                    InventoryUtility.ResetTransform(obj.transform);
                }
            }
        }

        /// <summary>
        /// Repaint a single stat.
        /// </summary>
        /// <param name="stat"></param>
        public virtual void RepaintStat(IInventoryCharacterStat stat)
        {
            if (window.isVisible == false || statusRowPrefab == null || statusCategoryPrefab == null)
                return;

            foreach (var row in rowsPool)
            {
                if (row.statName.text == stat.statName)
                {
                    row.Repaint(stat.statName, stat.ToString(), stat.color, stat.icon);
                }
            }
        }

        public override bool CanSetItem(uint slot, InventoryItemBase item)
        {
            bool set = base.CanSetItem(slot, item);
            if (set == false)
                return false;

            if (item == null)
                return true;

            var equippable = item as EquippableInventoryItem;
            if (equippable == null)
            {
                return false; // Can't equip this item type, only Equippable and anything that inherits from Equippable.
            }

            if (equippable.VerifyCustomUseConditionals() == false)
            {
                return false;
            }

            if (equippable.CanEquip(this) == false)
            {
                return false;
            }

            var slots = GetEquippableSlots(equippable);
            if (slots.Length == 0)
            {
                return false;
            }

            return slots.Any(o => o.index == slot);
        }

        /// <summary>
        /// Some item's require multiple slots, for example a 2 handed item forces the left handed item to be empty.
        /// </summary>
        /// <returns>true if items were removed, false if items were not removed.</returns>
        protected virtual bool HandleLocks(InventoryEquippableField equipSlot, EquippableInventoryItem equippable)
        {
            var toBeRemoved = new List<uint>(8);

            // Loop through things we want to block
            foreach (var blockType in equippable.equipType.blockTypes)
            {
                // Check every slot against this block type
                foreach (var field in equipSlotFields)
                {
                    var item = items[field.index].item;
                    if (item != null)
                    {
                        var eq = (EquippableInventoryItem)item;

                        if (eq.equipType.ID == blockType && field.index != equipSlot.index)
                        {
                            toBeRemoved.Add(field.index);
                            bool canAdd = InventoryManager.CanAddItem(eq);
                            if (canAdd == false)
                                return false;
                        }
                    }
                }
            }

            foreach (uint i in toBeRemoved)
            {
                bool added = InventoryManager.AddItemAndRemove(items[i].item);
                Assert.IsTrue(added, "Item could not be saved, even after check, please report this bug + stacktrace.");
            }

            return true;
        }

        public override bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            return false;
        }
        public override bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            return SwapSlots(slot1, handler2, slot2, repaint);
        }

        public override bool SetItem(uint slot, InventoryItemBase item)
        {
            var equippable = item as EquippableInventoryItem;
            if (equippable != null)
            {
                bool handled = HandleLocks(equipSlotFields[slot], equippable);
                if (handled == false)
                {
                    return false;
                }
            }

            return base.SetItem(slot, item);
        }


        public override void NotifyItemAdded(IEnumerable<InventoryItemBase> items, uint amount, bool cameFromCollection)
        {
            base.NotifyItemAdded(items, amount, cameFromCollection);

            foreach (var item in items)
            {
                Assert.IsTrue(item is EquippableInventoryItem, "Non equippable item was added to character collection. This is not allowed.");
                ((EquippableInventoryItem)item).EquippedItem(equipSlotFields[item.index], amount);
            }
        }

        public override void NotifyItemRemoved(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            base.NotifyItemRemoved(item, itemID, slot, amount);
            ((EquippableInventoryItem)item).UnEquippedItem(amount);

            // Item was removed, stats changed, can other items still remain equipped?
            foreach (var wrapper in items)
            {
                if (wrapper.item != null)
                {
                    var equip = (EquippableInventoryItem)wrapper.item;
                    // Un-use the item, then check if the stats are valid, if not unequip
                    InventoryItemUtility.SetItemProperties(player, item.properties, InventoryItemUtility.SetItemPropertiesAction.UnUse, false);
                    bool canEquip = equip.CanEquip(this);
                    // Restore stats
                    InventoryItemUtility.SetItemProperties(player, item.properties, InventoryItemUtility.SetItemPropertiesAction.Use, false);

                    if (canEquip == false)
                    {
                        UnEquipItem(equip, true);
                    }
                }
            }
        }

        public bool EquipItem(InventoryEquippableField equipSlot, EquippableInventoryItem item)
        {
            Assert.IsNotNull(item);          

            bool handled = HandleLocks(equipSlot, item);
            if (handled == false)
            {
                return false;
            }


            // The values before the collection / slot changed.
            uint fromIndex = item.index;
            var fromCollection = item.itemCollection;


            // There was already an item in this slot, un-equip that one first
            if (items[equipSlot.index].item != null)
            {
                bool unEquipped = UnEquipItem((EquippableInventoryItem) items[equipSlot.index].item, true);
                if (unEquipped == false)
                {
                    return false;
                }
            }

            // EquippedItem the item -> Will swap as merge is not possible
            bool canSet = CanSetItem(equipSlot.index, item);
            if (canSet)
            {
                bool set = SetItem(equipSlot.index, item);
                if (set)
                {
                    fromCollection.SetItem(fromIndex, null);
                    fromCollection[fromIndex].Repaint();
                }

                fromCollection.NotifyItemRemoved(item, item.ID, fromIndex, item.currentStackSize);
                NotifyItemAdded(item, item.currentStackSize, true);
                items[equipSlot.index].Repaint();

                return true;
            }
            
            return true;
        }

        public bool UnEquipItem(EquippableInventoryItem item, bool addToInventory)
        {
            Assert.IsTrue(item.itemCollection == this, "Trying to un-equip an item that wasn't equipped in this collection!");
            Assert.IsNotNull(item);

            if (addToInventory)
            {
                bool canAdd = InventoryManager.CanAddItem(item);
                if (canAdd == false)
                {
                    return false;
                }

                bool added = InventoryManager.AddItemAndRemove(item);
                Assert.IsTrue(added);
                return added;
            }

            SetItem(item.index, null);
            NotifyItemRemoved(item, item.ID, item.index, item.currentStackSize);

            return true;
        }
    }
}