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
		public int itemDurability;
		public string collection;

		public virtual bool isReference {
			get { return collection != ""; }
		}

		public InventoryItemSerializationModel ()
		{
            
		}

		public InventoryItemSerializationModel (int itemID, uint amount, int itemDurability) : this (itemID, amount, "", itemDurability)
		{
		}

		public InventoryItemSerializationModel (int itemID, uint amount, string collection, int itemDurability)
		{
			this.itemID = itemID;
			this.amount = amount;
			this.collection = collection;
			this.itemDurability = itemDurability;
		}
	}
}
