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
		public static InventoryItemBase m_instance = null;

		void Awake ()
		{
			m_instance = this;
		}

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

		[SerializeField]
		private int _itemDurability = 1;

		/// <summary>
		/// ItemQuality
		/// </summary>
		public int itemDurability {
			get {
				return _itemDurability;
			}
			set {
				_itemDurability = value;
			}
		}

		[SerializeField]
		private bool _isPlaceable = false;

		/// <summary>
		/// Is Item Placable
		/// </summary>
		public bool isPlaceable {
			get {
				return _isPlaceable;
			}
			set {
				_isPlaceable = false;
			}
		}

		[SerializeField]
		private bool _isConsumable = false;

		/// <summary>
		/// Is Item Placable
		/// </summary>
		public bool isConsumable {
			get {
				return _isConsumable;
			}
			set {
				_isConsumable = false;
			}
		}

		[SerializeField]
		private int _itemID = 0;

		/// <summary>
		/// Is Item Placable
		/// </summary>
		public int itemID {
			get {
				return _itemID;
			}
			set {
				_itemID = 0;
			}
		}

		public int RemovePlacedItem ()
		{
			int used = Use ();
			if (used < 0)
				return used;

			// Do something with item
			currentStackSize--; // Remove 1
            
			NotifyItemUsed (1, true);
			// InventoryAudioManager.AudioPlayOneShot(audioClipWhenUsed);

			return 1; // 1 item used
		}

	}
}
