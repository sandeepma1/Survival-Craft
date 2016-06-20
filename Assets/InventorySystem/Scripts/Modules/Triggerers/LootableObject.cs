using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;
using UnityEngine.Serialization;

namespace Devdog.InventorySystem
{
    using System.Linq;

    [HelpURL("http://devdog.nl/documentation/lootables-generators/")]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ObjectTriggerer))]
    [AddComponentMenu("InventorySystem/Triggers/Lootable objet")]
    public partial class LootableObject : MonoBehaviour, IObjectTriggerUser, IInventoryItemContainer
    {
        public delegate void LootedItem(InventoryItemBase item, uint itemID, uint slot, uint amount);
        public delegate void Empty();

        /// <summary>
        /// Called when an item was looted by a player from this lootable object.
        /// </summary>
        public event LootedItem OnLootedItem;

        public event Empty OnEmpty;


        [SerializeField]
        private string _uniqueName;
        public string uniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }



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


        public LootUI lootWindow { get; protected set; }
        public UIWindow window { get; protected set; }

        protected Animator animator;
        public ObjectTriggerer triggerer { get; protected set; }


        public int itemCount
        {
            get
            {
                return items.Sum(o => o == null ? 0 : 1);
            }
        }

        protected virtual void Start()
        {
            //base.Awake();
            lootWindow = InventoryManager.instance.loot;
            if (lootWindow == null)
            {
                Debug.LogWarning("No loot window set, yet there's a lootable object in the scene", transform);
                return;
            }

            if (GetComponent(typeof (IInventoryItemContainerGenerator)) == null)
            {
                // Items were not generated -> Instantiate them

                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = Instantiate<InventoryItemBase>(items[i]);
                    items[i].transform.SetParent(transform);
                    items[i].gameObject.SetActive(false);
                }
            }
            

            window = lootWindow.window;
            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.window = window;
            triggerer.handleWindowDirectly = false; // We're in charge now :)

            animator = GetComponent<Animator>();

            triggerer.OnTriggerUse += Use;
            triggerer.OnTriggerUnUse += UnUse;
        }

        protected void LootWindowOnRemovedItem(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            items[slot] = null;

            if (OnLootedItem != null)
                OnLootedItem(item, itemID, slot, amount);

            if (itemCount == 0)
            {
                if (OnEmpty != null)
                    OnEmpty();

            }
        }

        protected virtual void Use(InventoryPlayer player)
        {
            // Set items
            lootWindow.SetItems(items, true);
            
            lootWindow.OnRemovedItem += LootWindowOnRemovedItem;

            window.Show();
        }

        protected virtual void UnUse(InventoryPlayer player)
        {

            lootWindow.OnRemovedItem -= LootWindowOnRemovedItem;

            window.Hide();
        }
    }
}