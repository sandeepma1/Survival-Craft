using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
	/// <summary>
	/// A physical representation of a crafting station.
	/// </summary>
	[AddComponentMenu ("InventorySystem/Triggers/Crafting triggerer")]
	[RequireComponent (typeof(ObjectTriggerer))]
	public class CraftingTriggerer : MonoBehaviour, IObjectTriggerUser
	{
		public int craftingCategoryID = 0;
		// What category can we craft from?
		protected InventoryCraftingCategory category {
			get {
				return ItemManager.database.craftingCategories.FirstOrDefault (o => o.ID == craftingCategoryID);
			}
		}

		[NonSerialized]
		protected CraftingWindowBase craftingWindow;
		protected static CraftingTriggerer currentCraftingStation;

		[NonSerialized]
		protected ObjectTriggerer triggerer;

        
		protected virtual void Start ()
		{			
			SetWindow ();

			triggerer = GetComponent<ObjectTriggerer> ();
			triggerer.window = craftingWindow.window;
			triggerer.handleWindowDirectly = false; // We're in charge now :)

			if (triggerer.window == null) {
				Debug.LogWarning ("Crafting triggerer created but no CraftingStandardUI found in scene, or not set in managers.", transform);
				return;
			}

			craftingWindow.window.OnHide += () => {
				currentCraftingStation = null;
			};

			triggerer.OnTriggerUse += (player) => {
				craftingWindow.window.Toggle ();

				if (craftingWindow.window.isVisible) {
					currentCraftingStation = this;
					craftingWindow.currentCraftingTriggerer = this;
					craftingWindow.SetCraftingCategory (category);
				}
			};
			triggerer.OnTriggerUnUse += (player) => {
				if (currentCraftingStation == this)
					craftingWindow.window.Hide ();
			};
		}

		protected virtual void SetWindow ()
		{
			if (InventoryManager.instance.craftingStandard == null) {
				Debug.LogWarning ("Crafting triggerer in scene, but no crafting window found", transform);
				return;
			}

			craftingWindow = InventoryManager.instance.craftingStandard;
		}
	}
}