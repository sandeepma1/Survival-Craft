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
		public int itemUses;
		public string collection;

		public virtual bool isReference {
			get { return collection != ""; }
		}

		public InventoryItemSerializationModel ()
		{
            
		}

		public InventoryItemSerializationModel (int itemID, uint amount, int uses) : this (itemID, amount, "", uses)
		{
		}

		public InventoryItemSerializationModel (int itemID, uint amount, string collection, int uses)
		{
			this.itemID = itemID;
			this.amount = amount;
			this.collection = collection;
			this.itemUses = uses;
		}
	}
}
