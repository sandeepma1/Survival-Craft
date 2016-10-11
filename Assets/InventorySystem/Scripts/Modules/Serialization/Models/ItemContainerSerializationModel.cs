using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Devdog.InventorySystem.Models
{
	public class ItemContainerSerializationModel
	{

		public InventoryItemSerializationModel[] items = new InventoryItemSerializationModel[0];


		public ItemContainerSerializationModel ()
		{
		}

		public ItemContainerSerializationModel (IEnumerable<InventoryItemBase> items)
		{
			this.items = GetSerializationModels (items);
//            this.items = SerializationModelsToItems(items, false);
		}

		public ItemContainerSerializationModel (ItemCollectionBase fillUsingCollection)
		{
			FillUsingCollection (fillUsingCollection);
		}

		/// <summary>
		/// Fill this data model with a collection reference.
		/// Gets the collection data from the collection and stores it in this serializable model.
		/// </summary>
		/// <param name="collection"></param>
		public virtual void FillUsingCollection (ItemCollectionBase collection)
		{
			FillItemsUsingCollection (collection);
		}

		public virtual void FillItemsUsingCollection (ItemCollectionBase collection)
		{
			this.items = GetSerializationModels (collection.Select (o => o.item));
		}

		public virtual InventoryItemSerializationModel[] GetSerializationModels (IEnumerable<InventoryItemBase> items)
		{
			var itemModelsList = new List<InventoryItemSerializationModel> ();
			foreach (var item in items) {
				if (item != null) {					
					itemModelsList.Add (new InventoryItemSerializationModel ((int)item.ID, item.currentStackSize, item.itemCollection.collectionName, item.itemDurability));
				} else {
					itemModelsList.Add (new InventoryItemSerializationModel (-1, 0, "", 1));
				}
			}

			return itemModelsList.ToArray ();
		}

		/// <summary>
		/// Fill a collection using this data object.
		/// </summary>
		/// <param name="collection">The collection to fill using this (ItemCollectionSerializationModel) object.</param>
		public virtual void FillCollectionUsingThis (ItemCollectionBase collection)
		{
			var itemsArray = SerializationModelsToItems (items, collection.useReferences);
			if (collection.useReferences) {
				collection.Resize ((uint)itemsArray.Length);
				for (int i = 0; i < itemsArray.Length; i++) {
					collection [i].item = itemsArray [i];
					collection [i].Repaint ();
				}
			} else {
				collection.SetItems (itemsArray, true);
			}
		}


		public virtual InventoryItemBase[] SerializationModelsToItems (IList<InventoryItemSerializationModel> items, bool useReferences)
		{
			var itemsArray = new InventoryItemBase[items.Count];
			for (int i = 0; i < items.Count; i++) {
				var item = items [i];
				if (item.itemID < 0) {
					itemsArray [i] = null;
				} else {
					var instanceItem = Object.Instantiate<InventoryItemBase> (ItemManager.database.items [item.itemID]);
					instanceItem.currentStackSize = item.amount;
					instanceItem.itemDurability = item.itemDurability;

					if (useReferences) {
						instanceItem.itemCollection = ItemCollectionBase.FindByName (item.collection);
					}

					itemsArray [i] = instanceItem;
				}
			}

			return itemsArray;
		}
	}
}
