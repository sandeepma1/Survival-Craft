using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using UnityEngine;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/crafting/")]
    public partial class CraftingWindowBase : ItemCollectionBase
    {
        protected struct ActiveCraft
        {
            public IEnumerator activeCraft;
            public CraftingTriggerer triggerer;
            public InventoryCraftingBlueprint blueprint;

            public ActiveCraft(IEnumerator activeCraft, CraftingTriggerer triggerer, InventoryCraftingBlueprint blueprint)
            {
                this.activeCraft = activeCraft;
                this.triggerer = triggerer;
                this.blueprint = blueprint;
            }

            public AudioSource triggererAudioSource
            {
                get
                {
                    if (triggerer != null)
                    {
                        return triggerer.gameObject.GetComponent<AudioSource>();
                    }

                    return null;
                }
            }
        }

        #region Events

        public delegate void CraftStart(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint);
        public delegate void CraftSuccess(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, IEnumerable<InventoryItemBase> result);
        public delegate void CraftFailed(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint);
        public delegate void CraftProgress(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress);
        public delegate void CraftCanceled(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress);

        public event CraftStart OnCraftStart;
        public event CraftSuccess OnCraftSuccess;
        public event CraftFailed OnCraftFailed;
        public event CraftProgress OnCraftProgress;
        public event CraftCanceled OnCraftCanceled;

        #endregion


        [Header("Crafting")]
        public int craftingCategoryID = 0;
        public InventoryCraftingCategory craftingCategory
        {
            get
            {
#if UNITY_EDITOR
                if (ItemManager.database.craftingCategories.Length == 0)
                {
                    //Debug.LogWarning("Crafting window in the scene, but no crafting categories defined.", transform);
                    return null;
                }
#endif

                return ItemManager.database.craftingCategories[craftingCategoryID];
            }
        }


        public bool cancelCraftingOnWindowClose = true;


        public float currentCraftProgress { get; protected set; }
        protected ActiveCraft activeCraft { get; set; }
        

        private UIWindow _window;
        public UIWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindow>();

                return _window;
            }
            protected set { _window = value; }
        }

        public CraftingTriggerer currentCraftingTriggerer { get; set; }

        public InventoryCraftingCategory currentCategory { get; protected set; }
        public InventoryCraftingBlueprint currentBlueprint { get; protected set; }

        [InventoryRequired]
        public RectTransform blueprintItemResultContainer;

        [InventoryRequired]
        public InventoryUIItemWrapperBase blueprintItemResultPrefab;


        [Header("Audio & Visuals")]
        public AnimationClip craftAnimation;
        //public AnimationClip successAnimation;
        //public AnimationClip failedAnimation;
        //public AnimationClip canceledAnimation;


        #region Notifies

        public virtual void NotifyCraftStart(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            InventoryAudioClip clip = category.craftingAudioClip;
            if (blueprint.overrideCategoryAudioClips)
            {
                clip = blueprint.craftingAudioClip;
            }

            if (clip != null)
            {
                if (activeCraft.triggerer == null)
                {
                    Debug.LogWarning("Can't play crafting audio clip because crafting triggerer isn't set.");
                }
                else
                {
                    var source = activeCraft.triggerer.gameObject.GetComponent<AudioSource>();
                    if (source == null)
                        source = activeCraft.triggerer.gameObject.AddComponent<AudioSource>();

                    source.clip = clip.audioClip;
                    source.volume = clip.volume;
                    source.pitch = clip.pitch;
                    source.loop = clip.loop;
                    source.Play();
                }
            }

            if (OnCraftStart != null)
                OnCraftStart(category, blueprint);
        }

        public virtual void NotifyCraftSuccess(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, IEnumerable<InventoryItemBase> result)
        {
            InventoryManager.langDatabase.craftedItem.Show(blueprint.name, blueprint.description);

            InventoryAudioClip clip = category.successAudioClip;
            if (blueprint.overrideCategoryAudioClips)
            {
                clip = blueprint.successAudioClip;
            }
            InventoryAudioManager.AudioPlayOneShot(clip);


            if (activeCraft.triggerer != null)
            {
                activeCraft.triggererAudioSource.Stop();
            }

            if (OnCraftSuccess != null)
                OnCraftSuccess(category, blueprint, result);
        }

        public virtual void NotifyCraftFailed(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            InventoryManager.langDatabase.craftingFailed.Show(blueprint.name, blueprint.description);

            InventoryAudioClip clip = category.failedAudioClip;
            if (blueprint.overrideCategoryAudioClips)
            {
                clip = blueprint.failedAudioClip;
            }
            
            InventoryAudioManager.AudioPlayOneShot(clip);


            if (activeCraft.triggerer != null)
            {
                activeCraft.triggererAudioSource.Stop();
            }

            if (OnCraftFailed != null)
                OnCraftFailed(category, blueprint);
        }

        public virtual void NotifyCraftProgress(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            currentCraftProgress = progress;

            if (OnCraftProgress != null)
                OnCraftProgress(category, blueprint, progress);
        }
        
        public virtual void NotifyCraftCanceled(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            InventoryManager.langDatabase.craftingCanceled.Show(blueprint.name, blueprint.description, progress);

            InventoryAudioClip clip = category.canceledAudioClip;
            if (blueprint.overrideCategoryAudioClips)
            {
                clip = blueprint.canceledAudioClip;
            }
            InventoryAudioManager.AudioPlayOneShot(clip);


            if (activeCraft.triggerer != null)
            {
                activeCraft.triggererAudioSource.Stop();
            }

            if (OnCraftCanceled != null)
                OnCraftCanceled(currentCategory, currentBlueprint, currentCraftProgress);
        }

        #endregion


        public virtual void SetCraftingCategory(InventoryCraftingCategory category)
        {
            Assert.IsNotNull(category, "Given crafting category is null!");

            if (currentCategory != category)
            {
                CancelActiveCraft();
            }
            currentCategory = category;
        }


        /// <summary>
        /// Does the inventory contain the required items?
        /// </summary>
        /// <param name="blueprint"></param>
        /// <param name="alsoScanBank"></param>
        /// <param name="craftCount"></param>
        /// <returns></returns>
        public virtual bool CanCraftBlueprint(InventoryCraftingBlueprint blueprint, bool alsoScanBank, int craftCount)
        {
            if (InventoryManager.CanRemoveCurrency(blueprint.craftingCost, true, alsoScanBank) == false)
            {
                InventoryManager.langDatabase.userNotEnoughGold.Show(blueprint.name, blueprint.description, craftCount, blueprint.craftingCost.GetFormattedString(craftCount));
                return false;
            }

            var player = InventoryPlayerManager.instance.currentPlayer;
            if (player.characterCollection != null)
            {
                foreach (var propLookup in blueprint.usageRequirementProperties)
                {
                    if (propLookup.CanUse(player) == false)
                    {
                        InventoryManager.langDatabase.craftingCannotPropertyNotValid.Show(blueprint.name, blueprint.description, propLookup.property.name);
                        return false;
                    }
                }
            }

            return true;
        }

        protected virtual InventoryCraftingBlueprint[] GetBlueprints(InventoryCraftingCategory category)
        {
            return currentCategory.blueprints;
        }


        /// <summary>
        /// Crafts the item and triggers the coroutine method to handle the crafting itself.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="blueprint"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CraftItem(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, int amount)
        {
            return CraftItem(category, blueprint, amount, blueprint.craftingTimeDuration);
        }

        /// <summary>
        /// Crafts the item and triggers the coroutine method to handle the crafting itself.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="blueprint"></param>
        /// <param name="amount"></param>
        /// <param name="timeOverride"></param>
        /// <returns></returns>
        public virtual bool CraftItem(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, int amount, float timeOverride)
        {
//            if (activeCraft.activeCraft != null)
//            {
//                Debug.Log("Already crafting, craft call ignored.", gameObject);
//                return false; // Already crafting
//            }

            var c = _CraftItem(category, blueprint, amount, timeOverride);
            activeCraft = new ActiveCraft()
            {
                activeCraft = c,
                triggerer = currentCraftingTriggerer,
                blueprint = blueprint
            };

            StartCoroutine(activeCraft.activeCraft);
            return true;
        }

        protected virtual IEnumerator _CraftItem(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, int amount, float currentCraftTime)
        {
            // Override me!
            return null;
        }

        /// <summary>
        /// Cancels any crafting action that is active. For example when you're crafting an item with a timer, cancel it when you walk away.
        /// </summary>
        public virtual void CancelActiveCraft()
        {
            if (activeCraft.activeCraft != null)
            {
                NotifyCraftCanceled(currentCategory, currentBlueprint, currentCraftProgress);
                StopCoroutine(activeCraft.activeCraft);
                activeCraft = new ActiveCraft();
            }
        }

        protected virtual void SetBlueprintResults(InventoryCraftingBlueprint blueprint)
        {
            if (blueprintItemResultContainer != null)
            {
                if (blueprint != null)
                {
                    foreach (Transform child in blueprintItemResultContainer)
                    {
                        Destroy(child.gameObject);
                    }

                    foreach (var row in blueprint.resultItems)
                    {
                        var wrapper = GameObject.Instantiate<InventoryUIItemWrapperBase>(blueprintItemResultPrefab);

                        wrapper.item = row.item;
                        wrapper.item.currentStackSize = row.amount;
                        wrapper.Repaint();
                        wrapper.item.currentStackSize = 1; // Reset

                        wrapper.transform.SetParent(blueprintItemResultContainer);
                        InventoryUtility.ResetTransform(wrapper.transform);
                    }
                }
                else
                {
                    foreach (Transform child in blueprintItemResultContainer)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }


        protected virtual void RemoveRequiredCraftItems(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            // Remove items from inventory
            foreach (var item in blueprint.requiredItems)
                InventoryManager.RemoveItem(item.item.ID, item.amount, category.alsoScanBankForRequiredItems); //  * GetCraftInputFieldAmount()

            // Remove gold
            InventoryManager.RemoveCurrency(blueprint.craftingCost);
        }


        protected virtual bool GiveCraftReward(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            if (blueprint.successChanceFactor >= UnityEngine.Random.value)
            {
                // Store crafted item
                var itemsToAdd = InventoryItemUtility.RowsToItems(blueprint.resultItems, true);
                if (category.forceSaveInCollection != null)
                {
                    bool added = category.forceSaveInCollection.AddItems(itemsToAdd);
                    Assert.IsTrue(added, "Couldn't add items even though check passed. Please report this error + stack trace.");
                }
                else
                {
                    bool added = InventoryManager.AddItems(itemsToAdd);
                    Assert.IsTrue(added, "Couldn't add items even though check passed. Please report this error + stack trace.");
                }

                NotifyCraftSuccess(category, blueprint, itemsToAdd);
                return true;
            }

            NotifyCraftFailed(category, blueprint);
            return false;
        }

    }
}
