using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
	[CustomEditor (typeof(InventoryItemBase), true)]
	public class InventoryItemBaseEditor : InventoryEditorBase
	{
		//private InventoryItemBase item;

		protected SerializedProperty id;
		protected SerializedProperty itemName;
		// Name is used by Editor.name...
		protected SerializedProperty description;
		protected SerializedProperty properties;
		protected SerializedProperty usageRequirementProperties;
		protected SerializedProperty useCategoryCooldown;
		protected SerializedProperty overrideDropObjectPrefab;
		protected SerializedProperty category;
		protected SerializedProperty icon;

		protected SerializedProperty weight;
		protected SerializedProperty layoutSizeCols;
		protected SerializedProperty layoutSizeRows;
		protected SerializedProperty requiredLevel;
		protected SerializedProperty rarity;
		protected SerializedProperty buyPrice;
		protected SerializedProperty sellPrice;
		protected SerializedProperty isDroppable;
		protected SerializedProperty isHandMined;
		protected SerializedProperty itemQuality;
		protected SerializedProperty isSellable;
		protected SerializedProperty isStorable;
		protected SerializedProperty maxStackSize;
		protected SerializedProperty cooldownTime;


		private UnityEditorInternal.ReorderableList propertiesList { get; set; }

		private UnityEditorInternal.ReorderableList usageRequirementPropertiesList { get; set; }

		private UnityEditorInternal.ReorderableList costPropertiesList { get; set; }



		public override void OnEnable ()
		{
			base.OnEnable ();

			id = serializedObject.FindProperty ("_id");
			itemName = serializedObject.FindProperty ("_name");
			description = serializedObject.FindProperty ("_description");
			properties = serializedObject.FindProperty ("_properties");
			usageRequirementProperties = serializedObject.FindProperty ("_usageRequirementProperties");
			useCategoryCooldown = serializedObject.FindProperty ("_useCategoryCooldown");
			overrideDropObjectPrefab = serializedObject.FindProperty ("_overrideDropObjectPrefab");
			category = serializedObject.FindProperty ("_category");
			icon = serializedObject.FindProperty ("_icon");
			weight = serializedObject.FindProperty ("_weight");
			layoutSizeCols = serializedObject.FindProperty ("_layoutSizeCols");
			layoutSizeRows = serializedObject.FindProperty ("_layoutSizeRows");
			requiredLevel = serializedObject.FindProperty ("_requiredLevel");
			rarity = serializedObject.FindProperty ("_rarity");
			buyPrice = serializedObject.FindProperty ("_buyPrice");
			sellPrice = serializedObject.FindProperty ("_sellPrice");
			isDroppable = serializedObject.FindProperty ("_isDroppable");
			itemQuality = serializedObject.FindProperty ("_itemQuality");
			isHandMined = serializedObject.FindProperty ("_isHandMined");
			isSellable = serializedObject.FindProperty ("_isSellable");
			isStorable = serializedObject.FindProperty ("_isStorable");
			maxStackSize = serializedObject.FindProperty ("_maxStackSize");
			cooldownTime = serializedObject.FindProperty ("_cooldownTime");


			var t = (InventoryItemBase)target;

			propertiesList = new UnityEditorInternal.ReorderableList (serializedObject, properties, true, true, true, true);
			propertiesList.drawHeaderCallback += rect => GUI.Label (rect, "Item properties");
			propertiesList.elementHeight = 40;
			propertiesList.drawElementCallback += (rect, index, active, focused) => {
				DrawPropertyElement (rect, properties.GetArrayElementAtIndex (index), active, focused, true, true);
			};
			propertiesList.onAddCallback += (list) => {
				var l = new List<InventoryItemPropertyLookup> (t.properties);
				l.Add (new InventoryItemPropertyLookup ());
				t.properties = l.ToArray ();

				GUI.changed = true; // To save..
				EditorUtility.SetDirty (target);
				serializedObject.ApplyModifiedProperties ();
				Repaint ();
			};

			usageRequirementPropertiesList = new UnityEditorInternal.ReorderableList (serializedObject, usageRequirementProperties, true, true, true, true);
			usageRequirementPropertiesList.drawHeaderCallback += rect => GUI.Label (rect, "Usage requirement properties");
			usageRequirementPropertiesList.elementHeight = 40;
			usageRequirementPropertiesList.drawElementCallback += (rect, index, active, focused) => {
				DrawUsageRequirementPropertyElement (rect, usageRequirementProperties.GetArrayElementAtIndex (index), active, focused, true);
			};
			usageRequirementPropertiesList.onAddCallback += (list) => {
				var l = new List<InventoryItemPropertyRequirementLookup> (t.usageRequirementProperties);
				l.Add (new InventoryItemPropertyRequirementLookup ());
				t.usageRequirementProperties = l.ToArray ();

				GUI.changed = true; // To save..
				EditorUtility.SetDirty (target);
				serializedObject.ApplyModifiedProperties ();
				Repaint ();
			};
		}

		protected virtual void DrawPropertyElement (Rect rect, SerializedProperty property, bool isActive, bool isFocused, bool drawRestore, bool drawPercentage)
		{
			InventoryEditorUtility.DrawPropertyElement (rect, property, isActive, isFocused, drawRestore, drawPercentage);
		}

		protected virtual void DrawUsageRequirementPropertyElement (Rect rect, SerializedProperty property, bool isActive, bool isFocused, bool drawFilterType)
		{
			InventoryEditorUtility.DrawUsageRequirementPropertyElement (rect, property, isActive, isFocused, drawFilterType);
		}

		private IEnumerator DestroyImmediateThis (InventoryItemBase obj)
		{
			yield return null;
			DestroyImmediate (obj.gameObject, false); // Destroy this object
		}


		protected override void OnCustomInspectorGUI (params CustomOverrideProperty[] extraOverride)
		{
			base.OnCustomInspectorGUI (extraOverride);

			serializedObject.Update ();
			overrides = extraOverride;

			var db = ItemManager.database;


			// Can't go below 0
			if (cooldownTime.floatValue < 0.0f)
				cooldownTime.floatValue = 0.0f;

			GUI.color = Color.yellow;
			var obj = (InventoryItemBase)target;
			if (obj.gameObject.activeInHierarchy && db.itemRarities.Length > obj._rarity && (db.itemRarities.FirstOrDefault (o => o.ID == obj._rarity) != null && db.itemRarities.First (o => o.ID == obj._rarity).dropObject != null)) {
				if (GUILayout.Button ("Convert to drop object")) {
					var dropObj = db.itemRarities.First (o => o.ID == obj._rarity).dropObject;
					var dropInstance = (GameObject)PrefabUtility.InstantiatePrefab (dropObj.gameObject);
					var holder = dropInstance.AddComponent<ObjectTriggererItemHolder> ();
					dropInstance.AddComponent<ObjectTriggererItem> (); // For item pickup

					string path = AssetDatabase.GetAssetPath (db.items [obj.ID]);
					var asset = (GameObject)AssetDatabase.LoadAssetAtPath (path, typeof(GameObject));
					holder.item = asset.GetComponent<InventoryItemBase> ();

					dropInstance.transform.SetParent (obj.transform.parent);
					dropInstance.transform.SetSiblingIndex (obj.transform.GetSiblingIndex ());
					dropInstance.transform.position = obj.transform.position;
					dropInstance.transform.rotation = obj.transform.rotation;

					Selection.activeGameObject = holder.gameObject;

					obj.StartCoroutine (DestroyImmediateThis (obj));
					return;
				}
			}
			GUI.color = Color.white;


//
//            if (db.items.Any(o => AssetDatabase.GetAssetPath(o) == AssetDatabase.GetAssetPath(target)) == false)
//            {
//                EditorGUILayout.HelpBox("This item is not in the currently selected database!\nEditing disabled.", MessageType.Error);
//                return;
//            }


			if (target == null)
				return;

			var t = (InventoryItemBase)target;

			var excludeList = new List<string> () {
				"m_Script",
				"_id",
				"_name",
				"_description",
				"_properties",
				"_usageRequirementProperties",
				"_category",
				"_useCategoryCooldown",
				"_overrideDropObjectPrefab",
				"_icon",
				"_weight",
				"_layoutSizeCols",
				"_layoutSizeRows",
				"_requiredLevel",
				"_rarity",
				"_buyPrice",
				"_sellPrice",
				"_isDroppable",
				"_isSellable",
				"_isStorable",
				"_maxStackSize",
				"_cooldownTime"
			};

			GUILayout.Label ("Default", InventoryEditorStyles.titleStyle);
			EditorGUILayout.BeginVertical (InventoryEditorStyles.boxStyle);
			if (FindOverride ("_id") != null)
				GUI.enabled = false;

			EditorGUILayout.LabelField ("ID: ", id.intValue.ToString ());
			GUI.enabled = true;

			if (FindOverride ("_name") != null)
				GUI.enabled = false;

			GUI.SetNextControlName ("ItemEditor_itemName");
			EditorGUILayout.PropertyField (itemName);

			GUI.enabled = true;

			if (FindOverride ("_description") != null)
				GUI.enabled = false;

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Description", GUILayout.Width (EditorGUIUtility.labelWidth - 5));
			EditorGUILayout.BeginVertical ();
			EditorGUILayout.HelpBox ("Note, that you can use rich text like <b>asd</b> to write bold text and <i>Potato</i> to write italic text.", MessageType.Info);
			description.stringValue = EditorGUILayout.TextArea (description.stringValue, InventoryEditorStyles.richTextArea);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();


			GUI.enabled = true;

			EditorGUILayout.PropertyField (icon);

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Item layout");

			EditorGUILayout.BeginVertical ();
			for (int i = 1; i < 7; i++) {
				EditorGUILayout.BeginHorizontal ();
				for (int j = 1; j < 7; j++) {
					if (layoutSizeCols.intValue < j || layoutSizeRows.intValue < i)
						GUI.color = Color.gray;

					var c = new GUIStyle ("CN Box");
					c.alignment = TextAnchor.MiddleCenter;
					if (GUILayout.Button (j + " X " + i, c, GUILayout.Width (40), GUILayout.Height (40))) {
						layoutSizeCols.intValue = j;
						layoutSizeRows.intValue = i;
					}

					GUI.color = Color.white;
				}
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			if (t.icon != null) {
				var a = t.icon.bounds.size.x / t.icon.bounds.size.y;
				var b = (float)t.layoutSizeCols / t.layoutSizeRows;

				if (Mathf.Approximately (a, b) == false) {
					EditorGUILayout.HelpBox ("Layout size is different from icon aspect ratio.", MessageType.Warning);
				}
			}

			EditorGUILayout.EndVertical ();


			// Draws remaining items
			GUILayout.Label ("Item specific", InventoryEditorStyles.titleStyle);
			EditorGUILayout.BeginVertical (InventoryEditorStyles.boxStyle);

			foreach (var item in extraOverride) {
				if (item.action != null)
					item.action ();

				excludeList.Add (item.serializedName);
			}

			DrawPropertiesExcluding (serializedObject, excludeList.ToArray ());
			EditorGUILayout.EndVertical ();

			#region Properties

			GUILayout.Label ("Item properties", InventoryEditorStyles.titleStyle);
			GUILayout.Label ("You can create properties in the Item editor / Item property editor");
            
			EditorGUILayout.BeginVertical (InventoryEditorStyles.reorderableListStyle);
			propertiesList.DoLayoutList ();
			EditorGUILayout.EndVertical ();


			GUILayout.Label ("Usage requirement properties", InventoryEditorStyles.titleStyle);
			GUILayout.Label ("Add properties the user is required to have in order to use this item.");
			GUILayout.Label ("Example: Usage requirement of 10 strength means:");
			GUILayout.Label ("The user can only use this item if he/she has 10 or more health.");

			EditorGUILayout.BeginVertical (InventoryEditorStyles.reorderableListStyle);
			usageRequirementPropertiesList.DoLayoutList ();
			EditorGUILayout.EndVertical ();
            
			#endregion

			int rarityIndex = InventoryUtility.FindIndex (db.itemRarities, o => o.ID == rarity.intValue);

            
			GUILayout.Label ("Behavior", InventoryEditorStyles.titleStyle);
			EditorGUILayout.BeginVertical (InventoryEditorStyles.boxStyle);

			GUILayout.Label ("Details", InventoryEditorStyles.titleStyle);
			if (db.itemRarities.Length >= rarityIndex && rarityIndex > 0) {
				var color = db.itemRaritiesColors [rarityIndex];
				color.a = 1.0f; // Ignore alpha in the editor.
				GUI.color = color;
			}

			var rar = InventoryEditorUtility.PopupField ("Rarity", db.itemRarityStrings, db.itemRarities, o => o.ID == rarity.intValue);
			if (rar != null)
				rarity.intValue = (int)rar.ID;
            
			GUI.color = Color.white;

			var cat = InventoryEditorUtility.PopupField ("Category", db.itemCategoriesStrings, db.itemCategories, o => o.ID == category.intValue);
			if (cat != null)
				category.intValue = (int)cat.ID;
            
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PropertyField (useCategoryCooldown);
			if (useCategoryCooldown.boolValue) {
				if (cat != null) {
					EditorGUILayout.LabelField (string.Format ("({0} seconds)", db.itemCategories.First (o => o.ID == category.intValue).cooldownTime));
				}
			}

			EditorGUILayout.EndHorizontal ();
			if (useCategoryCooldown.boolValue == false)
				EditorGUILayout.PropertyField (cooldownTime);


			GameObject dropPrefab = null;
			if (overrideDropObjectPrefab.objectReferenceValue != null) {
				EditorGUILayout.HelpBox ("Overriding drop object to: " + overrideDropObjectPrefab.objectReferenceValue.name, MessageType.Info);
				dropPrefab = t.overrideDropObjectPrefab;
			} else if (t.rarity != null && t.rarity.dropObject != null) {
				EditorGUILayout.HelpBox ("Using rarity drop object: " + t.rarity.dropObject.name, MessageType.Info);
				dropPrefab = t.rarity.dropObject;
			} else {
				EditorGUILayout.HelpBox ("No drop object set.", MessageType.Info);
				dropPrefab = t.gameObject;
			}
			if (dropPrefab.GetComponentsInChildren<Collider> (true).Any (o => o.isTrigger) == false) {
				EditorGUILayout.HelpBox ("Drop object has no triggers and therefore can never be picked up!", MessageType.Error);
			}

			EditorGUILayout.PropertyField (overrideDropObjectPrefab);



			GUILayout.Label ("Buying & Selling", InventoryEditorStyles.titleStyle);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Buy price", GUILayout.Width (InventoryEditorStyles.labelWidth));
			EditorGUILayout.PropertyField (buyPrice);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Sell price", GUILayout.Width (InventoryEditorStyles.labelWidth));
			EditorGUILayout.PropertyField (sellPrice);
			EditorGUILayout.EndHorizontal ();


			GUILayout.Label ("Restrictions", InventoryEditorStyles.titleStyle);
			EditorGUILayout.PropertyField (isDroppable);
			EditorGUILayout.PropertyField (isSellable);
			EditorGUILayout.PropertyField (isStorable);
			EditorGUILayout.PropertyField (maxStackSize);
			EditorGUILayout.PropertyField (weight);
			EditorGUILayout.PropertyField (requiredLevel);


			//GUILayout.Label("Audio & Visuals", InventoryEditorStyles.titleStyle);
			//EditorGUILayout.PropertyField(icon);
            

			//EditorGUILayout.EndVertical();

			EditorGUILayout.EndVertical ();


			serializedObject.ApplyModifiedProperties ();
		}
	}
}