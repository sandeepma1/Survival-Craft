#if EASY_SAVE_2

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devdog.InventorySystem.Models;

public class ES2UserType_DevdogInventorySystemModelsInventoryCurrencySerializationModel : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Devdog.InventorySystem.Models.InventoryCurrencySerializationModel data = (Devdog.InventorySystem.Models.InventoryCurrencySerializationModel)obj;
		// Add your writer.Write calls here.
		writer.Write(data.currencyID);
		writer.Write(data.amount);

	}
	
	public override object Read(ES2Reader reader)
	{
		Devdog.InventorySystem.Models.InventoryCurrencySerializationModel data = new Devdog.InventorySystem.Models.InventoryCurrencySerializationModel();
		// Add your reader.Read calls here and return your object.
		data.currencyID = reader.Read<System.UInt32>();
		data.amount = reader.Read<System.Single>();

		return data;
	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_DevdogInventorySystemModelsInventoryCurrencySerializationModel():base(typeof(Devdog.InventorySystem.Models.InventoryCurrencySerializationModel)){}
}

#endif