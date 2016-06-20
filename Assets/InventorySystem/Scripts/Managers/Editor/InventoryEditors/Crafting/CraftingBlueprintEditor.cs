﻿using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class CraftingBlueprintEditor : InventorySystemEditorCrudBase<InventoryCraftingBlueprint>
    {
        protected override List<InventoryCraftingBlueprint> crudList
        {
            get { return new List<InventoryCraftingBlueprint>(ItemManager.database.craftingCategories[categoryIndex].blueprints); }
            set { ItemManager.database.craftingCategories[categoryIndex].blueprints = value.ToArray(); }
        }

        private int categoryIndex
        {
            get
            {
                for(int i = 0; i < parentEditor.childEditors.Count; i++)
                {
                    if (parentEditor.childEditors[i] == this)
                        return i;
                }

                Debug.Log("Child editor not found, please report this bug + stack trace");
                return 0;
            }
        }

        private InventoryCraftingCategory category
        {
            get { return ItemManager.database.craftingCategories[categoryIndex]; }
        }



        public Vector2 scrollPos;
        private EmptyEditor parentEditor { get; set; }
        private UnityEditorInternal.ReorderableList requiredItemsList { get; set; }
        private UnityEditorInternal.ReorderableList resultItemsList { get; set; }
        private UnityEditorInternal.ReorderableList usageRequirementPropertiesList { get; set; }


        public CraftingBlueprintEditor(string singleName, string pluralName, EditorWindow window, EmptyEditor parentEditor)
            : base(singleName, pluralName, window)
        {
            this.parentEditor = parentEditor;
            forceUpdateIDsWhenOutOfSync = false; // Don't sync ID's are global over all categories.
            canReOrderItems = true;

        }

        public override void EditItem(InventoryCraftingBlueprint item)
        {
            base.EditItem(item);


            requiredItemsList = new UnityEditorInternal.ReorderableList(item.requiredItems, typeof(InventoryItemAmountRow), true, true, true, true);
            requiredItemsList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Required items");
            requiredItemsList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;


                var r2 = rect;
                r2.width /= 2;
                r2.width -= 5;

                if (item.requiredItems[index].amount < 1)
                {
                    item.requiredItems[index].SetAmount(1);
                }

                item.requiredItems[index].SetAmount((uint)EditorGUI.IntField(r2, (int)item.requiredItems[index].amount));

                r2.x += r2.width + 5;

                if (item.requiredItems[index].item == null)
                    GUI.backgroundColor = Color.red;

                if (GUI.Button(r2, (item.requiredItems[index].item != null) ? item.requiredItems[index].item.name : string.Empty, EditorStyles.objectField))
                {
                    var itemPicker = InventoryItemPicker.Get();
                    itemPicker.Show(ItemManager.database);
                    itemPicker.OnPickObject += obj =>
                    {
                        item.requiredItems[index].SetItem(obj);
                        window.Repaint();
                        GUI.changed = true; // To save..
                    };
                }

                GUI.backgroundColor = Color.white;
            };
            requiredItemsList.onAddCallback += list =>
            {
                var l = new List<InventoryItemAmountRow>(item.requiredItems);
                l.Add(new InventoryItemAmountRow());
                item.requiredItems = l.ToArray();
                list.list = item.requiredItems;

                window.Repaint();
            };
            requiredItemsList.onRemoveCallback += list =>
            {
                var l = new List<InventoryItemAmountRow>(item.requiredItems);
                l.RemoveAt(list.index);
                item.requiredItems = l.ToArray();
                list.list = item.requiredItems;

                window.Repaint();
            };



            resultItemsList = new UnityEditorInternal.ReorderableList(item.resultItems, typeof(InventoryItemAmountRow), true, true, true, true);
            resultItemsList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Result items");
            resultItemsList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;



                var r2 = rect;
                r2.width /= 2;
                r2.width -= 5;

                if (item.resultItems[index].amount < 1)
                {
                    item.resultItems[index].SetAmount(1);
                }

                item.resultItems[index].SetAmount((uint)EditorGUI.IntField(r2, (int)item.resultItems[index].amount));

                r2.x += r2.width + 5;

                if (item.resultItems[index].item == null)
                    GUI.backgroundColor = Color.red;

                if (GUI.Button(r2, (item.resultItems[index].item != null) ? item.resultItems[index].item.name : string.Empty, EditorStyles.objectField))
                {
                    var itemPicker = InventoryItemPicker.Get();
                    itemPicker.Show(ItemManager.database);
                    itemPicker.OnPickObject += obj =>
                    {
                        item.resultItems[index].SetItem(obj);
                        window.Repaint();
                        GUI.changed = true; // To save..
                    };
                }

                GUI.backgroundColor = Color.white;
            };
            resultItemsList.onAddCallback += list =>
            {
                var l = new List<InventoryItemAmountRow>(item.resultItems);
                l.Add(new InventoryItemAmountRow());
                item.resultItems = l.ToArray();
                list.list = item.resultItems;

                window.Repaint();
            };
            resultItemsList.onRemoveCallback += list =>
            {
                var l = new List<InventoryItemAmountRow>(item.resultItems);
                l.RemoveAt(list.index);
                item.resultItems = l.ToArray();
                list.list = item.resultItems;

                window.Repaint();
            };


            usageRequirementPropertiesList = new UnityEditorInternal.ReorderableList(item.usageRequirementProperties, typeof(InventoryItemPropertyRequirementLookup), true, true, true, true);
            usageRequirementPropertiesList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Property requirements");
            usageRequirementPropertiesList.elementHeight = 42;
            usageRequirementPropertiesList.drawElementCallback += (rect, index, active, focused) =>
            {

                InventoryEditorUtility.DrawUsageRequirementPropertyElement(rect, item.usageRequirementProperties[index], active, focused, true);
            };
            usageRequirementPropertiesList.onAddCallback += list =>
            {
                var l = new List<InventoryItemPropertyRequirementLookup>(item.usageRequirementProperties);
                l.Add(new InventoryItemPropertyRequirementLookup());
                item.usageRequirementProperties = l.ToArray();
                list.list = item.usageRequirementProperties;

                window.Repaint();
            };
            usageRequirementPropertiesList.onRemoveCallback += list =>
            {
                var l = new List<InventoryItemPropertyRequirementLookup>(item.usageRequirementProperties);
                l.RemoveAt(list.index);
                item.usageRequirementProperties = l.ToArray();
                list.list = item.usageRequirementProperties;

                window.Repaint();
            };
        }

        protected override bool MatchesSearch(InventoryCraftingBlueprint item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search) || item.description.ToLower().Contains(search));
        }

        protected override void CreateNewItem()
        {
            var item = new InventoryCraftingBlueprint();
            int highestID = 0;
            foreach (var cat in ItemManager.database.craftingCategories)
            {
                foreach (var blueprint in cat.blueprints)
                {
                    if (blueprint.ID > highestID)
                        highestID = blueprint.ID;
                }
            }

            item.ID = ++highestID;
            AddItem(item, true);
        }

        public override void DuplicateItem(int index)
        {
            var item = Clone(index);
            item.ID = (crudList.Count > 0) ? crudList.Max(o => o.ID) + 1 : 0;
            AddItem(item);
        }

        protected override void DrawSidebarRow(InventoryCraftingBlueprint item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            if (IsValidBlueprint(item))
            {
                DrawSidebarRowElement(item.name, 260);
            }
            else
            {
                // Invalid
                DrawSidebarRowElement("Invalid", "sv_label_4", 60, 20);
                DrawSidebarRowSpace(10); // Spacing
                DrawSidebarRowElement(item.name, 190);
            }

            EndSidebarRow(item, i);
        }

        protected virtual bool IsValidBlueprint(InventoryCraftingBlueprint blueprint)
        {
            if (blueprint.requiredItems.Any(item => item.item == null))
            {
                return false;
            }

            if (blueprint.resultItems.Any(item => item.item == null) || blueprint.resultItems.Length == 0)
            {
                return false;
            }
            
            return true; // All good
        }

        protected override void DrawDetail(InventoryCraftingBlueprint selectedBlueprint, int index)
        {
            EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;


            #region About craft

            EditorGUILayout.LabelField("Step 1. What are we crafting?", InventoryEditorStyles.titleStyle);



            var itemRow = selectedBlueprint.resultItems.FirstOrDefault();
            string name = "";
            string desc = "";
            string cat = "";
            if (itemRow.item != null)
            {
                name = itemRow.item.name;
                desc = itemRow.item.description;
                cat = itemRow.item.category.name;
            }



            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);
            selectedBlueprint.useItemResultNameAndDescription = EditorGUILayout.Toggle("Use result item's name", selectedBlueprint.useItemResultNameAndDescription);
            if (selectedBlueprint.useItemResultNameAndDescription == false)
            {
                selectedBlueprint.customName = EditorGUILayout.TextField("Blueprint name", selectedBlueprint.customName);
                selectedBlueprint.customDescription = EditorGUILayout.TextField("Blueprint description", selectedBlueprint.customDescription);
                GUI.enabled = false;

                EditorGUILayout.TextField("Category", cat);
            }
            else
            {
                GUI.enabled = false;


                EditorGUILayout.TextField("Blueprint name", name);
                EditorGUILayout.TextField("Blueprint description", desc);
                EditorGUILayout.TextField("Category", cat);
            }
            GUI.enabled = true;


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio clips", InventoryEditorStyles.titleStyle);

            selectedBlueprint.overrideCategoryAudioClips = EditorGUILayout.Toggle("Override category audio clips", selectedBlueprint.overrideCategoryAudioClips);
            if (selectedBlueprint.overrideCategoryAudioClips)
            {
                InventoryEditorUtility.InventoryAudioClip("Success Audio clip", selectedBlueprint.successAudioClip);
                InventoryEditorUtility.InventoryAudioClip("Crafting Audio clip", selectedBlueprint.craftingAudioClip);
                InventoryEditorUtility.InventoryAudioClip("Canceled Audio clip", selectedBlueprint.canceledAudioClip);
                InventoryEditorUtility.InventoryAudioClip("Failed Audio clip", selectedBlueprint.failedAudioClip);
            }
            else
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Using category audio clips", InventoryEditorStyles.labelStyle);

                EditorGUILayout.LabelField("Crafting Audio clip", category.successAudioClip.audioClip != null ? category.successAudioClip.audioClip.name : "(none)");
                EditorGUILayout.LabelField("Success Audio clip", category.craftingAudioClip.audioClip != null ? category.craftingAudioClip.audioClip.name : "(none)");
                EditorGUILayout.LabelField("Canceled Audio clip", category.canceledAudioClip.audioClip != null ? category.canceledAudioClip.audioClip.name : "(none)");
                EditorGUILayout.LabelField("Failed Audio clip", category.failedAudioClip.audioClip != null ? category.failedAudioClip.audioClip.name : "(none)");

                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndVertical();

            #endregion


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            #region Crafting process

            EditorGUILayout.LabelField("Step 2. How are we crafting it?", InventoryEditorStyles.titleStyle);

            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            selectedBlueprint.playerLearnedBlueprint = EditorGUILayout.Toggle("Player learned blueprint", selectedBlueprint.playerLearnedBlueprint);
            selectedBlueprint.successChanceFactor = EditorGUILayout.Slider("Chance factor", selectedBlueprint.successChanceFactor, 0.0f, 1.0f);
            selectedBlueprint.craftingTimeDuration = EditorGUILayout.FloatField("Crafting time duration (seconds)", selectedBlueprint.craftingTimeDuration);
            selectedBlueprint.craftingTimeSpeedupFactor = EditorGUILayout.FloatField("Speedup factor", selectedBlueprint.craftingTimeSpeedupFactor);
            selectedBlueprint.craftingTimeSpeedupMax = EditorGUILayout.FloatField("Max speedup", selectedBlueprint.craftingTimeSpeedupMax);



            if (selectedBlueprint.craftingTimeSpeedupFactor != 1.0f)
            {
                EditorGUILayout.Space();

                for (int i = 1; i < 16; i++)
                {
                    float f = Mathf.Clamp(Mathf.Pow(selectedBlueprint.craftingTimeSpeedupFactor, i * 5), 0.0f, selectedBlueprint.craftingTimeSpeedupMax);

                    if (f != selectedBlueprint.craftingTimeSpeedupMax)
                        EditorGUILayout.LabelField("Speedup after \t" + (i * 5) + " crafts \t" + System.Math.Round(f, 2) + "x \t(" + System.Math.Round(selectedBlueprint.craftingTimeDuration / f, 2) + "s per item)");
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Reached max after " + 1.0f / Mathf.Log(selectedBlueprint.craftingTimeSpeedupFactor, selectedBlueprint.craftingTimeSpeedupMax) + " crafts");
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();



            EditorGUILayout.LabelField("Step 3. What items does the user need? (Ignore if using layouts)", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);


            var db = ItemManager.database;
            if (selectedBlueprint.craftingCost == null)
            {
                if (db.currencies.Length > 0)
                {
                    selectedBlueprint.craftingCost = new InventoryCurrencyLookup(db.currencies.First());
                }
                else
                {
                    EditorGUILayout.Popup("", 0, new[] { "No currencies " });
                }
            }

            if (selectedBlueprint.craftingCost != null)
                InventoryEditorUtility.CurrencyLookup("Crafting cost", selectedBlueprint.craftingCost);


            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(InventoryEditorStyles.reorderableListStyle);
            requiredItemsList.DoLayoutList();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            usageRequirementPropertiesList.DoLayoutList();

            #endregion


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            #region Craft result

            EditorGUILayout.LabelField("Step 4. What's the result?", InventoryEditorStyles.titleStyle);

            EditorGUILayout.BeginVertical(InventoryEditorStyles.reorderableListStyle);
            resultItemsList.DoLayoutList();
            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            #region Layout editor

            EditorGUILayout.LabelField("Step 5. (optional) Define the layouts to use", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            int counter = 0;
            foreach (var l in selectedBlueprint.blueprintLayouts)
            {
                EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);
                EditorGUILayout.BeginHorizontal();

                l.enabled = EditorGUILayout.BeginToggleGroup("Layout #" + l.ID + "-" + (l.enabled ? "(enabled)" : "(disabled)"), l.enabled);
                EditorGUILayout.BeginHorizontal();

                GUI.color = Color.red;
                if (GUILayout.Button("Delete"))
                {
                    var t = new List<InventoryCraftingBlueprintLayout>(selectedBlueprint.blueprintLayouts);
                    t.RemoveAt(counter);
                    selectedBlueprint.blueprintLayouts = t.ToArray();

                    AssetDatabase.SaveAssets();
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                //EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginVertical();
                if (l.enabled)
                {
                    foreach (var r in l.rows)
                    {
                        EditorGUILayout.BeginHorizontal();
                        foreach (var c in r.columns)
                        {
                            if (c.item != null)
                                GUI.color = Color.green;

                            EditorGUILayout.BeginVertical("box", GUILayout.Width(80), GUILayout.Height(80));

                            EditorGUILayout.LabelField((c.item != null) ? c.item.name : string.Empty, InventoryEditorStyles.labelStyle);
                            c.amount = EditorGUILayout.IntField(c.amount);

                            if (GUILayout.Button("Set", GUILayout.Width(80)))
                            {
                                var pick = InventoryItemPicker.Get();
                                pick.Show(ItemManager.database);
                                var clickedItem = c;
                                pick.OnPickObject += item =>
                                {
                                    clickedItem.item = item;
                                    clickedItem.amount = 1;
                                    GUI.changed = true;
                                    window.Repaint();
                                };

                                //layoutObjectPickerSetFor = c;
                                //EditorGUIUtility.ShowObjectPicker<UnityEngine.Object>(null, false, "l:InventoryItemPrefab", 61);
                            }
                            if (GUILayout.Button("Clear", EditorStyles.miniButton))
                            {
                                c.amount = 0;
                                c.item = null;
                            }

                            EditorGUILayout.EndVertical();

                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndToggleGroup();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                counter++;
            }


            if (GUILayout.Button("Add layout"))
            {
                var l = new List<InventoryCraftingBlueprintLayout>(selectedBlueprint.blueprintLayouts);
                var obj = new InventoryCraftingBlueprintLayout();

                obj.ID = l.Count;
                obj.rows = new InventoryCraftingBlueprintLayout.Row[category.rows];
                for (int i = 0; i < obj.rows.Length; i++)
                {
                    obj.rows[i] = new InventoryCraftingBlueprintLayout.Row();
                    obj.rows[i].index = i;
                    obj.rows[i].columns = new InventoryCraftingBlueprintLayout.Row.Column[category.cols];

                    for (int j = 0; j < obj.rows[i].columns.Length; j++)
                    {
                        obj.rows[i].columns[j] = new InventoryCraftingBlueprintLayout.Row.Column();
                        obj.rows[i].columns[j].index = j;
                    }
                }

                l.Add(obj);
                selectedBlueprint.blueprintLayouts = l.ToArray();
            }

            EditorGUILayout.EndVertical();
            #endregion


            GUI.enabled = true; // From layouts
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
