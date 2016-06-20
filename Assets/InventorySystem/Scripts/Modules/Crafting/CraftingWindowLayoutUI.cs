using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Windows/Crafting layout")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CraftingWindowLayoutUI : CraftingWindowBase
    {


        //[Header("Behavior")] // Moved to custom editor
        public Dictionary<uint, uint> currentBlueprintItemsDict { get; protected set; }


        [SerializeField]
        protected uint _initialCollectionSize = 9;
        public override uint initialCollectionSize
        {
            get
            {
                return _initialCollectionSize;
            }
        }

        #region UI Elements

        [Header("UI elements")]
        public UnityEngine.UI.Text blueprintTitle;
        public UnityEngine.UI.Text blueprintDescription;

        public InventoryCurrencyUIGroup blueprintCraftCost;

        public UnityEngine.UI.Slider blueprintCraftProgressSlider;
        public UnityEngine.UI.Button blueprintCraftButton;
    
        #endregion

        [Header("Audio & Visuals")]
        public Color enoughGoldColor;
        public Color notEnoughGoldColor;


        private bool canDragInCollectionDefault;
        
        public override void Awake()
        {
            base.Awake();

            canDragInCollectionDefault = canDragInCollection;
            currentBlueprintItemsDict = new Dictionary<uint, uint>(9);

            if(craftingCategory != null)
                SetCraftingCategory(craftingCategory);
        
            blueprintCraftButton.onClick.AddListener(() =>
            {
                if(currentBlueprint != null)
                    CraftItem(currentCategory, currentBlueprint, 1);
            });
        }

        public override void Start()
        {
            base.Start();


            window.OnHide += () =>
            {
                if (cancelCraftingOnWindowClose)
                {
                    CancelActiveCraft();
                }
            };
            window.OnShow += () =>
            {
                ValidateReferences();
                ValidateBlueprint();
            };


            if (useReferences)
            {
                foreach (var col in InventoryManager.GetLootToCollections())
                {
                    col.OnRemovedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
                    {
                        if (window.isVisible == false)
                            return;

                        ValidateReferences();
                        ValidateBlueprint();
                    };
                    col.OnAddedItem += (itemArr, amount, cameFromCollection) =>
                    {
                        if (window.isVisible == false)
                            return;

                        foreach (var i in items)
                        {
                            i.Repaint();
                        }

                        ValidateBlueprint();
                    };
                    col.OnUsedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
                    {
                        if (window.isVisible == false)
                            return;

                        foreach (var i in items)
                        {
                            if (i.item != null && i.item.ID == itemID)
                                i.Repaint();
                        }

                        if (currentBlueprintItemsDict.ContainsKey(itemID))
                        {
                            CancelActiveCraft(); // Used an item that we're using in crafting.
                            ValidateBlueprint();
                        }
                    };
                }
            }
        }


        protected void ValidateReferences()
        {
            if (useReferences == false)
                return;

            foreach (var wrapper in items)
            {
//                if (item.item != null)
//                {
//                    var i = item.item;
//                    // If the item was dropped remove it from the references window
//                    if (i.itemCollection == null)
//                    {
//                        item.item = null;
//                        continue;
//                    }
//
//                    // If the original item no longer exists, scan the inventories for another item, can be null
//                    if (i.itemCollection[i.index].item == null)
//                        item.item = InventoryManager.Find(i.ID, currentCategory.alsoScanBankForRequiredItems);
//
//                    uint count = InventoryManager.GetItemCount(i.ID, currentCategory.alsoScanBankForRequiredItems);
//                    if (count == 0)
//                        item.item = null;
//                }

                if (wrapper.item != null)
                {
                    uint count = InventoryManager.GetItemCount(wrapper.item.ID, currentCategory.alsoScanBankForRequiredItems);
                    if (count == 0)
                    {
                        wrapper.item = null;
                    }
                }

                wrapper.Repaint();
            }
        }

        public override void SetCraftingCategory(InventoryCraftingCategory category)
        {
            base.SetCraftingCategory(category);
            
            if (category.cols * category.rows > items.Length)
            {
                Debug.Log("Increasing crafting layout UI slot count");
                AddSlots((uint)(category.cols * category.rows - items.Length)); // Increase
            }
            else if (category.cols * category.rows < items.Length)
            {
                Debug.Log("Decreasing crafting layout UI slot count");
                RemoveSlots((uint)(items.Length - category.cols * category.rows)); // Decrease
            }
        }

        public override void CancelActiveCraft()
        {
            base.CancelActiveCraft();

            canDragInCollection = canDragInCollectionDefault;
        }

        protected virtual void SetBlueprint(InventoryCraftingBlueprint blueprint)
        {
            Assert.IsNotNull(blueprint, "Given blueprint is null!");

            currentBlueprint = blueprint;

            // Set all the details for the blueprint.
            if (blueprintTitle != null)
                blueprintTitle.text = blueprint.name;

            if (blueprintDescription != null)
                blueprintDescription.text = blueprint.description;

            SetBlueprintResults(blueprint);

            if (blueprintCraftCost != null)
                blueprintCraftCost.Repaint(blueprint.craftingCost);

        }

        /// <summary>
        /// Tries to find a blueprint based on the current layout / items inside the UI item wrappers (items).
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Returns blueprint if found one, null if not.</returns>
        public virtual InventoryCraftingBlueprint GetBlueprintFromCurrentLayout(InventoryCraftingCategory category)
        {
            if(items.Length != category.cols * category.rows)
            {
                Debug.LogWarning("Updating blueprint but blueprint layout cols/rows don't match the collection");
            }

            int totalItemCountInLayout = 0; // Nr of items inside the UI wrappers.
            foreach (var item in items)
            {
                if (item.item != null)
                    totalItemCountInLayout++;
            }

            foreach (var b in GetBlueprints(category))
            {
                foreach (var a in b.blueprintLayouts)
                {
                    if (a.enabled)
                    {
                        var hasItems = new Dictionary<uint, uint>(); // ItemID, amount
                        //var requiredItems = new Dictionary<uint, uint>(); // ItemID, amount
                        currentBlueprintItemsDict.Clear();

                        int counter = 0; // Item index counter
                        int shouldHaveCount = 0; // Amount we should have..
                        int hasCount = 0; // How many slots in our layout
                        bool matchFailed = false;
                        foreach (var r in a.rows)
                        {
                            if (matchFailed)
                                break;

                            foreach (var c in r.columns)
                            {
                                if (c.item != null && c.amount > 0)
                                {
                                    if (currentBlueprintItemsDict.ContainsKey(c.item.ID) == false)
                                        currentBlueprintItemsDict.Add(c.item.ID, 0);

                                    currentBlueprintItemsDict[c.item.ID] += (uint)c.amount;
                                    shouldHaveCount++;

                                    if (items[counter].item != null)
                                    {
                                        if (items[counter].item.ID != c.item.ID)
                                        {
                                            matchFailed = true;
                                            break; // Item in the wrong place...
                                        }

                                        if(hasItems.ContainsKey(c.item.ID) == false)
                                        {
                                            uint itemCount = 0;
                                            if (useReferences)
                                            {
                                                itemCount = InventoryManager.GetItemCount(c.item.ID, category.alsoScanBankForRequiredItems);
                                            }
                                            else
                                            {
//                                                itemCount = items[counter].item.currentStackSize;
                                                itemCount = GetItemCount(c.item.ID);
                                            }

                                            hasItems.Add(c.item.ID, itemCount);
                                        }

                                        hasCount++;
                                    }
                                    else if(items[counter].item == null && c != null)
                                    {
                                        matchFailed = true;
                                        break;
                                    }
                                }

                                counter++;
                            }
                        }

                        if (matchFailed)
                            continue;

                        // Filled slots test
                        if (totalItemCountInLayout != hasCount || shouldHaveCount != hasCount)
                            continue;

                        // Check count
                        foreach (var item in currentBlueprintItemsDict)
                        {
                            if (hasItems.ContainsKey(item.Key) == false || hasItems[item.Key] < item.Value)
                                matchFailed = true;
                        }

                        if (matchFailed == false)
                        {
                            return b;
                        }
                    }
                }
            }

            return null; // Nothing found
        }

        /// <summary>
        /// Check if the bluerint is still valid and craftable.
        /// </summary>
        protected virtual void ValidateBlueprint()
        {
            SetBlueprintResults(null); // Clear the old, check again
            var blueprint = GetBlueprintFromCurrentLayout(currentCategory);
            if (blueprint != null)
            {
                // Found something to craft!
                SetBlueprint(blueprint);
            }
            else
            {
                currentBlueprint = null;
                currentBlueprintItemsDict.Clear();
            }
        }

        public override bool SetItem(uint slot, InventoryItemBase item)
        {
            bool set = base.SetItem(slot, item);
            if(set)
            {
                ValidateBlueprint();
            }

            return set;
        }

        /// <summary>
        /// Called when an item is being crafted.
        /// </summary>
        /// <param name="progress"></param>
        public override void NotifyCraftProgress(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            base.NotifyCraftProgress(category, blueprint, progress);
            
            if (blueprintCraftProgressSlider != null)
                blueprintCraftProgressSlider.value = progress;
        }

        public override void NotifyCraftCanceled(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            base.NotifyCraftCanceled(category, blueprint, progress);

            canDragInCollection = canDragInCollectionDefault;
        }

        protected override void RemoveRequiredCraftItems(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            uint[] keys = currentBlueprintItemsDict.Keys.ToArray();
            uint[] vals = currentBlueprintItemsDict.Values.ToArray();
            if (useReferences)
            {
                // Remove items from inventory
                for (int i = 0; i < keys.Length; i++)
                {
                    InventoryManager.RemoveItem(keys[i], vals[i], category.alsoScanBankForRequiredItems); //  * GetCraftInputFieldAmount()
                }
            }
            else
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    RemoveItem(keys[i], vals[i]);
                }
            }

            // Remove gold
            InventoryManager.RemoveCurrency(blueprint.craftingCost);
        }


        protected override IEnumerator _CraftItem(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, int amount, float currentCraftTime)
        {
            bool canCraft = CanCraftBlueprint(blueprint, category.alsoScanBankForRequiredItems, amount);
            if (canCraft)
            {
                NotifyCraftStart(category, blueprint);
                canDragInCollection = false; // Disable dragging while we're crafting.

                float counter = currentCraftTime;
                while (true)
                {
                    yield return new WaitForSeconds(Time.deltaTime); // Update loop
                    counter -= Time.deltaTime;
                    NotifyCraftProgress(category, blueprint, 1.0f - Mathf.Clamp01(counter / currentCraftTime));

                    if (counter <= 0.0f)
                        break;
                }

                if (CanCraftBlueprint(blueprint, category.alsoScanBankForRequiredItems, amount))
                {
                    RemoveRequiredCraftItems(category, blueprint);
                    GiveCraftReward(category, blueprint);
                }


                amount--;
                currentBlueprintItemsDict.Clear();
                ValidateReferences();
                activeCraft = new ActiveCraft();

                if (amount > 0)
                {
                    CraftItem(category, blueprint, amount);
                }
                else
                {
                    canDragInCollection = canDragInCollectionDefault;
                }
            }
            else
            {
                activeCraft = new ActiveCraft();
                canDragInCollection = canDragInCollectionDefault;
            }
        }


        /// <summary>
        /// Does the inventory contain the required items?
        /// </summary>
        /// <param name="blueprint"></param>
        /// <param name="alsoScanBank"></param>
        /// <param name="craftCount"></param>
        /// <returns></returns>
        public override bool CanCraftBlueprint(InventoryCraftingBlueprint blueprint, bool alsoScanBank, int craftCount)
        {
            bool can = base.CanCraftBlueprint(blueprint, alsoScanBank, craftCount);
            if (can == false)
                return false;

            // No blueprint found
            if (GetBlueprintFromCurrentLayout(currentCategory) == null)
                return false;

            return true;
        }
    }
}