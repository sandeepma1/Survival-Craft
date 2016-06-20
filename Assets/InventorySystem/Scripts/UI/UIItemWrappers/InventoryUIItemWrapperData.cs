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
    /// <summary>
    /// Used as data object, doesn't show any info.
    /// </summary>
    public partial class InventoryUIItemWrapperData : InventoryUIItemWrapperBase
    {
        public override Material material
        {
            get { return null; }
            set { Debug.LogWarning("Setting material on data object isn't allowed."); }
        }


        public virtual void Awake()
        {
        }


        #region Triggers

        public override void TriggerContextMenu()
        {
            Debug.Log("Trigger actions can't be used on data wrapper.");
        }

        // <inheritdoc />
        public override void TriggerUnstack(ItemCollectionBase toCollection, int toIndex = -1)
        {
            Debug.Log("Trigger actions can't be used on data wrapper.");
        }

        public override void TriggerDrop(bool useRaycast = true)
        {
            Debug.Log("Trigger actions can't be used on data wrapper.");
        }

        public override void TriggerUse()
        {
            Debug.Log("Trigger actions can't be used on data wrapper.");
        }

        #endregion

        /// <summary>
        /// Repaints the item icon and amount.
        /// </summary>
        public override void Repaint()
        {
            
        }
    }
}