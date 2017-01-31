using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Devdog.InventorySystem
{
	[AddComponentMenu ("InventorySystem/Player/Inventory player")]
	public partial class InventoryPlayer : InventoryPlayerBase
	{
		public delegate void PickedUpItem (uint itemID, uint itemAmount);

		public event PickedUpItem OnPickedUpItem;

		[SerializeField]
		private bool isPlayerDynamicallyInstantiated = false;


		private InventoryPlayerRangeHelper _rangeHelper;

		public InventoryPlayerRangeHelper rangeHelper {
			get {
				if (_rangeHelper == null) {
					var comps = GetComponentsInChildren<InventoryPlayerRangeHelper> (true); // GetComponentInChildren (single) doesn't grab in-active objects.
					_rangeHelper = comps.Length > 0 ? comps [0] : null;
				}

				return _rangeHelper;
			}
			protected set { _rangeHelper = value; }
		}

		public InventoryPlayerEquipHelper equipHelper { get; set; }

		public bool isInitialized { get; protected set; }

		/// <summary>
		/// Initialize this player. The player will be added to the players list ( assigned to the InventoryPlayerManager )
		/// </summary>
		public void Init ()
		{
			Assert.IsFalse (isInitialized, "Tried to initialize player - Player was already initialized!");
			isInitialized = true;

			if (dynamicallyFindUIElements) {
				FindUIElements ();
			}

			if (characterCollection != null) {
				characterCollection.player = this;
			}

			UpdateEquipLocations ();
			equipHelper = new InventoryPlayerEquipHelper (this);
			InventoryPlayerManager.AddPlayer (this);
		}

		protected override void Awake ()
		{
			base.Awake ();

			if (isPlayerDynamicallyInstantiated == false) {
				Init ();
			}
		}

		public void NotifyPickedUpItem (uint itemID, uint itemAmount)
		{
			if (OnPickedUpItem != null)
				OnPickedUpItem (itemID, itemAmount);
		}
        
		//        public void SetActive(bool active)
		//        {
		//            this.enabled = active;
		//            this.rangeHelper.enabled = active;
		//
		//            var userControl = gameObject.GetComponent<IInventoryPlayerController>();
		//            if (userControl == null)
		//            {
		//                Debug.LogWarning("No component found on player that implements IInventoryPlayerController. If you implement your own controller, be sure to implement IInventoryPlayerController.", transform);
		//                return;
		//            }
		//
		//            userControl.SetActive(active);
		//        }


		/// <summary>
		/// For collider based characters
		/// </summary>
		/// <param name="col"></param>
		public virtual void OnTriggerEnter (Collider col)
		{
			//print (col + "Trigger");
			TryPickup (col.gameObject);
		}


		/// <summary>
		/// For 2D collider based characters
		/// </summary>
		/// <param name="col"></param>
		public virtual void OnTriggerEnter2D (Collider2D col)
		{
			TryPickup (col.gameObject);
		}

		/// <summary>
		/// Collision pickup attempts
		/// </summary>
		/// <param name="obj"></param>
		protected virtual void TryPickup (GameObject obj)
		{
			// Just for safety in-case the collision matrix isn't set up correctly..
			if (obj.layer == InventorySettingsManager.instance.equipmentLayer)
				return;

			if (InventorySettingsManager.instance.itemTriggerOnPlayerCollision || CanPickupGold (obj)) {
				var item = obj.GetComponent<ObjectTriggererItem> ();
				if (item != null)
					item.Use (this);
			}
		}

		protected virtual bool CanPickupGold (GameObject obj)
		{
			return InventorySettingsManager.instance.alwaysTriggerGoldItemPickupOnPlayerCollision && obj.GetComponent<CurrencyInventoryItem> () != null;
		}

		/// <summary>
		/// Add the range helper this object depends on.
		/// </summary>
		public void AddRangeHelper ()
		{
			var col = new GameObject ("_Col");
			col.transform.SetParent (transform);
			InventoryUtility.ResetTransform (col.transform);

			col.gameObject.AddComponent<InventoryPlayerRangeHelper> ();
		}
	}
}
