using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
	public partial class InventoryItemBase  // My new partial class 
	{
		
		[SerializeField]
		private bool _isHandMined = true;

		/// <summary>
		/// Can the item be Hand Mined or picked?
		/// </summary>
		public bool isHandMined {
			get {
				return _isHandMined;
			}
			set {
				_isHandMined = false;
			}
		}

		[SerializeField]
		private bool _isCurrentlyDropped = false;

		/// <summary>
		/// isDropable
		/// </summary>
		public bool isCurrentlyDropped {
			get {
				return _isCurrentlyDropped;
			}
			set {
				_isCurrentlyDropped = false;
			}
		}

		[SerializeField]
		private float _itemQuality = 1;

		/// <summary>
		/// ItemQuality
		/// </summary>
		public float itemQuality {
			get {
				return _itemQuality;
			}
			set {
				_itemQuality = 1;
			}
		}
	}
}
