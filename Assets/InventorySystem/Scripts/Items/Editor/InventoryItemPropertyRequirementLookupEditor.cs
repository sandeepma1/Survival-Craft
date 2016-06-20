using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Devdog.InventorySystem.Editors;

namespace Devdog.InventorySystem.Models
{
    [CustomPropertyDrawer(typeof(InventoryItemPropertyRequirementLookup), true)]
    public partial class InventoryItemPropertyRequirementLookupEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            InventoryEditorUtility.DrawUsageRequirementPropertyElement(rect, property, true, true, true);

            EditorGUI.EndProperty();
        }
    }
}
