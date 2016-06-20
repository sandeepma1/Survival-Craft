using UnityEngine;
using System.Collections;
using System;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEditor;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(InventoryUIItemWrapperBase), true)]
    public partial class InventoryUIItemWrapperEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            if (EditorApplication.isPlaying)
            {
                var t = (InventoryUIItemWrapperBase) target;

                EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

                EditorGUILayout.LabelField("Run-time info", InventoryEditorStyles.titleStyle);

                EditorGUILayout.LabelField("Item: " + ((t.item != null) ? t.item.name : "(empty)"));
                EditorGUILayout.LabelField("Stacksize: " + ((t.item != null) ? t.item.currentStackSize.ToString() : "-"));
                EditorGUILayout.LabelField("Size: " + ((t.item != null) ? t.item.layoutSizeCols + " X " + t.item.layoutSizeRows : "-"));

                EditorGUILayout.EndVertical();
            }

            DrawDefaultInspector();
            
        }
    }
}