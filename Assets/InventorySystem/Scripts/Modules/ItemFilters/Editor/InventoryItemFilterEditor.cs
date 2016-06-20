using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{

    [CustomPropertyDrawer(typeof(InventoryItemFilter), true)]
    public class InventoryItemFilterEditor : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var r = position;
            r.width = 100;

            EditorGUI.PropertyField(r, property.FindPropertyRelative("restrictionType"), new GUIContent(""));

            r.x += r.width + 5;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("filterType"), new GUIContent(""));



            int restrictionTypeIndex = property.FindPropertyRelative("restrictionType").enumValueIndex;
            var restrictionType = (InventoryItemFilter.RestrictionType)restrictionTypeIndex;

            int filterTypeIndex = property.FindPropertyRelative("filterType").enumValueIndex;
            InventoryItemFilter.FilterType filterType = (InventoryItemFilter.FilterType)filterTypeIndex;

            r.x += r.width + 5;
            r.width = position.width - 210;

            var stringValue = property.FindPropertyRelative("stringValue");
            var boolValue = property.FindPropertyRelative("boolValue");
            var floatValue = property.FindPropertyRelative("floatValue");
            var intValue = property.FindPropertyRelative("intValue");
            var db = ItemManager.database; // Updates current item database

            switch (restrictionType)
            {
                case InventoryItemFilter.RestrictionType.Type:

                    //r.width -= 65;
                    GUI.enabled = false;
                    var t = System.Type.GetType(stringValue.stringValue);
                    EditorGUI.LabelField(r, t != null ? t.Name : "(NOT SET)");
                    GUI.enabled = true;

                    r.x += r.width - 60;
                    r.width = 60;
                    r.height = 14;
                    if (GUI.Button(r, "Set", "minibutton"))
                    {
                        var typePicker = InventoryItemTypePicker.Get();
                        typePicker.Show(db);
                        typePicker.OnPickObject += type =>
                        {
                            stringValue.stringValue = type.AssemblyQualifiedName;
                            GUI.changed = true; // To save..
                            stringValue.serializedObject.ApplyModifiedProperties();
                        };
                    }

                    break;

                case InventoryItemFilter.RestrictionType.Properties:

                    if (filterType == InventoryItemFilter.FilterType.LessThan || filterType == InventoryItemFilter.FilterType.GreatherThan)
                    {
                        r.width /= 2;

                        var prop = InventoryEditorUtility.PopupField(r, "",
                            ItemManager.database.propertiesStrings,
                            ItemManager.database.properties, o => o.ID == intValue.intValue);
                        if (prop != null)
                        {
                            intValue.intValue = (int) prop.ID;
                        }

//                        intValue.intValue = EditorGUI.Popup(r, intValue.intValue, ItemManager.database.propertiesStrings);

                        r.x += r.width;
                        floatValue.floatValue = EditorGUI.FloatField(r, floatValue.floatValue);
                    }
                    else
                    {
                        var prop = InventoryEditorUtility.PopupField(r, "",
                            ItemManager.database.propertiesStrings,
                            ItemManager.database.properties, o => o.ID == intValue.intValue);
                        if (prop != null)
                        {
                            intValue.intValue = (int)prop.ID;
                        }
                    }

                    break;

                case InventoryItemFilter.RestrictionType.Category:

                    var cat = InventoryEditorUtility.PopupField(r, "",
                        ItemManager.database.itemCategoriesStrings,
                        ItemManager.database.itemCategories, o => o.ID == intValue.intValue);
                    if (cat != null)
                    {
                        intValue.intValue = (int) cat.ID;
                    }

//                    intValue.intValue = EditorGUI.Popup(r, intValue.intValue, ItemManager.database.itemCategoriesStrings);

                    break;

                case InventoryItemFilter.RestrictionType.Rarity:

                    var rar = InventoryEditorUtility.PopupField(r, "",
                        ItemManager.database.itemRarityStrings,
                        ItemManager.database.itemRarities, o => o.ID == intValue.intValue);
                    if (rar != null)
                    {
                        intValue.intValue = (int)rar.ID;
                    }

//                    intValue.intValue = EditorGUI.Popup(r, intValue.intValue, ItemManager.database.itemRarityStrings);

                    break;

                case InventoryItemFilter.RestrictionType.Weight:
                    floatValue.floatValue = EditorGUI.FloatField(r, floatValue.floatValue);
                    break;

                case InventoryItemFilter.RestrictionType.Sellable:
                case InventoryItemFilter.RestrictionType.Storable:
                case InventoryItemFilter.RestrictionType.Droppable:
                    boolValue.boolValue = EditorGUI.Toggle(r, boolValue.boolValue);
                    break;
                default:
                    break;
            }

            EditorGUI.EndProperty();
        }
    }
}