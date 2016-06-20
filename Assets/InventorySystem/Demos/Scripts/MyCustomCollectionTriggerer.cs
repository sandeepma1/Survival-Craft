using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    [RequireComponent(typeof(ObjectTriggerer))]
    public class MyCustomCollectionTriggerer : MonoBehaviour, IInventoryItemContainer
    {
        [SerializeField]
        private InventoryItemBase[] _items;
        public InventoryItemBase[] items
        {
            get { return _items; }
            set { _items = value; }
        }


        [SerializeField]
        private string _uniqueName;
        public string uniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        private CollectionToArraySyncer _syncer;


        public void Awake()
        {
            // Create instance objects.
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    items[i] = GameObject.Instantiate<InventoryItemBase>(items[i]);
                    items[i].transform.SetParent(transform);
                    items[i].gameObject.SetActive(false);
                }
            }


            // The triggererHandler component, that is always there because of RequireComponent
            var triggerer = GetComponent<ObjectTriggerer>();

            // The collection we want to place the items into.
            var collection = triggerer.window.GetComponent<ItemCollectionBase>();

            // When the trigger is used by the player, behavior can be modified in the inspector.
            triggerer.OnTriggerUse += (player) =>
            {
                // When the user has triggered this object, set the items in the window
                collection.SetItems(items, true);

                _syncer = new CollectionToArraySyncer(collection, items);
                _syncer.StartSyncing();

                // And done!
            };

            triggerer.OnTriggerUnUse += (player) =>
            {
                _syncer.StopSyncing();
            };
        }
    }
}