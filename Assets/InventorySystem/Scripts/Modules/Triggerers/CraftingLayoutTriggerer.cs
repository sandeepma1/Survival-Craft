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
    [AddComponentMenu("InventorySystem/Triggers/Crafting alyout triggerer")]
    [RequireComponent(typeof(ObjectTriggerer))]
    public class CraftingLayoutTriggerer : CraftingTriggerer
    {
        protected override void SetWindow()
        {
            if (InventoryManager.instance.craftingLayout == null)
            {
                Debug.LogWarning("Crafting triggerer in scene, but no crafting window found", transform);
                return;
            }

            craftingWindow = InventoryManager.instance.craftingLayout;
        }
    }
}