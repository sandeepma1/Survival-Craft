using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using UnityEngine;
using UnityEngine.Assertions;


namespace Devdog.InventorySystem.UI
{
    [RequireComponent(typeof(UIWindow))]
    [AddComponentMenu("InventorySystem/UI Helpers/Window sync")]
    public partial class UIWindowSync : MonoBehaviour
    {
        public UIWindow[] otherWindows = new UIWindow[0];

        public bool showWhenShown = true;
        public bool hideWhenHidden = true;


        public void Awake()
        {
            var window = GetComponent<UIWindow>();
            Assert.IsFalse(otherWindows.Any(o => o == null), "Window on UIWindowSync is null! This can cause problems. Remove the empty fields on the UIWindowSync component to resolve.");


            window.OnHide += () =>
            {
                foreach (var w in otherWindows)
                {
                    if(w.isVisible && hideWhenHidden)
                        w.Hide();
                }
            };

            window.OnShow += () =>
            {
                foreach (var w in otherWindows)
                {
                    if (w.isVisible == false && showWhenShown)
                        w.Show();
                }
            };
        }
    }
}
