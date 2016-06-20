using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(CraftingWindowLayoutUI), true)]
    public class CraftingWindowLayoutUIEditor : ItemCollectionBaseEditor
    {
        //private CraftingStation item;
        private SerializedProperty craftingCategoryID;

        private string[] allCraftingCategories
        {
            get
            {
                var l = new string[ItemManager.database.craftingCategories.Length];
                for (int i = 0; i < ItemManager.database.craftingCategories.Length; i++)
                    l[i] = ItemManager.database.craftingCategories[i].name;

                return l;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            craftingCategoryID = serializedObject.FindProperty("craftingCategoryID");
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraSpecific)
        {
            base.OnCustomInspectorGUI(new CustomOverrideProperty(craftingCategoryID.name, () =>
            {
                GUILayout.Label("Behavior", InventoryEditorStyles.titleStyle);
                craftingCategoryID.intValue = EditorGUILayout.Popup("Crafting category", craftingCategoryID.intValue, allCraftingCategories);
            }));
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}