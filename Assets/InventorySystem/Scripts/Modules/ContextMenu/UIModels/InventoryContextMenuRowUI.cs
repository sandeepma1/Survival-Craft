using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// An item in the context menu (visual item)
    /// </summary>
    public partial class InventoryContextMenuRowUI : MonoBehaviour, IPointerClickHandler, IPoolableObject
    {
        public UnityEngine.UI.Button button;
        public Text text;

        [HideInInspector]
        public InventoryItemBase item;

        public InventoryAudioClip onUse;

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (onUse == null)
                return;

            button.onClick.AddListener(() =>
            {
                InventoryAudioManager.AudioPlayOneShot(onUse);
            });
        }

        public void Reset()
        {
            //item = null; // No need to reset
            button.onClick.RemoveAllListeners();
        }
    }
}