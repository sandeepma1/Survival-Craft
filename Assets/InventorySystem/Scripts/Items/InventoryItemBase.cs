using UnityEngine;
using System.Collections.Generic;
using System;
using Devdog.InventorySystem.Models;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
	using System.Linq;

	/// <summary>
	/// The base item of all the inventory items, contains some default behaviour for items, which can (almost) all be overriden.
	/// </summary>
	[HelpURL ("http://devdog.nl/documentation/creating-a-custom-item-type/")]
	public partial class InventoryItemBase : MonoBehaviour
	{
        
		#region Item data

		#region Convenience properties

		public uint index { get; set; }

		public ItemCollectionBase itemCollection { get; set; }

		#endregion


		[SerializeField]
		[HideInInspector]
		private uint _id;

		/// <summary>
		/// Unique ID of the object
		/// </summary>
		public uint ID {
			get {
				return _id;
			}
			set {
				_id = value;
			}
		}

		[SerializeField]
		private string _name = "";

		/// <summary>
		/// Name of the object (does not have to be unique)
		/// </summary>
		public virtual new string name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}

		[SerializeField]
		private string _description = "";

		/// <summary>
		/// Description of the object.
		/// </summary>
		public virtual string description {
			get {
				return _description;
			}
			set {
				_description = value;
			}
		}

		/// <summary>
		/// The category of this object, defines global cooldown and others.
		/// </summary>
		[SerializeField]
		[InventoryStat]
		public uint _category;

		public InventoryItemCategory category {
			get {
				foreach (var cat in ItemManager.database.itemCategories) {
					if (cat.ID == _category) {
						return cat;
					}
				}

				Debug.LogError ("Couldn't find category on item. Corrupted item? (ItemID #" + ID + ")");
				return null;
			}
			set {
				_category = value.ID;
			}
		}

		/// <summary>
		/// Use the cooldown from the category? If true the global cooldown will be used, if false a unique cooldown can be set.
		/// </summary>
		[SerializeField]
		private bool _useCategoryCooldown = true;

		public bool useCategoryCooldown {
			get {
				return _useCategoryCooldown;
			}
			set {
				_useCategoryCooldown = value;
			}
		}

		[SerializeField]
		private GameObject _overrideDropObjectPrefab;

		public GameObject overrideDropObjectPrefab {
			get { return _overrideDropObjectPrefab; }
			set { _overrideDropObjectPrefab = value; }
		}


		[SerializeField]
		private Sprite _icon;

		/// <summary>
		/// The icon shown in the UI.
		/// </summary>
		public Sprite icon {
			get {
				return _icon;
			}
			set {
				_icon = value;
			}
		}


		[SerializeField]
		[Range (1, 4)]
		private uint _layoutSizeCols = 1;

		public uint layoutSizeCols {
			get { return (uint)Mathf.Max (1, _layoutSizeCols); }
			set { _layoutSizeCols = value; }
		}

		[SerializeField]
		[Range (1, 4)]
		private uint _layoutSizeRows = 1;

		public uint layoutSizeRows {
			get { return (uint)Mathf.Max (1, _layoutSizeRows); }
			set { _layoutSizeRows = value; }
		}

		public uint layoutSize {
			get { return layoutSizeRows * layoutSizeCols; }
		}


		[SerializeField]
		[InventoryStat]
		[Range (0.0f, 999.0f)]
		private float _weight;

		/// <summary>
		/// The weight of the object, KG / LBS / Stone whatever you want, as long as every object uses the same units.
		/// </summary>
		public float weight {
			get {
				return _weight;
			}
			set {
				_weight = value;
			}
		}

		[SerializeField]
		[InventoryStat]
		[Range (0, 100)]
		private uint _requiredLevel;

		/// <summary>
		/// The weight of the object, KG / LBS / Stone whatever you want, as long as every object uses the same units.
		/// </summary>
		public uint requiredLevel {
			get {
				return _requiredLevel;
			}
			set {
				_requiredLevel = value;
			}
		}


		[SerializeField]
		[InventoryStat]
		public uint _rarity = 0;

		/// <summary>
		/// Raritys can be managed through the editor, 
		/// </summary>
		public InventoryItemRarity rarity {
			get {
				var r = ItemManager.database.itemRarities.FirstOrDefault (o => o.ID == _rarity);
				Assert.IsNotNull (r, "Couldn't find rarity on item. Corrupted item? (ItemID #" + ID + ")");
				return r;
			}
		}


		[SerializeField]
		private InventoryItemPropertyLookup[] _properties = new InventoryItemPropertyLookup[0];

		/// <summary>
		/// Item properties, to define your own custom data on items.
		/// If you have a property that repeats itself all the time consider making an itemType (check documentation)
		/// </summary>
		public InventoryItemPropertyLookup[] properties {   
			get {
				return _properties;
			}
			set {
				_properties = value;
			}
		}


		[SerializeField]
		private InventoryItemPropertyRequirementLookup[] _usageRequirementProperties;

		/// <summary>
		/// How much of a specific stat (property) does the user need to have in order to use this item?
		/// Example: requirement of 10 strength > The item can only be used if the player has 10 or more strength.
		/// </summary>
		public InventoryItemPropertyRequirementLookup[] usageRequirementProperties {
			get { return _usageRequirementProperties; }
			set { _usageRequirementProperties = value; }
		}


		[SerializeField]
		private InventoryCurrencyLookup _buyPrice;

		public InventoryCurrencyLookup buyPrice {
			get {
				return _buyPrice;
			}
			set {
				_buyPrice = value;
			}
		}

		[SerializeField]
		private InventoryCurrencyLookup _sellPrice;

		public InventoryCurrencyLookup sellPrice {
			get {
				return _sellPrice;
			}
			set {
				_sellPrice = value;
			}
		}

		[SerializeField]
		private bool _isDroppable = true;

		/// <summary>
		/// Can the item be dropped?
		/// </summary>
		public bool isDroppable {
			get {
				return _isDroppable;
			}
			set {
				_isDroppable = value;
			}
		}

		[SerializeField]
		private bool _isSellable = true;

		/// <summary>
		/// Can the item be sold?
		/// </summary>
		public bool isSellable {
			get {
				return _isSellable;
			}
			set {
				_isSellable = value;
			}
		}

		[SerializeField]
		private bool _isStorable = true;

		/// <summary>
		/// Can the item be stored in a bank / or crate / etc.
		/// </summary>
		public bool isStorable {
			get {
				return _isStorable;
			}
			set {
				_isStorable = value;
			}
		}

		[SerializeField]
		[Range (1, 999)]
		private uint _maxStackSize = 1;

		/// <summary>
		/// How many items fit in 1 pile / stack
		/// </summary>
		public uint maxStackSize {
			get {
				return _maxStackSize;
			}
			set {
				_maxStackSize = value;
			}
		}

		[NonSerialized]
		private uint _currentStackSize = 1;

		/// <summary>
		/// The current amount of items in this stack
		/// </summary>
		public virtual uint currentStackSize {
			get {
				return _currentStackSize;
			}
			set {
				_currentStackSize = value;
			}
		}


		[SerializeField]
		private float _cooldownTime = 0.0f;

		/// <summary>
		/// The time an item is unusable for once it's used.
		/// </summary>
		public float cooldownTime {
			get {
				if (useCategoryCooldown) {
					return category.cooldownTime;
				}

				return _cooldownTime;
			}
		}

		/// <summary>
		/// Used to calculate if the cooldown is over. ((lastUsageTime + cooldown) > Time.TimeSinceStarted).
		/// Only used when useCategoryCooldown is false.
		/// </summary>
		public float lastUsageTime {
			get {
				if (useCategoryCooldown) {
					return category.lastUsageTime;
				}

				// Can't use LINQ -> GC....
				InventoryItemCategory.OverrideCooldownRow row = null;
				foreach (var r in category.overrideCooldownList) {
					if (r.itemID == ID) {
						row = r;
						break;
					}
				}
				if (row != null) {
					return row.lastUsageTime;
				}

				return 0f;
			}
			set {
				if (useCategoryCooldown) {
					category.lastUsageTime = value;
					return;
				}

				// Can't use LINQ -> GC....
				InventoryItemCategory.OverrideCooldownRow row = null;
				foreach (var r in category.overrideCooldownList) {
					if (r.itemID == ID) {
						row = r;
						break;
					}
				}
				if (row == null) {
					row = new InventoryItemCategory.OverrideCooldownRow () {
						itemID = ID,
						lastUsageTime = value
					};
					category.overrideCooldownList.Add (row);
				}

				row.lastUsageTime = value;
			}
		}

		public bool isInCooldown {
			get {
				return Time.timeSinceLevelLoad - lastUsageTime < cooldownTime && lastUsageTime > 0f;
			}
		}

		/// <summary>
		/// Value from 0 to ... that defines how far the cooldown is. 0 is just started 1 or higher means the cooldown is over.
		/// Use isInCooldown first to verify if item is in cooldown.
		/// </summary>
		public float cooldownFactor {
			get {
				return (Time.timeSinceLevelLoad - lastUsageTime) / cooldownTime;
			}
		}


		#endregion

		#region Events

		public delegate void UsedItemItem (uint amount);

		public delegate void DroppedItemItem (GameObject dropObj);

		public delegate void UnstackedItem (uint newSlot, uint amount);

		public delegate void PickedUpItem ();

		public delegate bool CanUseItemDelegate (InventoryItemBase item);

		public event UsedItemItem OnUsedItem;
		public event DroppedItemItem OnDroppedItem;
		public event UnstackedItem OnUnstackedItem;
		public event PickedUpItem OnPickedUpItem;

		/// <summary>
		/// Returns true if the item can be used, and false when the item cannot be used.
		/// Allows you to add your own conditions to items.
		/// </summary>
		public static List<CanUseItemDelegate> canUseItemConditionals { get; protected set; }

		#endregion

		static InventoryItemBase ()
		{
			canUseItemConditionals = new List<CanUseItemDelegate> ();
		}



		/// <summary>
		/// Get the info of this box, useful when displaying this item.
		/// 
		/// Some elements are displayed by default, these are:
		/// Item icon
		/// Item name
		/// Item description
		/// Item rarity
		/// 
		/// </summary>
		/// <returns>
		/// Returns a LinkedList , which works as follows.
		/// Each InfoBoxUI.Row is used to define a row / property of an item.
		/// Each row has a title and description, the color, font type, etc, can all be changed.
		/// </returns>
		public virtual LinkedList<InventoryItemInfoRow[]> GetInfo ()
		{
			var list = new LinkedList<InventoryItemInfoRow[]> ();
        
			list.AddLast (new InventoryItemInfoRow[] {
				new InventoryItemInfoRow ("Weight", weight.ToString ()),
				new InventoryItemInfoRow ("Required level", requiredLevel.ToString ()),
				new InventoryItemInfoRow ("Category", category.name),
			});

			var extra = new List<InventoryItemInfoRow> (3) {
				new InventoryItemInfoRow ("Sell price", sellPrice.GetFormattedString ()),
				new InventoryItemInfoRow ("Buy price", buyPrice.GetFormattedString ())
			};

			if (isDroppable == false || isSellable == false || isStorable == false)
				extra.Add (new InventoryItemInfoRow ((!isDroppable ? "Not droppable" : "") + (!isSellable ? ", Not sellable" : "") + (!isStorable ? ", Not storable" : ""), Color.yellow));

			list.AddLast (extra.ToArray ());

			var extraProperties = new List<InventoryItemInfoRow> ();
			foreach (var property in properties) {
				var prop = property.property;
				if (prop == null) {
					continue;
				}

				if (prop.showInUI) {
					if (property.isFactor && property.isSingleValue)
						extraProperties.Add (new InventoryItemInfoRow (prop.name, (property.floatValue - 1.0f) * 100 + "%", prop.color, prop.color));
					else
						extraProperties.Add (new InventoryItemInfoRow (prop.name, property.value, prop.color, prop.color));
				}
			}

			if (extraProperties.Count > 0)
				list.AddLast (extraProperties.ToArray ());
        
			return list;
		}


		/// <summary>
		/// Returns a list of usabilities for this item, what can it do?
		/// </summary>
		public virtual IList<InventoryItemUsability> GetUsabilities ()
		{
			var l = new List<InventoryItemUsability> (8);

			if (itemCollection.canUseFromCollection) {
				l.Add (new InventoryItemUsability ("Use", (item) => {
					itemCollection [index].TriggerUse ();
				}));
			}

			if (currentStackSize > 1 && itemCollection.canPutItemsInCollection) {
				l.Add (new InventoryItemUsability ("Unstack", (item) => {
					itemCollection [index].TriggerUnstack (itemCollection);
				}));
			}

			if (isDroppable && itemCollection.canDropFromCollection) {
				l.Add (new InventoryItemUsability ("Drop", (item) => {
					itemCollection [index].TriggerDrop (false);
				}));
			}

			return l;
		}

		public virtual bool CanPickupItem ()
		{
			return InventoryManager.CanAddItem (this);
		}

		/// <summary>
		/// Pickups the item and stores it in the Inventory.
		/// </summary>
		/// <returns>Returns 0 if item was stored, -1 if not, -2 for some other unknown reason.</returns>
		public virtual bool PickupItem ()
		{
			//itemCollection = null; // No item collection if we're "picking" up stuff.
			bool pickedUp = InventoryManager.AddItem (this);
			if (pickedUp)
				NotifyItemPickedUp ();

			return pickedUp;
		}

		public void NotifyItemPickedUp ()
		{
			if (OnPickedUpItem != null)
				OnPickedUpItem ();
		}

		/// <summary>
		/// When an item is used, notify the object so that events can be fired.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="alsoNotifyCollection">If the collection of the item didn't change in the process it's safe to notify the collection.</param>
		public virtual void NotifyItemUsed (uint amount, bool alsoNotifyCollection)
		{
			if (itemCollection != null && alsoNotifyCollection)
				itemCollection.NotifyItemUsed (this, ID, index, amount); // Dont forget the collection

			if (OnUsedItem != null)
				OnUsedItem (amount);
		}

		public void NotifyItemUnstacked (uint newSlot, uint amount)
		{
			if (OnUnstackedItem != null)
				OnUnstackedItem (newSlot, amount);
		}

		public void NotifyItemDropped (GameObject dropObj, bool notifyCollection = true)
		{
			if (OnDroppedItem != null)
				OnDroppedItem (dropObj);

			// Keep a list of all items in the world for distance based pickups.



			if (itemCollection != null) {
				// Clear old collection (CLEARS COLLECTION REFERENCE IN THIS OBJECT ALSO!)
				itemCollection.NotifyItemDropped (this, ID, currentStackSize, dropObj);
			}
		}

		public virtual bool VerifyCustomUseConditionals ()
		{
			foreach (var canUse in canUseItemConditionals) {
				if (canUse (this) == false)
					return false;
			}

			return true;
		}

		public virtual bool CanUse ()
		{
			if (itemCollection != null) {
				// Collection denies action
				if (itemCollection.canUseFromCollection == false)
					return false;
			}

			if (VerifyCustomUseConditionals () == false) {
				return false;
			}

			foreach (var prop in properties) {
				if (prop.actionEffect == InventoryItemPropertyLookup.ActionEffect.Decrease) {
					if (prop.actionEffect == InventoryItemPropertyLookup.ActionEffect.Decrease && prop.CanDoDecrease (InventoryPlayerManager.instance.currentPlayer) == false) {
						InventoryManager.instance.sceneLangDatabase.itemCannotBeUsedToLowProperty.Show (name, description, prop.property.name);
						return false;
					}
				}
			}

			foreach (var prop in usageRequirementProperties) {
				if (prop.CanUse (InventoryPlayerManager.instance.currentPlayer) == false) {
					InventoryManager.instance.sceneLangDatabase.itemCannotBeUsedPropertyNotValid.Show (name, description, prop.property.name);
					return false;
				}
			}

			return true;
		}


		/// <summary>
		/// Use the item, returns the amount of items that have been used.
		/// (n) the amount of items that have been used.
		///  0 when 0 items were used but there is still an effect (like a re-usable item that doesn't decrease in stack size)
		/// -1 when the item is in cooldown
		/// -2 when the item cannot be used.
		/// When overriding this method, do not forget to call base.Use();
		/// <b>Note that the caller has to handle the UI repaint.</b>
		/// </summary>
		/// <returns>Returns -1 if in timeout / cooldown, returns -2 if item use failed, 0 is 0 items were used, 1 if 1 item was used, 2 if 2...</returns>
		public virtual int Use ()
		{
			if (CanUse () == false)
				return -2;

			if (itemCollection != null) {
				// Collection has overridden behavior.
				bool overrideBehaviour = itemCollection.OverrideUseMethod (this);
				if (overrideBehaviour)
					return -2;

			}

			if (isInCooldown) {
				InventoryManager.langDatabase.itemIsInCooldown.Show (name, description, lastUsageTime + cooldownTime - Time.timeSinceLevelLoad, cooldownTime);
				return -1;
			}
            
			// Set the last used time, used to figure out if item is in cooldown
			lastUsageTime = Time.timeSinceLevelLoad;

			return 0;
		}

		protected virtual GameObject CreateDropObject ()
		{
			GameObject dropObj = gameObject;
			if (overrideDropObjectPrefab != null) {
				// Drop override drop object
				dropObj = Instantiate<GameObject> (overrideDropObjectPrefab);
				var holder = dropObj.AddComponent<ObjectTriggererItemHolder> ();
				holder.item = this;
			} else if (rarity != null && rarity.dropObject != null) {
				// Drop a specific item whenever this is dropped
				dropObj = Instantiate<GameObject> (rarity.dropObject);
				var holder = dropObj.AddComponent<ObjectTriggererItemHolder> ();
				holder.item = this;
			}

			var triggerer = dropObj.GetComponent<ObjectTriggererItem> ();
			if (triggerer == null) {
				dropObj.AddComponent<ObjectTriggererItem> ();
			}

			return dropObj;
		}

		public virtual Vector3 GetDropPosition (Vector3 initialLocation, Quaternion initialRotation)
		{
			Vector3 dropLocation = Vector3.zero;

			var settings = InventorySettingsManager.instance;


			var dropObj = new GameObject ("TEMP"); // Used to easilly handle the rotation, position (Translate(), etc.)
			if (settings.dropAtMousePosition)
				dropObj.transform.position = initialLocation;
			else
				dropObj.transform.position = initialLocation;


			dropObj.transform.rotation = initialRotation;
			dropObj.transform.Translate (settings.dropOffsetVector);
			dropObj.transform.rotation = Quaternion.identity;


			float droppableFromDistanceUp = 10.0f; // Start at 10.0f
			if (settings.dropItemRaycastToGround) {
				// If there is something above the item, we can't move it up to raycast down, as this would place it on the collider above it. So first check how much we can go up...
				RaycastHit aboveHit;
				if (Physics.Raycast (initialLocation, Vector3.up, out aboveHit, 10.0f)) {
					float dist = Vector3.Distance (aboveHit.transform.position, initialLocation);
					droppableFromDistanceUp = Mathf.Clamp (dist - 0.1f, 0.1f, 10.0f); // Needs to be at least a little above the ground
				}
			}


			if (settings.dropItemRaycastToGround) {
				RaycastHit hit;
				if (Physics.Raycast (initialLocation + (Vector3.up * droppableFromDistanceUp), Vector3.down, out hit, 25.0f)) {
					// place it on the ground
					dropObj.transform.position = hit.point + (Vector3.up * 0.1f); // + a little offset to avoid it falling through the ground
				}
			}

			dropLocation = dropObj.transform.position;
			Destroy (dropObj); // No longer need that...
			print (dropLocation + " item droppeddd");
			return dropLocation;
		}


		public GameObject Drop (Transform transform)
		{
			return Drop (transform.position, transform.rotation);
		}

		public GameObject Drop (Vector3 pos)
		{
			return Drop (pos, Quaternion.identity);
		}

		/// <summary>
		/// Drop item at the specified location.
		/// </summary>
		/// <param name="location">Location.</param>
		/// <returns>Returns the object that is dropped. <b>Dropped object does not have to be the same as this object.</b></returns>
		public virtual GameObject Drop (Vector3 location, Quaternion rotation)
		{
			if (isDroppable == false || (itemCollection != null && itemCollection.canDropFromCollection == false))
				return null;

			var dropObj = CreateDropObject ();
			dropObj.transform.SetParent (null); // Drop item into the world
			dropObj.transform.position = GetDropPosition (location, rotation);
			dropObj.layer = InventorySettingsManager.instance.itemWorldLayer;
			dropObj.SetActive (true);

			NotifyItemDropped (dropObj);

			return dropObj;
		}


		/// <summary>
		/// Unstack this item to the first empty slot
		/// </summary>
		/// <param name="amount"></param>
		public virtual bool UnstackItem (uint amount)
		{
			if (itemCollection == null) {
				Debug.LogWarning ("Can't unstack an item that is not in a collection", transform);
				return false;
			}

			return itemCollection.UnstackSlot (index, amount);
		}


		/// <summary>
		/// Unstack this item
		/// </summary>
		/// <param name="toCollection"></param>
		/// <param name="toSlot"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public virtual bool UnstackItem (ItemCollectionBase toCollection, uint toSlot, uint amount)
		{
			if (itemCollection == null) {
				Debug.LogWarning ("Can't unstack an item that is not in a collection", transform);
				return false;
			}

			return itemCollection.UnstackSlot (index, toCollection, toSlot, amount);
		}

		/// <summary>
		/// A very un-efficient way to check if an object is an instance object or not.
		/// Note this method is O(n), so it's rather slow...
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool IsInstanceObject ()
		{
			return !ItemManager.database.items.Contains (this);
			//            return GetInstanceID() <= 0; // TODO: Fix -> Unity bug
		}

		public override string ToString ()
		{
			return name;
		}
	}
}