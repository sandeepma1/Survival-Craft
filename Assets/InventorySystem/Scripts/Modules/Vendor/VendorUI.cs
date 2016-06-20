using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    using Devdog.InventorySystem.Models;

    [HelpURL("http://devdog.nl/documentation/vendors/")]
    [AddComponentMenu("InventorySystem/Windows/Vendor UI")]
    [RequireComponent(typeof(UIWindow))]
    public partial class VendorUI : ItemCollectionBase, IInventoryDragAccepter
    {

        #region Events

        public delegate void VendorItemAction(InventoryItemBase item, uint amount, VendorTriggerer vendor);
        

        /// <summary>
        /// Fired when an item is sold.
        /// </summary>
        public event VendorItemAction OnSoldItemToVendor;

        /// <summary>
        /// Fired when an item is bought, also fired when an item is bought back.
        /// </summary>
        public event VendorItemAction OnBoughtItemFromVendor;

        /// <summary>
        /// Fired when an item is bought back from a vendor.
        /// </summary>
        public event VendorItemAction OnBoughtItemBackFromVendor;

        #endregion


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


        protected VendorTriggerer _currentVendor;
        public VendorTriggerer currentVendor
        {
            get
            {
                return _currentVendor;
            }
            set
            {
                if (_currentVendor != null)
                {
                    _currentVendor.items = items.Select(o => o.item).Where(o => o != null).ToArray(); // Set items from this collection to the vendor to store them.
                }

                _currentVendor = value;
                if (_currentVendor != null)
                {
                    if (window.isVisible == false)
                        return;

                    RepaintWindow();
                }
            }
        }


        public VendorUIBuyBack buyBackCollection;
        public UnityEngine.UI.Text vendorNameText;


        /// <summary>
        /// Prices can be modified per vendor, 0 will generate an int 1 will generate a value of 1.1 and so forth.
        /// </summary>
        //public int roundPriceToDecimals = 0;

        public InventoryAudioClip audioWhenSoldItemToVendor;
        public InventoryAudioClip audioWhenBoughtItemFromVendor;


        [SerializeField]
        protected uint _initialCollectionSize = 20;
        public override uint initialCollectionSize
        {
            get
            {
                return _initialCollectionSize;
            }
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            InventoryManager.instance.inventory.OnCurrencyChanged += (float before, InventoryCurrencyLookup lookup) =>
            {
                if (window.isVisible == false)
                    return;

                if (window.isVisible)
                    RepaintWindow();
            };

            window.OnShow += RepaintWindow;
        }

        #region Notifies 

        public virtual void NotifyItemSoldToVendor(InventoryItemBase item, uint amount)
        {
            InventoryManager.langDatabase.vendorSoldItemToVendor.Show(item.name, item.description, amount, currentVendor.name, item.sellPrice.GetFormattedString(amount));

            InventoryAudioManager.AudioPlayOneShot(audioWhenSoldItemToVendor);

            if (OnSoldItemToVendor != null)
                OnSoldItemToVendor(item, amount, currentVendor);
        }

        public virtual void NotifyItemBoughtFromVendor(InventoryItemBase item, uint amount)
        {
            InventoryManager.langDatabase.vendorBoughtItemFromVendor.Show(item.name, item.description, amount, currentVendor.name, item.buyPrice.GetFormattedString(amount));

            InventoryAudioManager.AudioPlayOneShot(audioWhenBoughtItemFromVendor);

            if (OnBoughtItemFromVendor != null)
                OnBoughtItemFromVendor(item, amount, currentVendor);
        }

        public virtual void NotifyItemBoughtBackFromVendor(InventoryItemBase item, uint amount)
        {
            if (OnBoughtItemBackFromVendor != null)
                OnBoughtItemBackFromVendor(item, amount, currentVendor);
        }

        #endregion


        protected virtual void RepaintWindow()
        {
            foreach (var item in items)
                item.Repaint();

            if (vendorNameText != null)
                vendorNameText.text = _currentVendor.name;
        }

        // <inheritdoc />
        public bool AcceptsDragItem(InventoryItemBase item)
        {
            if (currentVendor == null)
                return false;

            return item.isSellable && currentVendor.canSellToVendor;
        }

        /// <summary>
        /// Called by the InventoryDragAccepter, when an item is dropped on the window / a specific location, this method is called to add a custom behavior.
        /// </summary>
        /// <param name="item"></param>
        public bool AcceptDragItem(InventoryItemBase item)
        {
            if (currentVendor == null || AcceptsDragItem(item) == false)
                return false;

            currentVendor.SellItemToVendor(item);
            return true;
        }


        public override bool AddItem(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true, bool fireEvents = true)
        {
            if (base.CanAddItem(item) == false)
            {
                AddSlots(1); // Make room for the new item
            }
            
            return base.AddItem(item, storedItems, repaint, fireEvents);
        }

        public override bool CanAddItem(InventoryItemBase item)
        {
            return true; // TODO: Add limit to vendor collection - OR Add currency, see if vendor can purchase items.
//            return base.CanAddItem(item);
        }


        public override bool OverrideUseMethod(InventoryItemBase item)
        {
            currentVendor.BuyItemFromVendor(item, false);
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
    }
}