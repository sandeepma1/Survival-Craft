using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devdog.InventorySystem.Editors
{
	public class ItemEditor : InventorySystemEditorCrudBase<InventoryItemBase>
	{
		protected class TypeFilter
		{
			public System.Type type { get; set; }

			public bool enabled { get; set; }

			public TypeFilter (System.Type type, bool enabled)
			{
				this.type = type;
				this.enabled = enabled;
			}
		}


		protected override List<InventoryItemBase> crudList {
			get { return new List<InventoryItemBase> (ItemManager.database.items); }
			set { ItemManager.database.items = value.ToArray (); }
		}

		private string previouslySelectedGUIItemName { get; set; }

		public Editor itemEditorInspector;

		private static InventoryItemBase previousItem;

		private List<TypeFilter> _allItemTypes;

		protected List<TypeFilter> allItemTypes {
			get {
				if (_allItemTypes == null)
					_allItemTypes = GetAllItemTypes ();

				return _allItemTypes;
			}
			set { _allItemTypes = value; }
		}


		//private Editor previewEditor;


		public ItemEditor (string singleName, string pluralName, EditorWindow window) : base (singleName, pluralName, window)
		{
			if (selectedItem != null)
				itemEditorInspector = Editor.CreateEditor (selectedItem);

			window.autoRepaintOnSceneChange = false;
		}

		protected virtual List<TypeFilter> GetAllItemTypes ()
		{
			var types = new List<TypeFilter> (16);
			foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>()) {
				if (script.GetClass () != null && script.GetClass ().IsSubclassOf (typeof(InventoryItemBase)) && script.GetClass ().IsAbstract == false)
					types.Add (new TypeFilter (script.GetClass (), false));
			}

			return types;
		}

		protected override bool MatchesSearch (InventoryItemBase item, string searchQuery)
		{
			string search = searchQuery.ToLower ();
			return (item.name.ToLower ().Contains (search) || item.description.ToLower ().Contains (search) ||
			item.ID.ToString ().Contains (search) || item.GetType ().Name.ToLower ().Contains (search));
		}

		protected override void CreateNewItem ()
		{
			var picker = InventoryCreateNewItemEditor.Get ((System.Type type, GameObject obj, EditorWindow thisWindow) => {
				string prefabPath = EditorPrefs.GetString ("InventorySystem_ItemPrefabPath") + "/item_" + System.DateTime.Now.ToFileTimeUtc () + "_PFB.prefab";
				if (Directory.Exists (EditorPrefs.GetString ("InventorySystem_ItemPrefabPath")) == false) {
					Debug.LogWarning ("The directory you're trying to save to doesn't exist! (" + EditorPrefs.GetString ("InventorySystem_ItemPrefabPath") + ")");
					return;
				}

				//var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
				var instanceObj = UnityEngine.Object.Instantiate<GameObject> (obj); // For unity 5.3+ - Source needs to be instance object.
				var prefab = PrefabUtility.CreatePrefab (prefabPath, instanceObj);
				UnityEngine.Object.DestroyImmediate (instanceObj);

				prefab.layer = InventorySettingsManager.instance.itemWorldLayer;

				AssetDatabase.SetLabels (prefab, new string[] { "InventoryItemPrefab" });

				var comp = (InventoryItemBase)prefab.AddComponent (type);
				comp.ID = (crudList.Count > 0) ? crudList.Max (o => o.ID) + 1 : 0;
				EditorUtility.SetDirty (comp); // To save it.

				if (prefab.GetComponent<ObjectTriggererItem> () == null)
					prefab.AddComponent<ObjectTriggererItem> ();

				if (prefab.GetComponent<SpriteRenderer> () == null) {
					// This is not a 2D object
					if (prefab.GetComponent<Collider> () == null)
						prefab.AddComponent<BoxCollider> ();

					var sphereCollider = prefab.GetComponent<SphereCollider> ();
					if (sphereCollider == null)
						sphereCollider = prefab.AddComponent<SphereCollider> ();

					sphereCollider.isTrigger = true;

					if (prefab.GetComponent<Rigidbody> () == null)
						prefab.AddComponent<Rigidbody> ();
				}



				// Avoid deleting the actual prefab / model, only the cube / internal models without an asset path.
				if (string.IsNullOrEmpty (AssetDatabase.GetAssetPath (obj)))
					Object.DestroyImmediate (obj);

				AddItem (comp, true);
				thisWindow.Close ();
			});
			picker.Show ();
		}

		public override void DuplicateItem (int index)
		{
			var source = crudList [index];

			var item = UnityEngine.Object.Instantiate<InventoryItemBase> (source);
			item.ID = (crudList.Count > 0) ? crudList.Max (o => o.ID) + 1 : 0;
			item.name += "(duplicate)";

			string prefabPath = EditorPrefs.GetString ("InventorySystem_ItemPrefabPath") + "/item_" + System.DateTime.Now.ToFileTimeUtc () + "_PFB.prefab";

			var prefab = PrefabUtility.CreatePrefab (prefabPath, item.gameObject);
			prefab.layer = InventorySettingsManager.instance.itemWorldLayer;

			AssetDatabase.SetLabels (prefab, new string[] { "InventoryItemPrefab" });


			AddItem (prefab.gameObject.GetComponent<InventoryItemBase> ());
			UnityEngine.Object.DestroyImmediate (item.gameObject, false);

			EditorUtility.SetDirty (prefab); // To save it.
			EditorUtility.SetDirty (ItemManager.database); // To save it.
			window.Repaint ();
		}

		public override void AddItem (InventoryItemBase item, bool editOnceAdded = true)
		{
			base.AddItem (item, editOnceAdded);

			UpdateAssetName (item);
		}

		public override void RemoveItem (int i)
		{
			AssetDatabase.DeleteAsset (AssetDatabase.GetAssetPath (ItemManager.database.items [i]));
			base.RemoveItem (i);
		}

		public override void EditItem (InventoryItemBase item)
		{
			base.EditItem (item);

			Undo.ClearUndo (previousItem);

			Undo.RecordObject (item, "INV_PRO_item");

			if (item != null)
				itemEditorInspector = Editor.CreateEditor (item);


			previousItem = item;
		}

		protected override void DrawSidebar ()
		{
			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical ();

			int i = 0;
			foreach (var type in allItemTypes) {
				if (i % 3 == 0)
					GUILayout.BeginHorizontal ();

				type.enabled = GUILayout.Toggle (type.enabled, type.type.Name.Replace ("InventoryItem", ""), "OL Toggle");
                
				if (i % 3 == 2 || i == allItemTypes.Count - 1)
					GUILayout.EndHorizontal ();

				i++;
			}
			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();

			base.DrawSidebar ();

		}

		protected override void DrawSidebarRow (InventoryItemBase item, int i)
		{
			int checkedCount = 0;
			foreach (var type in allItemTypes) {
				if (type.enabled)
					checkedCount++;
			}

			if (checkedCount > 0)
			if (allItemTypes.FirstOrDefault (o => o.type == item.GetType () && o.enabled) == null)
				return;


			BeginSidebarRow (item, i);

			DrawSidebarRowElement ("#" + item.ID.ToString (), 40);
			DrawSidebarRowElement (item.name, 120);
			DrawSidebarRowElement (item.GetType ().Name.Replace ("InventoryItem", ""), 120);
			bool t = DrawSidebarRowElementToggle (true, "", "VisibilityToggle", 20);
			if (t == false) // User clicked view icon
                AssetDatabase.OpenAsset (selectedItem);

			EndSidebarRow (item, i);
		}

		protected override void ClickedSidebarRowElement (InventoryItemBase item)
		{
			base.ClickedSidebarRowElement (item);
		}

		protected override void DrawDetail (InventoryItemBase item, int index)
		{
			EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;


			InventoryEditorUtility.ErrorIfEmpty (EditorPrefs.GetString ("InventorySystem_ItemPrefabPath") == string.Empty, "Inventory item prefab folder is not set, items cannot be saved! Please go to settings and define the Inventory item prefab folder.");
			if (EditorPrefs.GetString ("InventorySystem_ItemPrefabPath") == string.Empty) {
				canCreateItems = false;
				return;
			}
			canCreateItems = true;

			GUILayout.Label ("Use the inspector if you want to add custom components.", InventoryEditorStyles.titleStyle);
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			if (GUILayout.Button ("Convert type")) {
				var typePicker = InventoryItemTypePicker.Get ();
				typePicker.Show (ItemManager.database);
				typePicker.OnPickObject += type => {
					ConvertThisToNewType (item, type);
				};

				return;
			}

			itemEditorInspector.OnInspectorGUI ();

			if (previouslySelectedGUIItemName == "ItemEditor_itemName" && GUI.GetNameOfFocusedControl () != previouslySelectedGUIItemName) {
				UpdateAssetName (item);
			}
            
			previouslySelectedGUIItemName = GUI.GetNameOfFocusedControl ();


			//// Preview box
			//if (previewEditor == null)
			//    previewEditor = Editor.CreateEditor(item);

			////previewEditor.OnInteractivePreviewGUI(new Rect(300, scrollPosition.y, 200, 200), GUIStyle.none);
			//var style = new GUIStyle();
			//style.normal.background = Texture2D.whiteTexture;

			//previewEditor.OnPreviewGUI(new Rect(300, scrollPosition.y, 200, 200), style);


			EditorGUIUtility.labelWidth = 0;
		}

		public static string GetAssetName (InventoryItemBase item)
		{
			return "#" + item.ID + "_" + (string.IsNullOrEmpty (item.name) ? string.Empty : item.name.ToLower ().Replace (" ", "_"));
			//return "Itm_" + (string.IsNullOrEmpty(item.name) ? string.Empty : item.name.ToLower().Replace(" ", "_")) + "_#" + item.ID + "_" + ItemManager.database.name + "_PFB";
		}

		public static void UpdateAssetName (InventoryItemBase item)
		{
			var newName = GetAssetName (item);
			if (AssetDatabase.GetAssetPath (item).EndsWith (newName + ".prefab") == false) {
				AssetDatabase.RenameAsset (AssetDatabase.GetAssetPath (item), newName);
			}
		}

		public void ConvertThisToNewType (InventoryItemBase currentItem, Type type)
		{
			var toFields = new List<FieldInfo> (128);
			InventoryEditorUtility.GetAllFieldsInherited (type, toFields);

			var comp = (InventoryItemBase)currentItem.gameObject.AddComponent (type);

			var currentFields = new List<FieldInfo> (128);
			InventoryEditorUtility.GetAllFieldsInherited (currentItem.GetType (), currentFields);
			foreach (var fieldInfo in currentFields) {
				var toField = toFields.FirstOrDefault (o => o.Name == fieldInfo.Name);
				if (toField != null)
					toField.SetValue (comp, fieldInfo.GetValue (currentItem));
			}

			// Set in database
			for (int i = 0; i < ItemManager.database.items.Length; i++) {
				if (ItemManager.database.items [i].ID == currentItem.ID) {
					ItemManager.database.items [i] = comp;
				}
			}

			selectedItem = comp;
			itemEditorInspector = Editor.CreateEditor (selectedItem);
			EditorUtility.SetDirty (selectedItem.gameObject);
			GUI.changed = true;

			Object.DestroyImmediate (currentItem, true); // Get rid of the old component
			window.Repaint ();
		}

		protected override bool IDsOutOfSync ()
		{
			uint next = 0;
			foreach (var item in crudList) {
				if (item == null || item.ID != next)
					return true;
                
				next++;
			}

			return false;
		}

		protected override void SyncIDs ()
		{
			Debug.Log ("Item ID's out of sync, force updating...");

			crudList = crudList.Where (o => o != null).ToList ();
			uint lastID = 0;
			foreach (var item in crudList) {
				item.ID = lastID++;
				EditorUtility.SetDirty (item);
			}

			GUI.changed = true;
			EditorUtility.SetDirty (ItemManager.database);
		}
	}
}
