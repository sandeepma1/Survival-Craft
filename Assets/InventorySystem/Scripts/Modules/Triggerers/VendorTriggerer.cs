using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    using System.Linq;

    using UnityEngine.Serialization;

    /// <summary>
    /// Represents a vendor that sells / buys something
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ObjectTriggerer))]
    [RequireComponent(typeof(SelectableObjectInfo))]
    [AddComponentMenu("InventorySystem/Triggers/Vendor triggererHandler")]
    public partial class VendorTriggerer : MonoBehaviour, IObjectTriggerUser, IInventoryItemContainer
    {
        [Header("Vendor")]
        //public string vendorName;
        public bool canSellToVendor;


        [SerializeField]
        private string _uniqueName;
        public string uniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }


        [Header("Items")]
        [FormerlySerializedAs("forSale")]
        [SerializeField]
        private InventoryItemBase[] _items;
        public InventoryItemBase[] items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }



        /// <summary>
        /// All prices are multiplied with this value. If you want to make items 10% more expensive in a certain are, or on a certain vendor, use this.
        /// </summary>
        [Range(0.0f, 10.0f)]
        [Header("Buying / Selling")]
        public float buyPriceFactor = 1.0f;

        /// <summary>
        /// All sell prices are multiplied with this value. If you want to make items 10% more expensive in a certain are, or on a certain vendor, use this.
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float sellPriceFactor = 1.0f;

        [Range(1, 999)]
        public uint maxBuyItemCount = 999;
        public bool removeItemAfterPurchase = false;

        /// <summary>
        /// Can items be bought back after they're sold?
        /// </summary>
        [Header("Buy back")]
        public bool enableBuyBack = true;

        /// <summary>
        /// When true the items will be stored in the buy back collection, when false the items will be added to the regular collection of the vendor.
        /// </summary>
        [Tooltip("When true the items will be stored in the buy back collection, when false the items will be added to the regular collection of the vendor.")]
        public bool addItemsSoldToVendorToBuyBack = true;

        /// <summary>
        /// How expensive is the item to buy back. item.sellPrice * buyBackCostFactor = the final price to buy back an item.
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float buyBackPriceFactor = 1.0f;

        /// <summary>
        /// Max number of items in buy back window.
        /// </summary>
        public uint maxBuyBackItemSlotsCount = 10;

        /// <summary>
        /// Is buyback shared between all vendors with the same category?
        /// </summary>
        public bool buyBackIsShared = false;

        /// <summary>
        /// The category this vendor belongs to, used for sharing the buyback.
        /// Shared buyback is shared based on the vendor categeory, all vendors with the same category will have the same buy back items.
        /// </summary>
        [Tooltip("Shared buyback is shared based on the vendor categeory, all vendors with the same category will have the same buy back items.")]
        public string vendorCategory = "Default";

        /// <summary>
        /// Generator used to generate a random set of items for this vendor
        /// </summary>
        public IItemGenerator itemGenerator { get; set; }

        protected VendorUI vendorUI;
        protected Animator animator;


        public VendorBuyBackDataStructure<InventoryItemBase> buyBackDataStructure { get; protected set; }

        [NonSerialized]
        protected Transform buyBackParent;
        
        [NonSerialized]
        protected ObjectTriggerer triggerer;


        protected virtual void Start()
        {
            vendorUI = InventoryManager.instance.vendor;
            if (vendorUI == null)
            {
                Debug.LogWarning("No vendor UI found, yet there's a vendor in the scene.", transform);
                return;
            }
            for (int i = 0; i < items.Length; i++)
            {
                var t = Instantiate<InventoryItemBase>(items[i]);
                t.currentStackSize = items[i].currentStackSize;
                t.maxStackSize = 999;
                t.transform.SetParent(transform);
                t.gameObject.SetActive(false);
                items[i] = t;
            }

            animator = GetComponent<Animator>();
            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.window = vendorUI.window;
            triggerer.handleWindowDirectly = false; // We're in charge now :)

            buyBackDataStructure = new VendorBuyBackDataStructure<InventoryItemBase>((int)maxBuyBackItemSlotsCount, buyBackIsShared, vendorCategory, (int)maxBuyBackItemSlotsCount);

            buyBackParent = new GameObject("Vendor_BuyBackContainer").transform;
            buyBackParent.SetParent(InventoryManager.instance.collectionObjectsParent);

            triggerer.OnTriggerUse += Use;
            triggerer.OnTriggerUnUse += UnUse;
        }


        protected virtual void Use(InventoryPlayer player)
        {
            if (InventoryUIUtility.CanReceiveInput(gameObject) == false)
            {
                return;
            }


            // Set items
            vendorUI.SetItems(items, false, false);
            vendorUI.currentVendor = this;

            vendorUI.window.Show();
        }

        protected virtual void UnUse(InventoryPlayer player)
        {
            if (InventoryUIUtility.CanReceiveInput(gameObject) == false)
            {
                return;
            }

            var before = vendorUI.currentVendor;
            vendorUI.currentVendor = null;

            // If a differnet window was opened, don't hide it.
            if (before == this)
            {
                vendorUI.window.Hide();
            }
        }


        /// <summary>
        /// Sell an item to this vendor.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public virtual void SellItemToVendor(InventoryItemBase item)
        {
            uint max = InventoryManager.GetItemCount(item.ID, false);

            if (CanSellItemToVendor(item, max) == false)
            {
                InventoryManager.langDatabase.vendorCannotSellItem.Show(item.name, item.description, max);
                return;
            }

            InventoryManager.instance.buySellDialog.ShowDialog(InventoryManager.instance.vendor.window.transform, "Sell " + name, "Are you sure you want to sell " + name, 1, (int)max, item, ItemBuySellDialogAction.Selling, this, (amount) =>
            {
                // Sell items
                SellItemToVendorNow(item, (uint)amount);

            }, (amount) =>
            {
                // Canceled

            });
        }

        /// <summary>
        /// Sell item now to this vendor. The vendor doesn't sell the object, the user sells to this vendor.
        /// Note that this does not show any UI or warnings and immediately handles the action.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual bool SellItemToVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanSellItemToVendor(item, amount) == false)
                return false;

            if (enableBuyBack)
            {
                var copy = GameObject.Instantiate<InventoryItemBase>(item);
                copy.currentStackSize = amount;
                copy.maxStackSize = 999; // Stacking
                copy.transform.SetParent(buyBackParent.transform);

                if (addItemsSoldToVendorToBuyBack)
                {
                    buyBackDataStructure.Add(copy);
                }
                else
                {
                    vendorUI.AddItem(copy); // Add to regular collection.
                }
            }

            InventoryManager.AddCurrency(GetSellPrice(item, amount), item.sellPrice.currency.ID);
            InventoryManager.RemoveItem(item.ID, amount, false);

            vendorUI.NotifyItemSoldToVendor(item, amount);
            return true;
        }

        public virtual bool CanSellItemToVendor(InventoryItemBase item, uint amount)
        {
            if (canSellToVendor == false)
                return false;

            if (item.isSellable == false)
                return false;

            if (addItemsSoldToVendorToBuyBack == false)
            {
                if (vendorUI.CanAddItem(item) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool CanBuyItemBackFromVendor(InventoryItemBase item, uint amount)
        {
            float totalCost = GetBuyBackPrice(item, amount);

            if (buyBackDataStructure.ItemCount(item.ID) < amount)
                return false; // Something wen't wrong, we don't have that many items for buy-back.

            if (InventoryManager.CanRemoveCurrency(totalCost, item.sellPrice.currency.ID, true) == false)
            {
                string totalCostString = item.sellPrice.GetFormattedString(amount * buyBackPriceFactor);
                var c = InventoryManager.instance.inventory.GetCurrencyByID(item.sellPrice.currency.ID);

                InventoryManager.langDatabase.userNotEnoughGold.Show(item.name, item.description, amount, totalCostString, c != null ? c.GetFormattedString() : "");
                return false; // Not enough gold for this many
            }
            
            if (CanAddItemsToInventory(item, amount) == false)
            {
                InventoryManager.langDatabase.collectionFull.Show(item.name, item.description, "Inventory");
                return false;
            }

            return true;
        }

        public virtual bool CanBuyItemFromVendor(InventoryItemBase item, uint amount)
        {
            float totalCost = GetBuyPrice(item, amount);

            if (InventoryManager.CanRemoveCurrency(totalCost, item.buyPrice.currency.ID, true) == false)
            {
                string totalCostString = item.buyPrice.GetFormattedString(amount * buyPriceFactor);
                var c = InventoryManager.instance.inventory.GetCurrencyByID(item.buyPrice.currency.ID);

                InventoryManager.langDatabase.userNotEnoughGold.Show(item.name, item.description, amount, totalCostString, c != null ? c.GetFormattedString() : "");
                return false; // Not enough gold for this many
            }

            if (CanAddItemsToInventory(item, amount) == false)
            {
                InventoryManager.langDatabase.collectionFull.Show(item.name, item.description, "Inventory");
                return false;
            }

            return true;
        }

        public virtual void BuyItemFromVendor(InventoryItemBase item, bool isBuyBack)
        {
            ItemBuySellDialogAction action = ItemBuySellDialogAction.Buying;
            uint maxAmount = removeItemAfterPurchase ? vendorUI.GetItemCount(item.ID) : maxBuyItemCount;
            if (isBuyBack)
            {
                action = ItemBuySellDialogAction.BuyingBack;
                maxAmount = item.currentStackSize;
            }

            InventoryManager.instance.buySellDialog.ShowDialog(InventoryManager.instance.vendor.window.transform, "Buy item " + item.name, "How many items do you want to buy?", 1, (int)maxAmount, item, action, this, (amount) =>
            {
                // Clicked yes!
                if(isBuyBack)
                    BuyItemBackFromVendorNow(item, (uint)amount);
                else
                    BuyItemFromVendorNow(item, (uint)amount);

            }, (amount) =>
            {
                // Clicked cancel...

            });
        }


        public virtual bool BuyItemBackFromVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanBuyItemBackFromVendor(item, amount) == false)
                return false;

            buyBackDataStructure.Remove(item, amount);


            var c1 = Instantiate<InventoryItemBase>(item);
            c1.currentStackSize = amount;
            c1.maxStackSize = ItemManager.database.items[c1.ID].maxStackSize; // Reset stack size from database.

            InventoryManager.RemoveCurrency(GetBuyBackPrice(item, amount), item.sellPrice.currency.ID);
            InventoryManager.AddItem(c1); // Will handle unstacking if the stack size goes out of bounds.

            vendorUI.NotifyItemBoughtBackFromVendor(item, amount);
            return true;
        }


        /// <summary>
        /// Buy an item from this vendor, this does not display a dialog, but moves the item directly to the inventory.
        /// Note that this does not show any UI or warnings and immediately handles the action.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public virtual bool BuyItemFromVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanBuyItemFromVendor(item, amount) == false)
                return false;


            var c1 = Instantiate<InventoryItemBase>(item);
            c1.currentStackSize = amount;
            c1.maxStackSize = ItemManager.database.items[c1.ID].maxStackSize; // Reset stack size from database.

            InventoryManager.RemoveCurrency(GetBuyPrice(item, amount), item.buyPrice.currency.ID);
            InventoryManager.AddItem(c1); // Will handle unstacking if the stack size goes out of bounds.
            
            if (removeItemAfterPurchase)
            {
                vendorUI.RemoveItem(item.ID, amount);
            }

            vendorUI.NotifyItemBoughtFromVendor(item, amount);
            return true;
        }


        /// <summary>
        /// Can this item * amount be added to the inventory, is there room?
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>True if items can be placed, false is not.</returns>
        protected virtual bool CanAddItemsToInventory(InventoryItemBase item, uint amount)
        {
            uint originalStackSize = item.currentStackSize;

            item.currentStackSize = amount;
            bool can = InventoryManager.CanAddItem(item);
            item.currentStackSize = originalStackSize; // Reset

            return can;
        }

        public virtual float GetBuyBackPrice(InventoryItemBase item, uint amount)
        {
            return item.sellPrice.amount * amount * buyBackPriceFactor;
        }
        public virtual float GetBuyPrice(InventoryItemBase item, uint amount)
        {
            return item.buyPrice.amount * amount * buyPriceFactor;
        }
        public virtual float GetSellPrice(InventoryItemBase item, uint amount)
        {
            return item.sellPrice.amount * amount * sellPriceFactor;
        }
    }
}