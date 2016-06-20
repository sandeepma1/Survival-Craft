using UnityEngine;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public struct InventoryItemAmountRow
    {

        /// <summary>
        /// The item in this row
        /// </summary>
        public InventoryItemBase item;

        /// <summary>
        /// The amount of items required.
        /// </summary>
        public uint amount;


        public InventoryItemAmountRow(InventoryItemBase item, uint amount)
        {
            this.item = item;
            this.amount = amount;
        }

        public void SetItem(InventoryItemBase item)
        {
            this.item = item;
        }

        public void SetAmount(uint amount)
        {
            this.amount = amount;
        }
    }
}