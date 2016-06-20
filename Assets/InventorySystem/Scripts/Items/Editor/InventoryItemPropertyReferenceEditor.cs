using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Devdog.InventorySystem.Editors;

namespace Devdog.InventorySystem.Models
{
    [CustomPropertyDrawer(typeof(InventoryItemPropertyReference), true)]
    public partial class InventoryItemPropertyReferenceEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 4;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var propID = property.FindPropertyRelative("_propertyID");
            var prop = InventoryEditorUtility.PopupField(rect, "Property", ItemManager.database.propertiesStrings, ItemManager.database.properties, o => o.ID == propID.intValue);
            if (prop != null)
            {
                propID.intValue = prop.ID;
            }

            EditorGUI.EndProperty();
        }
    }
}
