using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.InventorySystem.Models
{
	public partial class InventoryCraftingBlueprint // My new partial class 
	{
		public Sprite icon {
			get {
				if (useItemResultNameAndDescription) {
					if (resultItems.Length == 0) {
						return resultItems.First ().item.icon;
					}
					return resultItems.First ().item != null ? resultItems.First ().item.icon : resultItems.First ().item.icon;
				}
				return customIcon;
			}
		}

		public Sprite customIcon;
	}
}