using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(ItemPouchInventoryItem), true)]
    public class ItemPouchInventoryItem : InventoryItemBaseEditor
    {

        public override void OnEnable()
        {
            base.OnEnable();

        }

        protected override void DrawPropertyElement(Rect rect, SerializedProperty property, bool isactive, bool isfocused, bool drawRestore, bool drawPercentage)
        {
            base.DrawPropertyElement(rect, property, isactive, isfocused, false, drawPercentage);
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {

            
            base.OnCustomInspectorGUI(extraOverride);
        }
    }
}