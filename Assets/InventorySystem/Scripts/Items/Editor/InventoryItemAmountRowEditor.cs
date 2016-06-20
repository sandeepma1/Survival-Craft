using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Devdog.InventorySystem.Editors;

namespace Devdog.InventorySystem.Models
{
    [CustomPropertyDrawer(typeof(InventoryItemAmountRow), true)]
    public partial class InventoryItemAmountRowEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            InventoryEditorUtility.DrawInventoryItemAmountRow(rect, property);

            EditorGUI.EndProperty();
        }
    }
}
