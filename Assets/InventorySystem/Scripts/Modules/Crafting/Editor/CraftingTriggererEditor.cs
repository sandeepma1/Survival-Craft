using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(CraftingTriggerer), true)]
    public class CraftingTriggererEditor : InventoryEditorBase
    {
        //private CraftingStation item;
        private SerializedObject serializer;

        private SerializedProperty craftingCategoryID;
    

        public override void OnEnable()
        {
            base.OnEnable();

            //item = (CraftingStation)target;
            serializer = serializedObject;

            craftingCategoryID = serializer.FindProperty("craftingCategoryID");
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);

            serializedObject.Update();

            // Draws remaining items
            EditorGUILayout.BeginVertical();
            DrawPropertiesExcluding(serializer, new string[]
            {
                "m_Script",
                "craftingCategoryID",
            });
            
            var cat = InventoryEditorUtility.PopupField("Crafting category", ItemManager.database.craftingCategoriesStrings, ItemManager.database.craftingCategories, (category) => category.ID == craftingCategoryID.intValue);
            if (cat != null)
            {
                craftingCategoryID.intValue = cat.ID;
            }
            
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}