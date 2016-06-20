using System;
using System.Collections.Generic;


namespace Devdog.InventorySystem.Models
{
    public partial class InventoryItemSerializationModel
    {
        /// <summary>
        /// ID is -1 if no item is in the given slot.
        /// </summary>
        public int itemID;
        public uint amount;

        public string collection;
        public virtual bool isReference
        {
            get { return collection != ""; }
        }

        public InventoryItemSerializationModel()
        {
            
        }

        public InventoryItemSerializationModel(int itemID, uint amount)
            : this(itemID, amount, "")
        { }

        public InventoryItemSerializationModel(int itemID, uint amount, string collection)
        {
            this.itemID = itemID;
            this.amount = amount;
            this.collection = collection;
        }
    }
}
