using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.UI;
using UnityEngine.UI;


namespace Devdog.InventorySystem.Editors
{
    using Devdog.InventorySystem.Models;

    public class InventoryProSetupWizard : EditorWindow
    {
        internal class SetupIssue
        {
            public string saveName { get; set; }
            private string finalSaveName
            {
                get { return "INV_PRO_" + saveName; }
            }

            public string message { get; set; }
            public MessageType messageType { get; set; }
            public List<IssueAction> actions { get; set; }


            public bool ignore
            {
                get
                {
                    return EditorPrefs.GetBool(finalSaveName, false);
                }
                set
                {
                    EditorPrefs.SetBool(finalSaveName, value);
                }
            }



            public SetupIssue(string saveName, string message, MessageType messageType, System.Action fixAction)
                : this(saveName, message, messageType, new IssueAction("Fix", fixAction))
            {

            }

            public SetupIssue(string saveName, string message, MessageType messageType, params IssueAction[] action)
            {
                this.saveName = saveName;
                this.message = message;
                this.messageType = messageType;

                this.actions = action.ToList();
                if (this.actions == null)
                    this.actions = new List<IssueAction>();
            }
        }

        internal class IssueAction
        {
            public System.Action action;
            public string name;

            public IssueAction(string name, System.Action action)
            {
                this.name = name;
                this.action = action;
            }
        }



        private static List<SetupIssue> _setupIssues;
        internal static List<SetupIssue> setupIssues
        {
            get
            {
                if (_setupIssues == null)
                    _setupIssues = new List<SetupIssue>();

                return _setupIssues;
            }
            set
            {
                _setupIssues = value;
            }
        }

        private Vector2 scrollPos { get; set; }


        internal delegate void IssuesUpdated(List<SetupIssue> issues);
        internal static event IssuesUpdated OnIssuesUpdated;


        [MenuItem("Tools/Inventory Pro/Setup wizard", false, 2)] // Always at bottom
        public static void ShowWindow()
        {
            var window = GetWindow<InventoryProSetupWizard>(true, "Inventory Pro - Setup wizard", true);
            window.minSize = new Vector2(400, 500);
            //window.maxSize = new Vector2(400, 500);

            CheckScene();
            window.Repaint();
        }


        [UnityEditor.Callbacks.DidReloadScripts]
        private static void DidReloadScripts()
        {
            CheckScene();
        }

        public static void CheckScene()
        {
            setupIssues.Clear();

            CheckManagers();
            CheckDatabase();
            CheckPlayers();
            CheckNullComponents();
            CheckInventoryProRequiredFields();
            CheckCollections();
            CheckCharacterSetup();
            CheckTriggerers();
            CheckSettings();

            if (OnIssuesUpdated != null)
                OnIssuesUpdated(setupIssues);
        }

        private static void CheckCharacterSetup()
        {
            var equippableFields = FindObjectsOfType<InventoryEquippableField>();
            foreach (var equippableField in equippableFields)
            {
                if (equippableField.equipTypes.Any(o => o == null))
                {
                    setupIssues.Add(new SetupIssue("null_in_equippable_field", "Equippable field has a null value (empty row)!", MessageType.Error));
                }
            }
        }

        private static void CheckManagers()
        {
            var managers = GameObject.Find("_Managers");
            if (managers == null)
            {
                setupIssues.Add(new SetupIssue("managers_obj", "No managers object found", MessageType.Error, () =>
                {
                    var m = new GameObject("_Managers");
                    m.AddComponent<InventoryManager>(); // Adds the other managers
                }));

                return;
            }

            var inventoryManager = GetOrAddComponent<InventoryManager>(managers, "managers_inventory", "No manager found on _Managers object", MessageType.Error);
            var itemManager = GetOrAddComponent<ItemManager>(managers, "managers_item", "No ItemManager found on _Managers object.", MessageType.Error);
            GetOrAddComponent<InventoryInputManager>(managers, "managers_input", "No InventoryInputManager found on _Managers object", MessageType.Error);
            GetOrAddComponent<InventoryTriggererManager>(managers, "managers_pickup", "No InventoryTriggererManager found on _Managers object", MessageType.Error);
            GetOrAddComponent<InventorySettingsManager>(managers, "managers_settings", "No InventorySettingsManager found on _Managers object", MessageType.Error);



            if (inventoryManager != null && InventoryManager.langDatabaseLookup.hasSelectedDatabase == false)
            {
                setupIssues.Add(new SetupIssue("managers_langdb", "No language database set on the InventoryManager\nYou can create a new database in your project folder.", MessageType.Error, new IssueAction("Select Managers objects", () =>
                {
                    Selection.activeGameObject = managers;
                })));
            }

            if (itemManager != null && ItemManager.itemDatabaseLookup.hasSelectedDatabase == false)
            {
                setupIssues.Add(new SetupIssue("manager_itemdb", "No item database set on the ItemManager\nYou can create a new database in your project folder.", MessageType.Error));
            }
        }

        private static void CheckDatabase()
        {
            if (ItemManager.itemDatabaseLookup.hasSelectedDatabase == false)
            {
                setupIssues.Add(new SetupIssue("no_selected_db", "No itemDatabase is selected in this project.",
                    MessageType.Error,
                    () =>
                    {
                        ItemManager.itemDatabaseLookup.ManuallySelectDatabase();
                    }));
            }

            if (InventoryManager.langDatabaseLookup.hasSelectedDatabase == false)
            {
                setupIssues.Add(new SetupIssue("no_selected_lang_db",
                    "No language database is selected in this project.", MessageType.Error,
                    () =>
                    {
                        InventoryManager.langDatabaseLookup.ManuallySelectDatabase();
                    }));
            }

            if (ItemManager.itemDatabaseLookup.hasSelectedDatabase)
            {
                var db = ItemManager.database;
                if (db != null)
                {
                    if (db.itemRarities.Length == 0)
                    {
                        setupIssues.Add(new SetupIssue("no_rarities_defined_in_db", "You should have atleast 1 rarity in your database.", MessageType.Error));
                    }

                    if (db.itemCategories.Length == 0)
                    {
                        setupIssues.Add(new SetupIssue("no_categories_defined_in_db", "You should have atleast 1 category in your database.", MessageType.Error));
                    }

                    if (db.currencies.Length == 0)
                    {
                        setupIssues.Add(new SetupIssue("no_currencies_defined_in_db", "You should have atleast 1 currency in your database.", MessageType.Error));
                    }

                    var itemsWithEmptyProperty = db.items.Where(o => o.properties.Contains(null) || o.properties.Any(s => s.property == null)).ToArray();
                    if (itemsWithEmptyProperty.Length > 0)
                    {
                        foreach (var item in itemsWithEmptyProperty)
                        {
                            setupIssues.Add(new SetupIssue("item_with_null_property", "Item (" + item.name + ") in database with an empty property! (ItemID #" + item.ID + ")", MessageType.Error));
                        }
                    }
                }
            }
        }

        private static void CheckPlayers()
        {
            var playerSpawners = Resources.FindObjectsOfTypeAll<InventoryPlayerSpawner>();
            var players = Resources.FindObjectsOfTypeAll<InventoryPlayer>();
            if (playerSpawners.Length == 0 && players.Length == 0)
            {
                setupIssues.Add(new SetupIssue("players_none_found", "No players or player spawners found in scene.", MessageType.Warning));
            }

            foreach (var playerSpawner in playerSpawners)
            {
                var p = playerSpawner; // Capture list and all...
                if (p.characterCollection == null && p.dynamicallyFindUIElements == false)
                {
                    setupIssues.Add(new SetupIssue("players_character_collection", "Player has no character collection and dynamically find is disabled.", MessageType.Warning));
                }

                if (p.gameObject.GetComponent(typeof (IInventoryPlayerController)) == null)
                {
                    setupIssues.Add(new SetupIssue("players_iplayer_controller", "Player has no controller that implements IInventoryPlayerController.\nWhen using your own player controller be sure to implement IInventoryPlayerController.", MessageType.Warning, new IssueAction("Select player", () =>
                    {
                        Selection.activeGameObject = p.gameObject;
                    })));
                }
            }

            foreach (var player in players)
            {
                var p = player;
                if (p.rangeHelper == null)
                {
                    setupIssues.Add(new SetupIssue("players_range_helper", "Player has no range helper.", MessageType.Error, new IssueAction("Fix", () =>
                    {
                        p.AddRangeHelper();

                    }), new IssueAction("Select player", () =>
                    {
                        Selection.activeGameObject = p.gameObject;
                    })));
                }
            }
        }

        private static void CheckNullComponents()
        {
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var obj in gameObjects)
            {
                var comps = obj.GetComponents<Component>();
                foreach (var comp in comps)
                {
                    if (comp == null)
                    {
                        var o = obj; // Capture list and all
                        setupIssues.Add(new SetupIssue("missing_comp", "Missing component on object (" + obj.name + ")", MessageType.Warning, new IssueAction("Select object", () =>
                        {
                            Selection.activeGameObject = o;
                        })));
                    }
                }
            }
        }

        private static void CheckInventoryProRequiredFields()
        {
            var comps = Resources.FindObjectsOfTypeAll<Component>();
            foreach (var comp in comps)
            {
                var type = comp.GetType();
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    var attr = field.GetCustomAttributes(typeof (InventoryRequiredAttribute), true);
                    if (attr.Length > 0)
                    {
                        // Has required comp.
                        var value = field.GetValue(comp);
                        if (value as UnityEngine.Object == null)
                        {
                            var c = comp; // Capture list...
                            setupIssues.Add(new SetupIssue("empty_required_field", "Required field " + field.Name + " on " + type.Name + " is empty.", MessageType.Error, new IssueAction("Select object", () =>
                            {
                                Selection.activeGameObject = c.gameObject;
                            })));
                        }
                    }
                }
            }
        }
    
        private static void CheckCollections()
        {
            var cols = FindObjectsOfType<ItemCollectionBase>();
            foreach (var col in cols)
            {
                var c = col; // Capture list
                if (col.manuallyDefineCollection)
                {
                    if (col.gameObject.GetComponentsInChildren<InventoryUIItemWrapperBase>().Length == 0)
                    {
                        setupIssues.Add(new SetupIssue("empty_col_manually_define", "Collection " + c.collectionName + " is manually defined but no wrappers were found in container.", MessageType.Warning, new IssueAction("Select collection", () =>
                        {
                            Selection.activeGameObject = c.gameObject;
                        })));
                    }
                }

                foreach (var filter in col.filters.filters)
                {
                    if (filter.typeValue == null && filter.restrictionType == InventoryItemFilter.RestrictionType.Type)
                    {
                        setupIssues.Add(new SetupIssue("empty_type_only_allow_types", "Collection " + c.collectionName + " contains a faulty type restriction.", MessageType.Error, new IssueAction("Select collection", () =>
                        {
                            Selection.activeGameObject = c.gameObject;
                        })));
                    }
                }

                if (col.container != null && col.ignoreItemLayoutSizes == false &&
                    col.container.GetComponent<DynamicLayoutGroup>() == null)
                {
                    setupIssues.Add(new SetupIssue("collection_no_layout_group", "Collection " + c.collectionName + "'s container has no dynamic layout group attached, but collection is using item layouts.", MessageType.Warning, new IssueAction("Select collection", () =>
                    {
                        Selection.activeGameObject = c.gameObject;
                    }), new IssueAction("Fix", () =>
                    {
                        if (c.container.GetComponent<LayoutGroup>() != null)
                        {
                            if (EditorUtility.DisplayDialog("Whoops",
                                "The collection's container already contains a layout group.\nWould you like to replace the existing layout group?",
                                "Yes!", "no"))
                            {
                                var old = c.container.GetComponent<GridLayoutGroup>();
                                Vector2 cellSize = new Vector2(100, 100);
                                RectOffset padding = new RectOffset();
                                int colCount = 5;
                                Vector2 spacing = Vector2.zero;
                                if (old != null)
                                {
                                    cellSize = old.cellSize;
                                    padding = old.padding;
                                    colCount = old.constraintCount;
                                    spacing = old.spacing;
                                }

                                DestroyImmediate(c.container.GetComponent<LayoutGroup>());
                                var n = c.container.gameObject.AddComponent<DynamicLayoutGroup>();
                                n.cellSize = cellSize;
                                n.padding = padding;
                                n.columnsCount = colCount;
                                n.spacing = spacing;

                                EditorUtility.SetDirty(n);
                            }
                            else
                            {
                                c.container.gameObject.AddComponent<DynamicLayoutGroup>();
                            }
                        }
                    })));
                }
            }

            var inventories = FindObjectsOfType<InventoryUI>();
            foreach (var inventory in inventories)
            {
                var i = inventory; // Capture list

                if (i.canUseFromCollection == false)
                {
                    setupIssues.Add(new SetupIssue("cant_use_from_inventory", "No items can be used from the inventory, are you sure this is what you want?.", MessageType.Info, new IssueAction("Select inventory", () =>
                    {
                        Selection.activeGameObject = i.gameObject;
                    })));
                }
            }

            var itemCols = FindObjectsOfType<GameObject>();
            foreach (var comp in itemCols)
            {
                var container = comp.GetComponent<IInventoryItemContainer>();
                if (container == null)
                    continue;

                foreach (var c in container.items)
                {
                    var compTemp = comp;
                    var containerTemp = container;
                    var cTemp = c;

                    if (cTemp == null)
                    {
                        setupIssues.Add(new SetupIssue("empty_item_in_IInventoryItemContainer", "An item container contains an empty object.", MessageType.Warning, new IssueAction("Select container", () =>
                        {
                            Selection.activeGameObject = compTemp.gameObject;
                        }), new IssueAction("Fix (remove)", () =>
                        {
                            var l = containerTemp.items.ToList();
                            l.Remove(cTemp);

                            containerTemp.items = l.ToArray();
                        })));
                    }
                }
            }
        }


        private static void CheckTriggerers()
        {
            var settings = InventorySettingsManager.instance;
            var triggerers = Resources.FindObjectsOfTypeAll<ObjectTriggererBase>();
            foreach (var triggerer in triggerers)
            {
                var t = triggerer; // Capture list...
                try
                {
                    if (triggerer.triggerMouseClick == false && triggerer.triggerKeyCode == KeyCode.None && settings.itemTriggerOnPlayerCollision == false)
                    {
                        setupIssues.Add(new SetupIssue("triggerer_no_usable_actions", "Triggerer " + t.name + " doesn't respond to clicks nor a key combination.", MessageType.Warning, new IssueAction("Select triggerer", () =>
                        {
                            Selection.activeGameObject = t.gameObject;
                        })));
                    }
                }
                catch (Exception)
                { }
            }

//            var objectTriggerers = Resources.FindObjectsOfTypeAll<ObjectTriggerer>();
//            foreach (var objectTriggerer in objectTriggerers)
//            {
//                var o = objectTriggerer;
//                if (objectTriggerer.handleWindowDirectly && objectTriggerer.window == null && objectTriggerer.gameObject.GetComponent(typeof(IObjectTriggerUser)) == null)
//                {
//                    setupIssues.Add(new SetupIssue("object_triggerer_no_iuser", "Triggerer " + o.name + " handles the window directly but has no window assigned and no IObjectTriggerUser was found.", MessageType.Error, new IssueAction("Select triggerer", () =>
//                    {
//                        Selection.activeGameObject = o.gameObject;
//                    })));
//                }
//            }


            var itemHolders = Resources.FindObjectsOfTypeAll<ObjectTriggererItemHolder>();
            foreach (var itemHolder in itemHolders)
            {
                if (itemHolder.item == null)
                {
                    var o = itemHolder; // Capture list
                    setupIssues.Add(new SetupIssue("object_triggerer_item_holder_empty", "Item holder " + o.name + " contains no item.", MessageType.Error, new IssueAction("Select item holder", () =>
                    {
                        Selection.activeGameObject = o.gameObject;
                    })));
                }
            }
        }

        private static void CheckSettings()
        {
            var settings = InventorySettingsManager.instance;
            var invManager = InventoryManager.instance;
            if (settings == null || invManager == null)
                return;

            if (settings.useContextMenu && invManager.contextMenu == null)
            {
                setupIssues.Add(new SetupIssue("settings_context_menu", "Use context menu is enabled in settings, but no context menu is set in InventoryManager.", MessageType.Error));
            }

            if (settings.showConfirmationDialogWhenDroppingItem && invManager.confirmationDialog == null)
            {
                setupIssues.Add(new SetupIssue("settings_confirmation_dialog", "Use show confirmation dialog is enabled in settings, but no confirmation dialog is set in InventoryManager.", MessageType.Error));
            }

            if (settings.useUnstackDialog && invManager.unstackDialog == null)
            {
                setupIssues.Add(new SetupIssue("settings_unstack_dialog", "Use unstack dialog is enabled in settings, but no usntack dialog is set in InventoryManager.", MessageType.Error));
            }

            if (settings.useObjectDistance <= 0f)
            {
                setupIssues.Add(new SetupIssue("settinsg_use_distance", "The use distance in your settings is to lower, items cannot be used.", MessageType.Warning));
            }

            if (settings.layersWhenDropping.value == 0)
            {
                setupIssues.Add(new SetupIssue("settings_layers_when_dropping", "Layers when dropping is set to Nothing, items cannot be dropped.", MessageType.Warning));
            }

            if (settings.disabledWhileDialogActive.Any(o => o == null))
            {
                setupIssues.Add(new SetupIssue("settings_disable_while_dialog_active", "Disabled while dialog active contains an null referece (empty reference).", MessageType.Error, () =>
                {
                    settings.disabledWhileDialogActive = settings.disabledWhileDialogActive.Where(o => o != null).ToArray();
                    EditorUtility.SetDirty(settings);
                }));
            }
            
            
        }




        private static T GetOrAddComponent<T>(GameObject obj, string saveName, string message, MessageType error) where T : Component
        {
            var comp = obj.GetComponent<T>();
            if (comp == null)
            {
                setupIssues.Add(new SetupIssue(saveName, message, error, () =>
                {
                    obj.AddComponent<T>();
                }));
            }

            return comp;
        }


        public void OnGUI()
        {
            //CheckScene();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal("Toolbar");

            if (setupIssues.Sum(o => o.ignore ? 1 : 0) > 0)
            {
                if (GUILayout.Button("Clear ignore list", "toolbarbutton"))
                {
                    var i = setupIssues.FindAll(o => o.ignore);
                    foreach (var issue in i)
                    {
                        issue.ignore = false;
                    }

                    CheckScene();
                }
            }

            GUI.color = Color.green;
            if (GUILayout.Button("Force rescan", "toolbarbutton"))
            {
                CheckScene();
                Repaint();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();


            if (setupIssues.Sum(o => o.ignore ? 0 : 1) == 0)
            {
                EditorGUILayout.HelpBox("No problems found...", MessageType.Info);
            }


            bool shouldUpdate = false;
            foreach (var issue in setupIssues)
            {
                if (issue.ignore)
                    continue;

                EditorGUILayout.HelpBox(issue.message, issue.messageType);


                GUILayout.BeginHorizontal("Toolbar");
                foreach (var action in issue.actions)
                {
                    if (action.name == "Fix")
                        GUI.color = Color.green;

                    if (GUILayout.Button(action.name, "toolbarbutton"))
                    {
                        action.action();
                        shouldUpdate = true;
                    }

                    GUI.color = Color.white;
                }


                GUI.color = Color.yellow;
                if (GUILayout.Button("Ignore", "toolbarbutton"))
                {
                    issue.ignore = true;
                    shouldUpdate = true;
                }
                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }

            // To avoid editing the list while itterating.
            if (shouldUpdate)
            {
                CheckScene();
                Repaint();
            }

            GUILayout.EndScrollView();
        }
    }
}