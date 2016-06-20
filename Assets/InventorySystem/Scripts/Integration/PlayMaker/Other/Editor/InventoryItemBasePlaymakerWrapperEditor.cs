#if PLAYMAKER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Editors;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;
using PropertyDrawer = HutongGames.PlayMakerEditor.PropertyDrawer;

namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [PropertyDrawer(typeof(InventoryItemBasePlaymakerWrapper))]
    public class InventoryItemBasePlaymakerWrapperEditor : PropertyDrawer
    {
        public override object OnGUI(GUIContent label, object obj, bool isSceneObject, params object[] attributes)
        {
            var t = (InventoryItemBasePlaymakerWrapper)obj;

            EditorGUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel(label);

            if (t.item == null)
                GUI.color = Color.yellow;

            if (GUILayout.Button(t.item != null ? t.item.name : "(No item selected)", EditorStyles.objectField))
            {
                var picker = EditorWindow.GetWindow<InventoryItemPicker>(true);
                picker.Show(ItemManager.database);

                picker.OnPickObject += (item) =>
                {
                    t.item = item;
                    GUI.changed = true;
                };
            }
            
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();

            return t;
        }
    }
}

#endif