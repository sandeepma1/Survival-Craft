using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.CodeDom.Compiler;
using System.IO;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Editors
{
    using Devdog.InventorySystem.Models;

    using Object = UnityEngine.Object;

    public static class InventoryEditorUtility
    {
        public static void GetAllFieldsInherited(Type startType, List<FieldInfo> appendList)
        {
            if (startType == typeof(MonoBehaviour) || startType == null)
                return;

            // Copied fields can be restricted with BindingFlags
            FieldInfo[] fields = startType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                appendList.Add(field);
            }

            // Keep going untill we hit UnityEngine.MonoBehaviour type.
            GetAllFieldsInherited(startType.BaseType, appendList);
        }

        #region UI helpers

        public static T SimpleObjectPicker<T>(string name, Object o, bool sceneObjects, bool required) where T : Object
        {
            if (o == null && required == true && GUI.enabled)
                GUI.color = Color.red;

            var item = (T)EditorGUILayout.ObjectField(name, o, typeof(T), sceneObjects);
            GUI.color = Color.white;

            return item;
        }
        public static T SimpleObjectPicker<T>(Rect rect, string name, Object o, bool sceneObjects, bool required) where T : Object
        {
            if (o == null && required == true && GUI.enabled)
                GUI.color = Color.red;

            var item = (T)EditorGUI.ObjectField(rect, name, o, typeof(T), sceneObjects);
            GUI.color = Color.white;

            return item;
        }

        public static void ErrorIfEmpty(System.Object o, string msg)
        {
            if (o == null)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static void ErrorIfEmpty(Object o, string msg)
        {
            if (o == null)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static void ErrorIfEmpty(bool o, string msg)
        {
            if (o)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        #endregion


        /// <summary>
        /// Uses conversion between index and ID (if the order of items and ID's don't match)
        /// </summary>
        /// <returns>The object</returns>
        public static T PopupField<T>(string name, string[] stringNames, IEnumerable<T> items, Func<T, bool> predicate) where T : class
        {
            int index = InventoryUtility.FindIndex(items, predicate);
            if (index >= items.Count() || index < 0)
            {
                GUI.color = Color.red;
            }

            index = EditorGUILayout.Popup(name, index, stringNames);
            GUI.color = Color.white;

            if (items.Count() - 1 >= index && index >= 0)
            {
                return items.ElementAt(index);
            }

            return null;
        }

        public static T PopupField<T>(Rect rect, string name, string[] stringNames, IEnumerable<T> items, Func<T, bool> predicate) where T : class
        {
            int index = InventoryUtility.FindIndex(items, predicate);
            if (index >= items.Count() || index < 0)
            {
                GUI.color = Color.red;
            }

            index = EditorGUI.Popup(rect, name, index, stringNames);
            GUI.color = Color.white;

            if (items.Count() - 1 >= index && index >= 0)
            {
                return items.ElementAt(index);
            }

            return null;
        }

        public static void CurrencyLookup(string name, InventoryCurrencyLookup currencyLookup)
        {
            EditorGUILayout.BeginHorizontal();

            var currency = PopupField("", ItemManager.database.pluralCurrenciesStrings, ItemManager.database.currencies, o => o.ID == currencyLookup.currency.ID);
            if (currency != null)
                currencyLookup._currencyID = currency.ID;
            
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            currencyLookup.amount = EditorGUILayout.FloatField("Amount", currencyLookup.amount);
            EditorGUIUtility.labelWidth = prevLabelWidth;

            EditorGUILayout.EndHorizontal();
        }

        private static bool IsPropertyValueValid(object val)
        {
            float singleVal;
            bool isSingle = Single.TryParse(val.ToString(), out singleVal);
            if (isSingle)
            {
                float floatVal = float.Parse(val.ToString());
                return floatVal >= 0f;
            }
            
            return true;
        }

        public static void DrawPropertyElement(Rect rect, SerializedProperty property, bool isActive, bool isFocused, bool drawRestore, bool drawPercentage)
        {
            rect.height = 16;
            rect.y += 2;

            var r2 = rect;
            r2.y += 20;
            r2.width /= 2;
            r2.width -= 5;

            var popupRect = rect;
            popupRect.width /= 2;
            popupRect.width -= 5;

            var propID = property.FindPropertyRelative("_propertyID");
            var propIsFactor = property.FindPropertyRelative("isFactor");
            var propValue = property.FindPropertyRelative("value");
            var propEffect = property.FindPropertyRelative("actionEffect");

            if (ItemManager.database.properties.FirstOrDefault(o => o.ID == propID.intValue) == null)
            {
                if (ItemManager.database.properties.Length == 0)
                {
                    EditorGUILayout.HelpBox("No properties defined", MessageType.Warning);
                    return;
                }

                propID.intValue = 0;
            }

            var prop = PopupField(popupRect, "", ItemManager.database.propertiesStrings, ItemManager.database.properties, o => o.ID == propID.intValue);
            if (prop != null)
            {
                propID.intValue = prop.ID;
            }

            popupRect.x += popupRect.width;
            popupRect.x += 5;

            if (propIsFactor.boolValue)
            {
                popupRect.width -= 65;

                propID.intValue = Mathf.Max(propID.intValue, 0);
                propValue.stringValue = EditorGUI.TextField(popupRect, "", propValue.stringValue);

                var p = popupRect;
                p.x += popupRect.width + 5;
                p.width = 60;

                float floatVal;
                float.TryParse(propValue.stringValue, out floatVal);

                EditorGUI.LabelField(p, "(" + (floatVal - 1.0f) * 100.0f + "%)");
            }
            else
            {
                propID.intValue = Mathf.Max(propID.intValue, 0);
                propValue.stringValue = EditorGUI.TextField(popupRect, "", propValue.stringValue);
            }

            if (drawRestore)
            {
                var r3 = r2;
                r3.width /= 2;
                r3.width -= 5;
                EditorGUI.LabelField(r3, "Action effect");

                r3.x += r3.width + 5;
                EditorGUI.PropertyField(r3, propEffect, new GUIContent(""));

                r2.x += r2.width + 5;
            }

            if (drawPercentage)
            {
                propIsFactor.boolValue = EditorGUI.Toggle(r2, "Is factor (multiplier 0...1)", propIsFactor.boolValue);
                r2.x += r2.width + 5;
            }


            GUI.enabled = true;
        }

        public static void DrawUsageRequirementPropertyElement(Rect rect, SerializedProperty property, bool isActive, bool isFocused, bool drawFilterType)
        {
            rect.height = 16;
            rect.y += 2;
            
            var r2 = rect;
            r2.y += 20;
            r2.width /= 2;
            r2.width -= 5;

            var popupRect = rect;
            popupRect.width /= 2;
            popupRect.width -= 5;

            var propID = property.FindPropertyRelative("_propertyID");
            var propValue = property.FindPropertyRelative("value");
            var filterType = property.FindPropertyRelative("filterType");

            if (ItemManager.database.properties.FirstOrDefault(o => o.ID == propID.intValue) == null)
            {
                if (ItemManager.database.properties.Length == 0)
                {
                    EditorGUILayout.HelpBox("No properties defined", MessageType.Warning);
                    return;
                }

                propID.intValue = 0;
            }

            var prop = PopupField(popupRect, "", ItemManager.database.propertiesStrings, ItemManager.database.properties, o => o.ID == propID.intValue);
            if (prop != null)
            {
                propID.intValue = prop.ID;
            }

            popupRect.x += popupRect.width;
            popupRect.x += 5;

            propID.intValue = Mathf.Max(propID.intValue, 0);


            if (IsPropertyValueValid(propValue.floatValue) == false)
                GUI.color = Color.red;

            propValue.floatValue = EditorGUI.FloatField(popupRect, "", propValue.floatValue);
            GUI.color = Color.white;

            if (drawFilterType)
            {
                var r3 = r2;
                EditorGUI.LabelField(r3, "Filter type");

                r3.x += r3.width + 5;
                EditorGUI.PropertyField(r3, filterType, new GUIContent(""));
            }

            GUI.enabled = true;
        }


        public static void DrawUsageRequirementPropertyElement(Rect rect, InventoryItemPropertyRequirementLookup property, bool isActive, bool isFocused, bool drawFilterType)
        {
            rect.height = 16;
            rect.y += 2;

            var r2 = rect;
            r2.y += 20;
            r2.width /= 2;
            r2.width -= 5;

            var popupRect = rect;
            popupRect.width /= 2;
            popupRect.width -= 5;

            if (ItemManager.database.properties.FirstOrDefault(o => o.ID == property._propertyID) == null)
            {
                if (ItemManager.database.properties.Length == 0)
                {
                    EditorGUILayout.HelpBox("No properties defined", MessageType.Warning);
                    return;
                }

                property._propertyID = 0;
            }

            var prop = PopupField(popupRect, "", ItemManager.database.propertiesStrings, ItemManager.database.properties, o => o.ID == property._propertyID);
            if (prop != null)
            {
                property._propertyID = prop.ID;
            }

            popupRect.x += popupRect.width;
            popupRect.x += 5;

            property._propertyID = Mathf.Max(property._propertyID, 0);


            if (IsPropertyValueValid(property.value) == false)
                GUI.color = Color.red;

            property.value = EditorGUI.FloatField(popupRect, "", property.value);
            GUI.color = Color.white;

            if (drawFilterType)
            {
                EditorGUI.LabelField(r2, "Filter type");

                r2.x += r2.width + 5;
                property.filterType = (InventoryItemPropertyRequirementLookup.FilterType)EditorGUI.EnumPopup(r2, GUIContent.none, property.filterType);
            }

            GUI.enabled = true;
        }

        public static void DrawInventoryItemAmountRow(Rect rect, SerializedProperty property)
        {
            var r2 = rect;
            r2.width /= 2;
            r2.width -= 5;

            var amount = property.FindPropertyRelative("amount");
            var i = property.FindPropertyRelative("item");
            var item = i.objectReferenceValue as InventoryItemBase;

            if (amount.intValue < 1)
            {
                amount.intValue = 1;
            }

            amount.intValue = EditorGUI.IntField(r2, (int)amount.intValue);
            r2.x += r2.width + 5;

            if (item == null)
                GUI.backgroundColor = Color.red;

            if (GUI.Button(r2, (item != null) ? item.name : string.Empty, EditorStyles.objectField))
            {
                var itemPicker = InventoryItemPicker.Get();
                itemPicker.Show(ItemManager.database);
                itemPicker.OnPickObject += obj =>
                {
                    i.serializedObject.UpdateIfDirtyOrScript();
                    i.objectReferenceValue = obj;
                    i.serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                };
            }

            GUI.backgroundColor = Color.white;
        }

        public static void InventoryAudioClip(string name, InventoryAudioClip clip)
        {
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            clip.audioClip = SimpleObjectPicker<AudioClip>(name, clip.audioClip, false, false);
            clip.volume = EditorGUILayout.FloatField("Volume", clip.volume);
            clip.pitch = EditorGUILayout.FloatField("Pitch", clip.pitch);
            clip.loop = EditorGUILayout.Toggle("Loop", clip.loop);
             
            EditorGUILayout.EndVertical();
        }
    }
}