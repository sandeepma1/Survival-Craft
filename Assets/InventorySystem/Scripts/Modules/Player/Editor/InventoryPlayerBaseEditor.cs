using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using UnityEditorInternal;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(InventoryPlayerBase), true)]
    public class InventoryPlayerBaseEditor : InventoryEditorBase
    {
        protected ReorderableList inventoriesList;
        protected ReorderableList inventoryCollectionsList;
        protected ReorderableList equipLocationsList;

        protected static InventoryItemDatabase itemDatabase;
        protected SerializedProperty equipLocations;

        protected SerializedProperty dynamicallyFindUIElements;
        protected SerializedProperty characterCollectionName;
        protected SerializedProperty inventoryCollectionNames;
        protected SerializedProperty skillbarCollectionName;


        protected SerializedProperty characterCollection;
        protected SerializedProperty inventoryCollections;
        protected SerializedProperty skillbarCollection;


        private InventoryPlayerBase tar;

//        private bool IsOutOfSync()
//        {
//            var equipSlotFields = tar.characterCollection.container.GetComponentsInChildren<InventoryEquippableField>(true);
//            return equipSlotFields.Length != equipLocations.arraySize;
//        }

        private void ReSync()
        {
            if (tar.dynamicallyFindUIElements)
            {
                tar.FindUIElements();
            }

            if ((tar.characterCollection == null || tar.characterCollection.container == null))
            {
                Debug.LogWarning("Can't scan for wrappers, character collection not set.");
                return;
            }

            var newList = new List<InventoryPlayerEquipTypeBinder>();
            var equipSlotFields = tar.characterCollection.container.GetComponentsInChildren<InventoryEquippableField>(true);

            for (int i = 0; i < equipSlotFields.Length; i++)
            {
                InventoryPlayerEquipTypeBinder toAdd = new InventoryPlayerEquipTypeBinder(equipSlotFields[i], null, InventoryPlayerEquipHelper.EquipHandlerType.MakeChild);

                // Find in old data
                var found = tar.equipLocations.FirstOrDefault(o => o.associatedField == equipSlotFields[i]);
                if (found != null)
                {
                    toAdd = found;
                    //toAdd = new InventoryPlayerEquipTypeBinder(t.equipLocations[i].associatedField, t.equipLocations[i].equipTransform);
                }

                newList.Add(toAdd); 
            }


            tar.equipLocations = newList.ToArray();
            GUI.changed = true; // To save

            equipLocationsList.list = tar.equipLocations; // Update equipLocationsList
            Repaint();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            tar = (InventoryPlayerBase)target;
            equipLocations = serializedObject.FindProperty("equipLocations");

            dynamicallyFindUIElements = serializedObject.FindProperty("dynamicallyFindUIElements");
            characterCollectionName = serializedObject.FindProperty("characterCollectionName");
            inventoryCollectionNames = serializedObject.FindProperty("inventoryCollectionNames");
            skillbarCollectionName = serializedObject.FindProperty("skillbarCollectionName");

            characterCollection = serializedObject.FindProperty("characterCollection");
            inventoryCollections = serializedObject.FindProperty("inventoryCollections");
            skillbarCollection = serializedObject.FindProperty("skillbarCollection");

            inventoriesList = new ReorderableList(serializedObject, inventoryCollectionNames, false, true, true, true);
            inventoriesList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Inventory paths");
            };
            inventoriesList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                var prop = inventoryCollectionNames.GetArrayElementAtIndex(index);
                if (tar.FindElement<ItemCollectionBase>(prop.stringValue, false) == null)
                {
                    var r = rect;
                    r.height = 18;
                    r.width = 140;
                    r.y -= 2;
                    r.x += 5;

                    rect.width -= (r.width + 10);
                    rect.x += (r.width + 10);

                    var style = new GUIStyle((GUIStyle) "CN EntryWarn")
                    {
                        wordWrap = false,
                        fixedHeight = rect.height,
                        fontStyle = FontStyle.Bold
                    };
                    EditorGUI.LabelField(r, "(Not found)", style);
                }

                EditorGUI.PropertyField(rect, prop);

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(tar);
                }
            };


            inventoryCollectionsList = new ReorderableList(serializedObject, inventoryCollections, false, true, true, true);
            inventoryCollectionsList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Inventory collections");
            };
            inventoryCollectionsList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                EditorGUI.PropertyField(rect, inventoryCollections.GetArrayElementAtIndex(index));

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(tar);
                }
            };


            



            itemDatabase = ItemManager.database;
            if (itemDatabase == null)
                Debug.LogError("No item database found in scene, cannot edit item.");


            equipLocationsList = new ReorderableList(serializedObject, equipLocations, false, true, false, false);
            equipLocationsList.elementHeight = 72;
            equipLocationsList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Equipment binding");
            };
            equipLocationsList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                var r3 = rect;
                r3.width = 25;

                rect.x += 30;
                rect.width -= 30;
                var r = rect;
                var r2 = rect;
                r2.width -= EditorGUIUtility.labelWidth;
                r2.x += EditorGUIUtility.labelWidth;

                var element = equipLocations.GetArrayElementAtIndex(index);

                r3.x += 8;
                r3.y += 24;
                var t = element.FindPropertyRelative("equipTransform").objectReferenceValue as Transform;
                if (t != null && t.IsChildOf(((InventoryPlayer) target).transform))
                {
                    EditorGUI.Toggle(r3, GUIContent.none, true, "VisibilityToggle");
                }
                else
                {
                    EditorGUI.Toggle(r3, GUIContent.none, false, "VisibilityToggle");                    
                }
                


                GUI.enabled = false;
                EditorGUI.ObjectField(r, element.FindPropertyRelative("associatedField"), new GUIContent("Associated field"));
                GUI.enabled = true;

                rect.y += 18;

                DrawSingleEquipField(element, rect);

                rect.y += 18;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("equipHandlerType"));
            };

        }

        protected virtual void DrawSingleEquipField(SerializedProperty element, Rect rect)
        {
            var rectLeftField = rect;
            rectLeftField.width = 20;
            rectLeftField.x += InventoryEditorStyles.labelWidth;

            var findDynamic = element.FindPropertyRelative("findDynamic");
            var equipTransformPath = element.FindPropertyRelative("equipTransformPath");
            var equipTransform = element.FindPropertyRelative("equipTransform");

            EditorGUI.PropertyField(rectLeftField, findDynamic, new GUIContent(""));

            if (findDynamic.boolValue)
            {
                EditorGUI.PropertyField(rect, equipTransformPath);
            }
            else
            {
                EditorGUI.PropertyField(rect, equipTransform);
            }

            var t = element.FindPropertyRelative("equipTransform").objectReferenceValue as Transform;
            if (t != null && t.IsChildOf(((InventoryPlayer)target).transform) == false)
            {
                rect.y += 18;
                EditorGUI.HelpBox(rect, "EquippedItem transform has to be a child of this character.", MessageType.Error);
            }
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            OnCustomInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();


            if (tar.dynamicallyFindUIElements)
            {
                tar.FindUIElements(false);
            }

            if (tar.characterCollection == null)
            {
                EditorGUILayout.HelpBox("This player spawner isn't attached to a CharacterUI, disabling visual equipment / mesh binding.", MessageType.Warning);
                return;
            }

            if (tar.characterCollection.container == null)
            {
                EditorGUILayout.HelpBox("This player spawner is attached to a CharacterUI, but the CharacterUI's container is null.\nEquip locations cannot be scanned..", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("Force rescan"))
            {
                ReSync();
            }


            equipLocationsList.DoLayoutList();


            serializedObject.ApplyModifiedProperties();
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);

            if (itemDatabase == null)
                return;



            // Draws remaining items
            DrawPropertiesExcluding(serializedObject, new[]
            {
                "m_Script",
                "equipLocations",
                "dynamicallyFindUIElements",
                "characterCollectionName",
                "inventoryCollectionNames",
                "skillbarCollectionName",
                "characterCollection",
                "inventoryCollections",
                "skillbarCollection"
            });

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Player's collections", InventoryEditorStyles.titleStyle);
            EditorGUILayout.PropertyField(dynamicallyFindUIElements);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            if (dynamicallyFindUIElements.boolValue)
            {
                EditorGUILayout.PropertyField(characterCollectionName, true);
                inventoriesList.DoLayoutList();
                EditorGUILayout.PropertyField(skillbarCollectionName, true);
            }
            
            if(dynamicallyFindUIElements.boolValue == false || EditorApplication.isPlaying)
            {
                GUI.enabled = !EditorApplication.isPlaying;

                EditorGUILayout.PropertyField(characterCollection, true);
                inventoryCollectionsList.DoLayoutList();
                EditorGUILayout.PropertyField(skillbarCollection, true);

                GUI.enabled = true;
            }

            EditorGUILayout.EndVertical();
        }
    }
}