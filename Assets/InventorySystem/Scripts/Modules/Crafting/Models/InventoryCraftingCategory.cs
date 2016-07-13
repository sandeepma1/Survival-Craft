using UnityEngine;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Models
{
	[System.Serializable]
	public partial class InventoryCraftingCategory
	{
		/// <summary>
		/// The unique ID as well as the Index in the ItemManager
		/// </summary>
		[HideInInspector]
		public int ID;

		/// <summary>
		/// The name of this category.
		/// </summary>
		public string name;

		/// <summary>
		/// The description of this category.
		/// </summary>
		public string description;

		public Sprite icon;

		/// <summary>
		/// Also scan through the bank for items to use when crafting the item.
		/// </summary>
		public bool alsoScanBankForRequiredItems = false;

		/// <summary>
		/// Force the item to be saved in this collection once the craft is completed.
		/// </summary>
		public ItemCollectionBase forceSaveInCollection;

		/// <summary>
		/// Amount of rows for layouts
		/// </summary>
		public uint rows = 3;

		/// <summary>
		/// Amount of cols for layouts
		/// </summary>
		public uint cols = 3;

		/// <summary>
		/// All available blueprints. Blueprints are craftable objects.
		/// </summary>
		public InventoryCraftingBlueprint[] blueprints = new InventoryCraftingBlueprint[0];

		/// <summary>
		/// The audio clip played when the craft has succeeded
		/// </summary>
		public InventoryAudioClip successAudioClip = new InventoryAudioClip ();

		/// <summary>
		/// The audio clip played on the crafting triggerer while crafting an item. The audio clip can be looped.
		/// </summary>
		public InventoryAudioClip craftingAudioClip = new InventoryAudioClip () { loop = true };

		/// <summary>
		/// The audio clip played when the craft was canceled.
		/// </summary>
		public InventoryAudioClip canceledAudioClip = new InventoryAudioClip ();
        
		/// <summary>
		/// The audio cilp played when the craft has failed.
		/// </summary>
		public InventoryAudioClip failedAudioClip = new InventoryAudioClip ();


		public override string ToString ()
		{
			return name;
		}
	}
}