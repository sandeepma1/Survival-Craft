using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.Models;
#if PLY_GAME
using Devdog.InventorySystem.Integration.plyGame.Editors;
#endif


namespace Devdog.InventorySystem.Editors
{
    using System.Linq;

    public class InventoryMainEditor : EditorWindow
    {
        private static int toolbarIndex { get; set; }

        public static EmptyEditor itemEditor { get; set; }
        public static EmptyEditor equipEditor { get; set; }
        public static CraftingEmptyEditor craftingEditor { get; set; }
        public static LanguageEditor languageEditor { get; set; }
        public static SettingsEditor settingsEditor { get; set; }

        public static List<IInventorySystemEditorCrud> editors = new List<IInventorySystemEditorCrud>(8);


        private static InventoryMainEditor _window;
//        private string[] _databasesInProject;

        public static InventoryMainEditor window
        {
            get
            {
                if(_window == null)
                    _window = GetWindow<InventoryMainEditor>(false, "Main manager", false);

                return _window;
            }
        }

        protected string[] editorNames
        {
            get
            {
                string[] items = new string[editors.Count];
                for (int i = 0; i < editors.Count; i++)
                {
                    items[i] = editors[i].ToString();
                }

                return items;
            }
        }

        [MenuItem("Tools/Inventory Pro/Main editor", false, -99)] // Always at the top
        public static void ShowWindow()
        {
            _window = GetWindow<InventoryMainEditor>(false, "Main manager", true);
        }

        private void OnEnable()
        {
            minSize = new Vector2(600.0f, 400.0f);
            toolbarIndex = 0;

            //if (ItemManager.database == null)
            //    return;

//            _databasesInProject = AssetDatabase.FindAssets("t:" + typeof(InventoryItemDatabase).Name);

            InventoryProSetupWizard.CheckScene();
            InventoryProSetupWizard.OnIssuesUpdated += UpdateMiniToolbar;

            CreateEditors();
        }

        private void OnDisable()
        {
            InventoryProSetupWizard.OnIssuesUpdated -= UpdateMiniToolbar;
        }

        static internal void UpdateMiniToolbar(List<InventoryProSetupWizard.SetupIssue> issues)
        {
            window.Repaint();
        }

        public static void SelectTab(Type type)
        {
            int i = 0;
            foreach (var editor in editors)
            {
                var ed = editor as EmptyEditor;
                if (ed != null)
                {
                    bool isChildOf = ed.childEditors.Select(o => o.GetType()).Contains(type);
                    if (isChildOf)
                    {
                        toolbarIndex = i;
                        for (int j = 0; j < ed.childEditors.Count; j++)
                        {
                            if (ed.childEditors[j].GetType() == type)
                            {
                                ed.toolbarIndex = j;
                            }
                        }

                        toolbarIndex = i;
                        ed.Focus();
                        window.Repaint();
                        return;
                    }
                }

                if (editor.GetType() == type)
                {
                    toolbarIndex = i;
                    editor.Focus();
                    window.Repaint();
                    return;
                }

                i++;
            }

            Debug.LogWarning("Trying to select tab in main editor, but type isn't in editor.");
        }

        public virtual void CreateEditors()
        {
            editors.Clear();
            itemEditor = new EmptyEditor("Items editor", this);
            itemEditor.requiresDatabase = true;
            itemEditor.childEditors.Add(new ItemEditor("Item", "Items", this));
            itemEditor.childEditors.Add(new ItemCategoryEditor("Item category", "Item categories", this));
            itemEditor.childEditors.Add(new ItemPropertyEditor("Item property", "Item properties", this) { canReOrderItems = true });
            itemEditor.childEditors.Add(new ItemRarityEditor("Item Rarity", "Item rarities", this) { canReOrderItems = true });
            editors.Add(itemEditor);

            var currencyEditor = new CurrencyEditor("Currency", "Currencies", this);
            currencyEditor.requiresDatabase = true;
            currencyEditor.canReOrderItems = true;
            editors.Add(currencyEditor);

            equipEditor = new EmptyEditor("Equipment editor", this);
            equipEditor.requiresDatabase = true;
            equipEditor.childEditors.Add(new EquipEditor("Stats", this));
#if PLY_GAME
            equipEditor.childEditors.Add(new plyStatsEditor("Ply stats", this));
#endif
            equipEditor.childEditors.Add(new EquipTypeEditor("EquippedItem type", "EquippedItem types", this));
            editors.Add(equipEditor);

            craftingEditor = new CraftingEmptyEditor("Crafting editor", this);
            craftingEditor.requiresDatabase = true;
            editors.Add(craftingEditor);

            languageEditor = new LanguageEditor("Language", "Language categories", this);
            editors.Add(languageEditor);

            settingsEditor = new SettingsEditor("Settings", "Settings categories", this);
            editors.Add(settingsEditor);
        }

        protected virtual void DrawToolbar()
        {
            if (ItemManager.instance != null && ItemManager.itemDatabaseLookup.hasSelectedDatabase)
            {
                if (AssetDatabase.GetAssetPath(ItemManager.database) != AssetDatabase.GetAssetPath(ItemManager.instance.sceneItemDatabase))
                {
                    EditorGUILayout.HelpBox("This scene contains a different database than is currently selected.", MessageType.Warning);
                    if (GUILayout.Button("Select scene's database"))
                    {
                        ItemManager.itemDatabaseLookup.SetDatabase(ItemManager.instance.sceneItemDatabase);
                    }
                }
            }


            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.grey;
            if (GUILayout.Button("< DB", InventoryEditorStyles.toolbarStyle, GUILayout.Width(60)))
            {
                ItemManager.itemDatabaseLookup.ManuallySelectDatabase();
                toolbarIndex = 0;
            }
            GUI.color = Color.white;

            int before = toolbarIndex;
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, editorNames, InventoryEditorStyles.toolbarStyle);
            if (before != toolbarIndex)
                editors[toolbarIndex].Focus();
            
            EditorGUILayout.EndHorizontal();
        }

        internal static void DrawMiniToolbar(List<InventoryProSetupWizard.SetupIssue> issues)
        {
            GUILayout.BeginVertical("Toolbar", GUILayout.ExpandWidth(true));
            
            var issueCount = issues.Sum(o => o.ignore == false ? 1 : 0);
            if (issueCount > 0)
                GUI.color = Color.red;
            else
                GUI.color = Color.green;
            
            if (GUILayout.Button(issueCount + " issues found in scene.", "toolbarbutton", GUILayout.Width(300)))
            {
                InventoryProSetupWizard.ShowWindow();
            }

            GUI.color = Color.white;

            if (ItemManager.itemDatabaseLookup.hasSelectedDatabase)
            {
                var style = EditorStyles.centeredGreyMiniLabel;

                var r = new Rect(320, window.position.height - 18, window.position.width - 320, 20);
                GUI.Label(r, "Selected database: " + AssetDatabase.GetAssetPath(ItemManager.database), style);
            }

            GUILayout.EndVertical();
        }


        public void OnGUI()
        {
            DrawToolbar();

//            EditorPrefs.DeleteKey("InventorySystem_ItemPrefabPath");

            InventoryEditorUtility.ErrorIfEmpty(EditorPrefs.GetString("InventorySystem_ItemPrefabPath") == string.Empty, "Inventory item prefab folder is not set, items cannot be saved! Please go to settings and define the Inventory item prefab folder.");
            if (EditorPrefs.GetString("InventorySystem_ItemPrefabPath") == string.Empty)
            {
                GUI.enabled = true;
                toolbarIndex = editors.Count - 1;
                // Draw the editor
                editors[toolbarIndex].Draw();

                if (GUI.changed)
                    EditorUtility.SetDirty(ItemManager.database); // To make sure it gets saved.

                GUI.enabled = false;
                return;
            }

//            if (CheckDatabase() == false && editors[toolbarIndex].requiresDatabase)
//                return;

            if (toolbarIndex < 0 || toolbarIndex >= editors.Count || editors.Count == 0)
            {
                toolbarIndex = 0;
                CreateEditors();
            }

            // Draw the editor
            editors[toolbarIndex].Draw();

            DrawMiniToolbar(InventoryProSetupWizard.setupIssues);

            if (GUI.changed)
                EditorUtility.SetDirty(ItemManager.database); // To make sure it gets saved.
        }
    }
}