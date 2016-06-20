#if EASY_SAVE_2

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devdog.InventorySystem.Models;

public class ES2UserType_DevdogInventorySystemModelsItemContainerSerializationModel : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Devdog.InventorySystem.Models.ItemContainerSerializationModel data = (Devdog.InventorySystem.Models.ItemContainerSerializationModel)obj;
		// Add your writer.Write calls here.
		writer.Write(data.items);

	}
	
	public override object Read(ES2Reader reader)
	{
		Devdog.InventorySystem.Models.ItemContainerSerializationModel data = new Devdog.InventorySystem.Models.ItemContainerSerializationModel();
		// Add your reader.Read calls here and return your object.
		data.items = reader.ReadArray<Devdog.InventorySystem.Models.InventoryItemSerializationModel>();

		return data;
	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_DevdogInventorySystemModelsItemContainerSerializationModel():base(typeof(Devdog.InventorySystem.Models.ItemContainerSerializationModel)){}
}

#endif