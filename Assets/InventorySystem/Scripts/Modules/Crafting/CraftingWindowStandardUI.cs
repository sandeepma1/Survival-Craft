using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Devdog.InventorySystem
{
	[AddComponentMenu ("InventorySystem/Windows/Crafting standard")]
	[RequireComponent (typeof(UIWindow))]
	public partial class CraftingWindowStandardUI : CraftingWindowBase
	{
		//public InventoryCraftingCategory defaultCategory
		//{
		//    get { return ItemManager.database.craftingCategories[defaultCategoryID]; }
		//}


		//[Header("Crafting")]
		//public int defaultCategoryID;

		/// <summary>
		/// Crafting category title
		/// </summary>
		[Header ("General UI references")]
		public UnityEngine.UI.Text currentCategoryTitle;

		/// <summary>
		/// Crafting category description
		/// </summary>
		public UnityEngine.UI.Text currentCategoryDescription;

		[InventoryRequired]
		public RectTransform blueprintsContainer;


		[Header ("Blueprint prefabs")]
		public InventoryCraftingCategoryUI blueprintCategoryPrefab;
        
		/// <summary>
		/// The button used to select the prefab the user wishes to craft.
		/// </summary>
		[InventoryRequired] public InventoryCraftingBlueprintUI blueprintButtonPrefab;

		/// <summary>
		/// A single required item to be shown in the UI.
		/// </summary>
		[InventoryRequired] public InventoryUIItemWrapper blueprintRequiredItemPrefab;

		#region Crafting item page

		[Header ("Craft blueprint UI References")]
		//        public InventoryUIItemWrapper blueprintIcon;
        public UnityEngine.UI.Text blueprintTitle;
		public UnityEngine.UI.Text blueprintDescription;

		[InventoryRequired]
		public RectTransform blueprintRequiredItemsContainer;
		public UnityEngine.UI.Slider blueprintCraftProgressSlider;


		public InventoryCurrencyUIGroup blueprintCurrencyUI;
		//public UnityEngine.UI.Text blueprintCraftCostText;

		/// <summary>
		/// Craft the selected item button
		/// </summary>
		[InventoryRequired]
		public UnityEngine.UI.Button blueprintCraftButton;
		public UnityEngine.UI.Button blueprintMinCraftButton;
		public UnityEngine.UI.InputField blueprintCraftAmountInput;
		public UnityEngine.UI.Button blueprintPlusCraftButton;

		#endregion

		[Header ("UI window pages")]
		public UIWindowPage noBlueprintSelectedPage;
		public UIWindowPage blueprintCraftPage;


		[Header ("Audio & Visuals")]
		public Color itemsAvailableColor = Color.white;
		public Color itemsNotAvailableColor = Color.red;



		#region Pools

		[NonSerialized]
		protected InventoryPool<InventoryCraftingCategoryUI> categoryPool;
        
		[NonSerialized]
		protected InventoryPool<InventoryCraftingBlueprintUI> blueprintPool;

		[NonSerialized]
		protected InventoryPool<InventoryUIItemWrapper> blueprintRequiredItemsPool;

		#endregion


		public override void Awake ()
		{
			base.Awake ();

			if (blueprintCategoryPrefab != null)
				categoryPool = new InventoryPool<InventoryCraftingCategoryUI> (blueprintCategoryPrefab, 16);
            
#if UNITY_EDITOR
			if (blueprintButtonPrefab == null)
				Debug.LogWarning ("Blueprint button prefab is empty in CraftingWindowStandardUI", gameObject);

			if (blueprintRequiredItemPrefab == null)
				Debug.LogWarning ("Blueprint required item prefab is empty in CraftingWindowStandardUI", gameObject);

			if (blueprintCraftButton == null)
				Debug.LogWarning ("Blueprint craft button is requred", gameObject);
#endif

			blueprintPool = new InventoryPool<InventoryCraftingBlueprintUI> (blueprintButtonPrefab, 128);
			blueprintRequiredItemsPool = new InventoryPool<InventoryUIItemWrapper> (blueprintRequiredItemPrefab, 8);

			if (craftingCategoryID >= 0 && craftingCategoryID <= ItemManager.database.craftingCategories.Length - 1)
				currentCategory = craftingCategory;

			if (blueprintMinCraftButton != null) {
				blueprintMinCraftButton.onClick.AddListener (() => {
					if (Input.GetKey (KeyCode.LeftShift))
						blueprintCraftAmountInput.text = (GetCraftInputFieldAmount () - 10).ToString ();
					else
						blueprintCraftAmountInput.text = (GetCraftInputFieldAmount () - 1).ToString ();

					ValidateCraftInputFieldAmount ();
				});
			}
			if (blueprintPlusCraftButton != null) {
				blueprintPlusCraftButton.onClick.AddListener (() => {
					if (Input.GetKey (KeyCode.LeftShift))
						blueprintCraftAmountInput.text = (GetCraftInputFieldAmount () + 10).ToString ();
					else
						blueprintCraftAmountInput.text = (GetCraftInputFieldAmount () + 1).ToString ();

					ValidateCraftInputFieldAmount ();
				});
			}

			blueprintCraftButton.onClick.AddListener (() => {
				if (activeCraft.activeCraft != null) {
					CancelActiveCraft ();
					return;
				}

				if (currentBlueprint == null) {
					Debug.LogWarning ("No blueprint selected, can't craft.");
					return;
				}

				CraftItem (currentCategory, currentBlueprint, GetCraftInputFieldAmount ());
			});
		}

		public override void Start ()
		{
			base.Start ();


			window.OnShow += () => {
				if (currentCategory != null)
					SetCraftingCategory (currentCategory);

				if (currentBlueprint != null)
					SetBlueprint (currentBlueprint);
			};

			window.OnHide += () => {
				if (cancelCraftingOnWindowClose) {
					CancelActiveCraft ();
				}
			};


			foreach (var col in InventoryManager.GetLootToCollections()) {
				col.OnAddedItem += (items, amount, cameFromCollection) => {
					if (window.isVisible) {
						if (currentBlueprint != null)
							SetBlueprint (currentBlueprint);

					}
				};
				col.OnRemovedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) => {
					if (window.isVisible) {
						if (currentBlueprint != null)
							SetBlueprint (currentBlueprint);

					}
				};
				col.OnDroppedItem += (InventoryItemBase item, uint slot, GameObject droppedObj) => {
					if (window.isVisible) {
						CancelActiveCraft (); // If the user drops something.

						if (currentBlueprint != null)
							SetBlueprint (currentBlueprint);

					}
				};
			}

			InventoryManager.instance.inventory.OnCurrencyChanged += (float before, InventoryCurrencyLookup lookup) => {

				if (currentBlueprint != null)
					SetBlueprint (currentBlueprint);
			};
		}

		protected virtual int GetCraftInputFieldAmount ()
		{
			
			if (blueprintCraftAmountInput != null)
				return int.Parse (blueprintCraftAmountInput.text);

			return 1;
		}

		protected virtual void ValidateCraftInputFieldAmount ()
		{
			int amount = GetCraftInputFieldAmount ();
			if (amount < 1)
				amount = 1;
			else if (amount > 999)
				amount = 999;

			blueprintCraftAmountInput.text = amount.ToString ();
		}


		public override void SetCraftingCategory (InventoryCraftingCategory category)
		{
			base.SetCraftingCategory (category);

			categoryPool.DestroyAll ();
			blueprintPool.DestroyAll ();
			if (blueprintCraftAmountInput != null)
				blueprintCraftAmountInput.text = "1"; // Reset
            
			if (currentCategoryTitle != null)
				currentCategoryTitle.text = category.name;
        
			if (currentCategoryDescription != null)
				currentCategoryDescription.text = category.description;

			if (noBlueprintSelectedPage != null)
				noBlueprintSelectedPage.Show ();

//            var blueprints = GetBlueprints(category);
//            if (blueprintCraftPage != null && blueprints.Length > 0)
//            {
//                SetBlueprint(blueprints[0]); // Select first blueprint
//                blueprintCraftPage.Show();
//            }

			int lastItemCategory = -1;
			string lastItemCategoryName = " ";
			//var uiCategory = categoryPool.Get ();
					

			foreach (var b in GetBlueprints(category)) {
				if (b.playerLearnedBlueprint == false)
					continue;

				var blueprintObj = blueprintPool.Get ();
				blueprintObj.transform.SetParent (blueprintsContainer);
				InventoryUtility.ResetTransform (blueprintObj.transform);
				blueprintObj.Set (b);



				if (blueprintCategoryPrefab != null) {
					Assert.IsTrue (b.resultItems.Length > 0, "No reward items set");
					var item = b.resultItems.First ().item;
					Assert.IsNotNull (item, "Empty reward row on blueprint!");

					if (lastItemCategoryName != item.category.name) {
						lastItemCategory = (int)item._category;

						var uiCategory = categoryPool.Get ();
						uiCategory.Set (item.category.name);

						uiCategory.transform.SetParent (blueprintsContainer);
						blueprintObj.transform.SetParent (uiCategory.container);

						InventoryUtility.ResetTransform (uiCategory.transform);
						InventoryUtility.ResetTransform (blueprintObj.transform);
					}
				}

				var bTemp = b; // Store capture list, etc.
				blueprintObj.button.onClick.AddListener (() => {
					currentBlueprint = bTemp;
					SetBlueprint (currentBlueprint);

                    
					// CancelActiveCraft();

					if (blueprintCraftPage != null && blueprintCraftPage.isVisible == false) {
						blueprintCraftPage.Show ();
					}
				});
			}
		}


		protected virtual void SetBlueprint (InventoryCraftingBlueprint blueprint)
		{
			if (window.isVisible == false)
				return;

			// Set all the details for the blueprint.
			if (blueprintTitle != null)
				blueprintTitle.text = blueprint.name;

			if (blueprintDescription != null)
				blueprintDescription.text = blueprint.description;

			SetBlueprintResults (blueprint);
            
			if (blueprintCraftProgressSlider)
				blueprintCraftProgressSlider.value = 0.0f; // Reset

			if (blueprintCurrencyUI != null) {
				blueprintCurrencyUI.Repaint (blueprint.craftingCost);
			}


			blueprintRequiredItemsPool.DestroyAll ();
			foreach (var item in blueprint.requiredItems) {
				var ui = blueprintRequiredItemsPool.Get ();
				item.item.currentStackSize = (uint)item.amount;
				ui.transform.SetParent (blueprintRequiredItemsContainer);
				InventoryUtility.ResetTransform (ui.transform);

				ui.item = item.item;
				if (InventoryManager.GetItemCount (item.item.ID, currentCategory.alsoScanBankForRequiredItems) >= item.amount)
					ui.icon.color = itemsAvailableColor;
				else
					ui.icon.color = itemsNotAvailableColor;

				ui.Repaint ();
				item.item.currentStackSize = 1; // Reset
			}
		}

		/// <summary>
		/// Called when an item is being crafted.
		/// </summary>
		/// <param name="progress"></param>
		public override void NotifyCraftProgress (InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
		{
			base.NotifyCraftProgress (category, blueprint, progress);

			if (blueprintCraftProgressSlider != null)
				blueprintCraftProgressSlider.value = progress;
		}


		protected override IEnumerator _CraftItem (InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, int amount, float currentCraftTime)
		{
			bool canCraft = CanCraftBlueprint (blueprint, category.alsoScanBankForRequiredItems, amount);
			if (canCraft) {
				NotifyCraftStart (category, blueprint);

				float counter = currentCraftTime;
				while (true) {
					yield return new WaitForSeconds (Time.deltaTime); // Update loop
					counter -= Time.deltaTime;
					NotifyCraftProgress (category, blueprint, 1.0f - Mathf.Clamp01 (counter / currentCraftTime));

					if (counter <= 0.0f) {
						break;
					}
				}

				// Double check...
				if (CanCraftBlueprint (blueprint, category.alsoScanBankForRequiredItems, amount)) {
					RemoveRequiredCraftItems (category, blueprint);
					GiveCraftReward (category, blueprint);
				}

				amount--;
				activeCraft = new ActiveCraft ();

				if (amount > 0) {
					if (blueprintCraftAmountInput != null) {
						blueprintCraftAmountInput.text = amount.ToString ();
					}

					CraftItem (category, blueprint, amount, Mathf.Clamp (currentCraftTime / blueprint.craftingTimeSpeedupFactor, 0.0f, blueprint.craftingTimeDuration));
				}
			} else {
				activeCraft = new ActiveCraft ();
			}
		}


		public override bool CanCraftBlueprint (InventoryCraftingBlueprint blueprint, bool alsoScanBank, int craftCount)
		{
			bool can = base.CanCraftBlueprint (blueprint, alsoScanBank, craftCount);
			if (can == false) {
				return false;
			}

			foreach (var item in blueprint.requiredItems) {
				uint count = InventoryManager.GetItemCount (item.item.ID, alsoScanBank);
				if (count < item.amount * craftCount) {
					InventoryManager.langDatabase.craftingDontHaveRequiredItems.Show (item.item.name, item.item.description, blueprint.name);
					return false;
				}
			}

			// Can the items be stored in the inventory / designated spot?
			if (currentCategory.forceSaveInCollection != null) {
				var tester = new ItemCollectionBaseAddCounter ();
				tester.LoadFrom (currentCategory.forceSaveInCollection);
				tester.TryRemoveItems (blueprint.requiredItems);
				var unAdded = tester.TryAdd (blueprint.resultItems);
				if (unAdded.Length > 0) {
					InventoryManager.langDatabase.collectionFull.Show (blueprint.name, blueprint.description, currentCategory.forceSaveInCollection.collectionName);
					return false;
				}
			} else {
				var tester = new ItemCollectionBaseAddCounter ();
				tester.LoadFrom (InventoryManager.GetLootToCollections ());
				tester.TryRemoveItems (blueprint.requiredItems);
				var unAdded = tester.TryAdd (blueprint.resultItems);
				if (unAdded.Length > 0) {
					InventoryManager.langDatabase.collectionFull.Show (blueprint.name, blueprint.description, "Inventory");
					return false;
				}
			}           

			return true;
		}
	}
}