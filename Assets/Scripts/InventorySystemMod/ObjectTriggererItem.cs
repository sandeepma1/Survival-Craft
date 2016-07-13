using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
	public partial class ObjectTriggererItem
	{
		public bool isPickable = false;

		public override bool Use (InventoryPlayer player, out bool removeSource, bool fireEvents = true)
		{
			if (InventoryUIUtility.CanReceiveInput (gameObject) == false) {
				removeSource = false;
				return false;
			}

			if (inRange) {			
				//print ("picked");	
				if (isPickable) {
					removeSource = PickupItem (player, fireEvents);
				} else {
					removeSource = false;
				}
				if (removeSource) {
					InventoryTriggererManager.instance.currentlyHoveringTriggerer = null;
				}
			} else {
				bool shouldDestroySource;
				var item = GetItem (out shouldDestroySource);
				if (item != null)
					InventoryManager.langDatabase.itemCannotBePickedUpToFarAway.Show (item.name, item.description);
				removeSource = false;
			}

			return removeSource;
		}

	}
}