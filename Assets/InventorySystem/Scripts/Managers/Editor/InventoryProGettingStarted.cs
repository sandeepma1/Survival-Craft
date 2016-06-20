using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Callbacks;


namespace Devdog.InventorySystem.Editors
{
    using Devdog.InventorySystem.Models;

    [InitializeOnLoad]
    public class InventoryProGettingStarted : EditorWindow
    {
        private static InventoryProGettingStarted _window;
        public static InventoryProGettingStarted window
        {
            get
            {
                if (_window == null)
                {
                    _window = GetWindow<InventoryProGettingStarted>(true, "Inventory Pro - Getting started", true);
                    _window.minSize = new Vector2(400, 500);
                    _window.maxSize = _window.minSize;
                }

                return _window;
            }
        }

        private static Texture _documentationIcon;
        private static Texture _videoTutorialsIcon;
        private static Texture _reviewIcon;
        private static Texture _forumIcon;
        private static Texture _integrationsIcon;

        private static bool _showOnStart = true;
        private static int _heightExtra;

        private static string editorPrefsKey
        {
            get { return "SHOW_INV_PRO_GETTING_STARTED_WINDOW" + InventorySettingsManager.version; }
        }

        [MenuItem("Tools/Inventory Pro/Getting started", false, 1)] // Always at bottom
        public static void ShowWindow()
        {
            _showOnStart = EditorPrefs.GetBool(editorPrefsKey, true);
            GetImages();
            window.Show();
        }

        static InventoryProGettingStarted()
        {
            EditorApplication.update += Update;
            GetImages();
        }

        private static void GetImages()
        {
            _documentationIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/InventorySystem/EditorStyles/INV_PRO_documentation.png");
            _videoTutorialsIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/InventorySystem/EditorStyles/INV_PRO_youtube.png");
            _reviewIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/InventorySystem/EditorStyles/INV_PRO_star.png");
            _forumIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/InventorySystem/EditorStyles/INV_PRO_forum.png");
            _integrationsIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/InventorySystem/EditorStyles/INV_PRO_integration.png");
        }

        private static void Update()
        {
            ShowWindowIfRequired();
            EditorApplication.update -= Update;
        }

        public static void ShowWindowIfRequired()
        {
            bool show = EditorPrefs.GetBool(editorPrefsKey, true);

            if (show)
            {
                ShowWindow();
            }
        }
        
        public void OnGUI()
        {
            _heightExtra = 0;

            GUILayout.BeginHorizontal("Toolbar");
            GUILayout.Label("Inventory Pro Version: " + InventorySettingsManager.version);
            GUILayout.EndVertical();

            DrawBox(0, 0, "Documentation", "The official documentation has a detailed description of all components and code examples.", _documentationIcon, () =>
            {
                Application.OpenURL("http://devdog.nl/document/inventory-pro/");
            });

            DrawBox(1, 0, "Video tutorials", "The video tutorials cover all interfaces and a complete set up.", _videoTutorialsIcon, () =>
            {
                Application.OpenURL("https://www.youtube.com/watch?v=kWeXmVIgqO4&list=PL_HIoK0xBTK4R3vX9eIT1QUl-fn78eyIM");
            });

            DrawBox(2, 0, "Forums", "Check out the Inventory Pro forums for some community power.", _forumIcon, () =>
            {
                Application.OpenURL("http://forum.devdog.nl");
            });

            DrawBox(3, 0, "Integrations", "Combine the power of assets and enable integrations.", _integrationsIcon, () =>
            {
                IntegrationHelperEditor.ShowWindow();
            });

            DrawBox(4, 0, "Rate / Review", "Like Inventory Pro? Share the experience :)", _reviewIcon, () =>
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/en/content/31226");
            });

            var toggle = GUI.Toggle(new Rect(10, window.minSize.y - 20, window.minSize.x - 10, 20), _showOnStart, "Show Inventory Pro getting started on start.");
            if (toggle != _showOnStart)
            {
                _showOnStart = toggle;
                EditorPrefs.SetBool(editorPrefsKey, toggle);
            }
        }

        private void DrawBox(int index, int extraHeight, string title, string desc, Texture texture, Action action)
        {
            _heightExtra += extraHeight;

            const int spacing = 10;
            const int offset = 30;
            int offsetY = offset + (spacing * index) + (64 * index);

            GUI.BeginGroup(new Rect(10, offsetY, window.minSize.x - 20, 64 + _heightExtra), InventoryEditorStyles.boxStyle);

            var rect = new Rect(0, 0, window.minSize.x - 20, 64 + _heightExtra);
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                action();
            }

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            var iconRect = new Rect(15, 7, 50, 50);
            GUI.DrawTexture(iconRect, texture);
            
            rect.x = 74;
            rect.y += 5;
            rect.width = window.minSize.x - 90;

            GUI.Label(rect, title, InventoryEditorStyles.titleStyle);

            rect.y += 20;
            rect.height = 44;
            GUI.Label(rect, desc, InventoryEditorStyles.labelStyle);
            
            GUI.EndGroup();
        }

        private bool Button(GUIContent content)
        {
            return GUILayout.Button(content, GUILayout.Width(128), GUILayout.Height(128));
        }
    }
}