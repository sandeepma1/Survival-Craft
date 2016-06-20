using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using UnityEditor;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(UIShowStat), true)]
    public class UIShowStatEditor : Editor
    {
        private SerializedProperty property;
        private SerializedProperty useCurrentPlayer;
        private SerializedProperty player;

        private SerializedProperty visualizer;

        private SerializedProperty useValueInterpolation;
        private SerializedProperty interpolationCurve;
        private SerializedProperty interpolationSpeed;

        public virtual void OnEnable()
        {
            property = serializedObject.FindProperty("property");
            useCurrentPlayer = serializedObject.FindProperty("useCurrentPlayer");
            player = serializedObject.FindProperty("player");

            visualizer = serializedObject.FindProperty("visualizer");

            useValueInterpolation = serializedObject.FindProperty("useValueInterpolation");
            interpolationCurve = serializedObject.FindProperty("interpolationCurve");
            interpolationSpeed = serializedObject.FindProperty("interpolationSpeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(property);
            EditorGUILayout.PropertyField(useCurrentPlayer);
            if (useCurrentPlayer.boolValue == false)
            {
                EditorGUILayout.PropertyField(player);
            }

            EditorGUILayout.PropertyField(useValueInterpolation);
            EditorGUILayout.PropertyField(interpolationCurve);
            EditorGUILayout.PropertyField(interpolationSpeed);

            EditorGUILayout.PropertyField(visualizer, true);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}