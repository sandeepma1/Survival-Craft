#if EASY_SAVE_2

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devdog.InventorySystem.Models;

public class ES2UserType_DevdogInventorySystemModelsItemCollectionSerializationModel : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Devdog.InventorySystem.Models.ItemCollectionSerializationModel data = (Devdog.InventorySystem.Models.ItemCollectionSerializationModel)obj;
		// Add your writer.Write calls here.
		writer.Write(data.items);
		writer.Write(data.currencies);

	}
	
	public override object Read(ES2Reader reader)
	{
		Devdog.InventorySystem.Models.ItemCollectionSerializationModel data = new Devdog.InventorySystem.Models.ItemCollectionSerializationModel();
		// Add your reader.Read calls here and return your object.
		data.items = reader.ReadArray<Devdog.InventorySystem.Models.InventoryItemSerializationModel>();
		data.currencies = reader.ReadArray<Devdog.InventorySystem.Models.InventoryCurrencySerializationModel>();

		return data;
	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_DevdogInventorySystemModelsItemCollectionSerializationModel():base(typeof(Devdog.InventorySystem.Models.ItemCollectionSerializationModel)){}
}

#endif