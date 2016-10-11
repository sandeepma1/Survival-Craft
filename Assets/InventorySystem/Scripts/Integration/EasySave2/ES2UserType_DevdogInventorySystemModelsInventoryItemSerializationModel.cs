#if EASY_SAVE_2

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devdog.InventorySystem.Models;

public class ES2UserType_DevdogInventorySystemModelsInventoryItemSerializationModel : ES2Type
{
	public override void Write (object obj, ES2Writer writer)
	{
		Devdog.InventorySystem.Models.InventoryItemSerializationModel data = (Devdog.InventorySystem.Models.InventoryItemSerializationModel)obj;
		// Add your writer.Write calls here.
		writer.Write (data.itemID);
		writer.Write (data.amount);
		writer.Write (data.collection);
		writer.Write (data.itemDurability);
	}

	public override object Read (ES2Reader reader)
	{
		Devdog.InventorySystem.Models.InventoryItemSerializationModel data = new Devdog.InventorySystem.Models.InventoryItemSerializationModel ();
		// Add your reader.Read calls here and return your object.
		data.itemID = reader.Read<System.Int32> ();
		data.amount = reader.Read<System.UInt32> ();
		data.collection = reader.Read<System.String> ();
		data.itemDurability = reader.Read<System.Int32> ();

		return data;
	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_DevdogInventorySystemModelsInventoryItemSerializationModel () : base (typeof(Devdog.InventorySystem.Models.InventoryItemSerializationModel))
	{
	}
}

#endif