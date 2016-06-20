using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/settings-menu/")]
    [RequireComponent(typeof(UIWindowInteractive))]
    public partial class SettingsMenuUI : MonoBehaviour
    {
        /// <summary>
        /// When true, each time the "keyCode" is pressed a window is hidden. When there are no longer any interactive windows visible the settings menu will be shown.
        /// When false, all windows will be hidden.
        /// </summary>
        public bool hideSingleWindowAtATime = true;

        /// <summary>
        /// When the settings menu is closed should the previously hidden windows be restored?
        /// </summary>
        public bool restoreWindowsAfterClose = true;

        [NonSerialized]
        private List<UIWindowInteractive> hiddenWindows = new List<UIWindowInteractive>();

        [NonSerialized]
        private UIWindowInteractive[] interactiveWindowsInScene = new UIWindowInteractive[0];

        private UIWindowInteractive _window;
        public UIWindowInteractive window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindowInteractive>();

                return _window;
            }
            set { _window = value; }
        }


        public void Start()
        {
            interactiveWindowsInScene = Resources.FindObjectsOfTypeAll<UIWindowInteractive>();

//            window.OnShow += HideInteractiveWindows;
//            if(restoreWindowsAfterClose)
//                window.OnHide += RestoreInteractiveWindows;

            window.enabled = false;
        }
        public virtual void Update()
        {
            if (window.keysDown)
            {
                if(window.isVisible)
                    RestoreInteractiveWindows();
                else
                    HideInteractiveWindows();
            }
        }

        public virtual void HideInteractiveWindows()
        {
            // Show menu, hide current interactive hiddenWindows
            foreach (var w in interactiveWindowsInScene)
            {
                if (w == window)
                    continue;

                if (w.isVisible)
                {
                    w.Hide();
                    hiddenWindows.Add(w);

                    if (hideSingleWindowAtATime)
                        return;

                }
            }

            window.Show();
        }

        public virtual void RestoreInteractiveWindows()
        {
            foreach (var w in hiddenWindows)
            {
                w.Show();
            }

            hiddenWindows.Clear();
            window.Hide();
        }
    }
}
