using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(ItemCollectionBase))]
    public partial class CollectionPopulator : MonoBehaviour
    {
        public InventoryItemAmountRow[] items = new InventoryItemAmountRow[0];

        /// <summary>
        /// This will ignore layout sizes, and force the items into the slots.
        /// </summary>
        public bool useForceSet = false;

        private InventoryItemBase[] _items = new InventoryItemBase[0];

        protected void Awake()
        {
            _items = InventoryItemUtility.RowsToItems(items, true);
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].transform.SetParent(transform);
                _items[i].gameObject.SetActive(false);
            }
        }


        protected void Start()
        {
            var col = GetComponent<ItemCollectionBase>();
            if (col == null)
            {
                Debug.LogError("CollectionPopulator can only be used on a collection.", transform);
                return;
            }

            if (useForceSet)
            {
                for (uint i = 0; i < _items.Length; i++)
                {
                    col.SetItem(i, _items[i], true);
                }
            }
            else
            {
                if (col.CanAddItems(_items))
                {
                    col.AddItems(_items);
                }
            }
        }
    }
}