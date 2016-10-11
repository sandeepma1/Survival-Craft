using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Integration.SimpleJson;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
	public class JsonItemsSerializer : IItemsSerializer
	{
        
		public virtual object SerializeCollection (ItemCollectionBase collection)
		{
			var serializationModel = new ItemCollectionSerializationModel (collection);
			return Integration.SimpleJson.SimpleJson.SerializeObject (serializationModel);
		}

		public object SerializeItems (IEnumerable<InventoryItemBase> items)
		{
			var serializationModel = new ItemContainerSerializationModel (items);
			return Integration.SimpleJson.SimpleJson.SerializeObject (serializationModel);
		}

		public virtual ItemCollectionSerializationModel DeserializeCollection (object serializedData)
		{
			Assert.IsTrue (serializedData is string, "Serialized data is not string, json collection serializer can only use a JSON string.");

			var jsonObject = (JsonObject)Integration.SimpleJson.SimpleJson.DeserializeObject ((string)serializedData);
			var model = new ItemCollectionSerializationModel ();

			// (generic method to model doesn't seem to work, so do manually).
			var items = jsonObject ["items"] as JsonArray;
			var currencies = jsonObject ["currencies"] as JsonArray;

			model.items = GetItems (items);
			model.currencies = GetCurrencies (currencies);

			return model;
		}

		protected virtual InventoryItemSerializationModel[] GetItems (JsonArray items)
		{
			var arr = new InventoryItemSerializationModel[items.Count];
			for (int i = 0; i < items.Count; i++) {
				var item = (JsonObject)items [i];
				arr [i] = new InventoryItemSerializationModel (int.Parse (item ["itemID"].ToString ()), uint.Parse (item ["amount"].ToString ()), item ["collection"].ToString (), int.Parse (item ["itemDurability"].ToString ()));
			}

			return arr;
		}

		protected virtual InventoryCurrencySerializationModel[] GetCurrencies (JsonArray currencies)
		{
			var arr = new InventoryCurrencySerializationModel[currencies.Count];
			for (int i = 0; i < currencies.Count; i++) {
				var currency = (JsonObject)currencies [i];
				arr [i] = new InventoryCurrencySerializationModel (uint.Parse (currency ["currencyID"].ToString ()), float.Parse (currency ["amount"].ToString ()));
			}

			return arr;
		}


		public ItemContainerSerializationModel DeserializeItems (object serializedData)
		{
			Assert.IsTrue (serializedData is string, "Serialized data is not string, json collection serializer can only use a JSON string.");

			var jsonObject = (JsonObject)Integration.SimpleJson.SimpleJson.DeserializeObject ((string)serializedData);
			var model = new ItemContainerSerializationModel ();

			var items = jsonObject ["items"] as JsonArray;

			model.items = GetItems (items);

			return model;
		}
	}
}
