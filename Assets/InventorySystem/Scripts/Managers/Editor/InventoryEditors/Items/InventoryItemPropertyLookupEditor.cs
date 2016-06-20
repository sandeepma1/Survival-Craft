using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{

    [CustomPropertyDrawer(typeof(InventoryItemPropertyLookup), true)]
    public class InventoryItemPropertyLookupEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            
            InventoryEditorUtility.DrawPropertyElement(rect, property, true, true, true, true);

            EditorGUI.EndProperty();
        }
    }
}