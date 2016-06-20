using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using UnityEngine;


namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// Used to visualize the triggering process (pickup, using, etc)
    /// </summary>
    [RequireComponent(typeof(UIWindow))]
    [AddComponentMenu("InventorySystem/UI Helpers/Object trigger FPS UI")]
    public partial class ObjectTriggererFPSUI : MonoBehaviour
    {
        public string textFormat = "{0}";

        public UnityEngine.UI.Image imageIcon;
        public UnityEngine.UI.Text shortcutText;

        public UIWindow window { get; protected set; }

        protected void Awake()
        {
            window = GetComponent<UIWindow>();
        }

        protected void Start()
        {

            InventoryPlayerManager.instance.OnPlayerChanged += InstanceOnOnPlayerChanged;

        }

        private void InstanceOnOnPlayerChanged(InventoryPlayer oldPlayer, InventoryPlayer newPlayer)
        {
            if (oldPlayer != null)
            {
                try
                {
                    oldPlayer.rangeHelper.OnChangedBestTriggerer -= RangeHelperOnOnChangedBestTriggerer;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            newPlayer.rangeHelper.OnChangedBestTriggerer += RangeHelperOnOnChangedBestTriggerer;
        }

        private void RangeHelperOnOnChangedBestTriggerer(ObjectTriggererBase old, ObjectTriggererBase newBest)
        {
            if (newBest != null)
                Repaint(newBest.uiIcon, string.Format(textFormat, newBest.triggerKeyCode.ToString()));
            else
                window.Hide();
        }

        public void Repaint(Sprite sprite, string text)
        {
            if(window.isVisible == false)
            {
                window.Show();
            }

            if (imageIcon != null && imageIcon.sprite != sprite)
                imageIcon.sprite = sprite;

            if (shortcutText != null && shortcutText.text != text)
                shortcutText.text = text;
        }
    }    
}
