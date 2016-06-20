using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class ItemRarityEditor : InventorySystemEditorCrudBase<InventoryItemRarity>
    {

        protected override List<InventoryItemRarity> crudList
        {
            get { return new List<InventoryItemRarity>(ItemManager.database.itemRarities); }
            set { ItemManager.database.itemRarities = value.ToArray(); }
        }

        public ItemRarityEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        { }

        protected override bool MatchesSearch(InventoryItemRarity item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search));
        }

        protected override void CreateNewItem()
        {
            var item = new InventoryItemRarity();
            item.ID = (crudList.Count > 0) ? crudList.Max(o => o.ID) + 1 : 0;
            AddItem(item, true);
        }

        public override void DuplicateItem(int index)
        {
            var item = Clone(index);
            item.ID = (crudList.Count > 0) ? crudList.Max(o => o.ID) + 1 : 0;
            item.name += "(duplicate)";
            AddItem(item);
        }

        public override void RemoveItem(int i)
        {
            //            var l = new List<InventoryItemBase>(ItemManager.database.items);
            var allUsingRarity = ItemManager.database.items.Where(o => o.rarity == crudList[i]).ToArray();
            if (allUsingRarity.Length == 0)
            {
                base.RemoveItem(i);
            }
            else
            {
                var window = InventoryDeleteReplaceWithDialog.Get((index, editorWindow) =>
                {
                    if (index == -1)
                    {
                        Debug.Log("Not replacing - Deleting rarity");
                    }
                    else
                    {
                        Debug.Log("Replace rarity with " + ItemManager.database.itemRarities[index].name);
                        foreach (var item in allUsingRarity)
                        {
                            item._rarity = ItemManager.database.itemRarities[index].ID;
                            EditorUtility.SetDirty(item);
                        }
                    }

                    base.RemoveItem(i);
                    editorWindow.Close();

                }, "Rarity", allUsingRarity.Length, ItemManager.database.itemRarityStrings);
                window.Show();
            }
        }


        protected override void DrawSidebarRow(InventoryItemRarity item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.name, 260);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(InventoryItemRarity item, int index)
        {
            EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;


            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            EditorGUILayout.LabelField("ID", item.ID.ToString());
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("The name of the rarity, is displayed in the tooltip in UI elements.", InventoryEditorStyles.infoStyle);
            item.name = EditorGUILayout.TextField("Rarity name", item.name);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("The color displayed in the UI.", InventoryEditorStyles.labelStyle);
            if (item.color.a == 0.0f)
                EditorGUILayout.HelpBox("Color alpha is 0, color is transparent.\nThis might not be intended behavior.", MessageType.Warning);

            item.color = EditorGUILayout.ColorField("Rarity color", item.color);



            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.LabelField("A custom object used when dropping this item like a pouch or chest.", InventoryEditorStyles.infoStyle);
            item.dropObject = (GameObject)EditorGUILayout.ObjectField("Drop object", item.dropObject, typeof(GameObject), false);
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();


            EditorGUIUtility.labelWidth = 0;
        }

        protected override bool IDsOutOfSync()
        {
            return false;
        }

        protected override void SyncIDs()
        {

        }
    }
}
