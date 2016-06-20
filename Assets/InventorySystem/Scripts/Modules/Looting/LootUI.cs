using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/lootui/")]
    [AddComponentMenu("InventorySystem/Windows/Loot")]
    [RequireComponent(typeof(UIWindow))]
    public partial class LootUI : ItemCollectionBase
    {
        public override uint initialCollectionSize
        {
            get { return 0; }
        }


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


        public override void Awake()
        {
            foreach (var item in items)
            {
                Assert.IsTrue(item.GetType().IsAssignableFrom(typeof(InventoryUIItemWrapperLoot)), "LootUI can only use InventoryUIItemWrapperLoot wrappers (or anything that inherits it), not loot wrappers found.");
            }
        }

        public override void Start()
        {
            base.Start();

            // Closes the window if no objects are left.
            OnRemovedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
            {
                foreach (var i in items)
                {
                    if (i.item != null)
                        return;
                }

                // All slots are empty
                window.Hide();
            };
        }

        // <inheritcdoc />
        public override void SetItems(InventoryItemBase[] newItems, bool setParent, bool repaint = true)
        {
            Resize((uint)items.Length, true); // Force resize, SetItems() doesn't force, hence the extra call.
            base.SetItems(newItems, setParent, repaint);
        }

        public virtual void TakeAll()
        {
            foreach (var item in this.items)
            {
                if(item != null && item.item != null)
                {
                    ((InventoryUIItemWrapperLoot)item).PickupItem();
                }
            }

            window.Hide();
        }

        public override IList<InventoryItemUsability> GetExtraItemUsabilities(IList<InventoryItemUsability> basic)
        {
            var l = base.GetExtraItemUsabilities(basic);
        
            l.Add(new InventoryItemUsability("Loot", (item) =>
            {
                var oldCollection = item.itemCollection;
                uint oldIndex = item.index;

                bool added = InventoryManager.AddItem(item);
                if (added)
                {
                    oldCollection.SetItem(oldIndex, null);
                    oldCollection[oldIndex].Repaint();

                    oldCollection.NotifyItemRemoved(item, item.ID, oldIndex, item.currentStackSize);
                }
            }));

            return l;
        }


        public override bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            return false;    
        }
        public override bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            return false;    
        }
    }
}