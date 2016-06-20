using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem.UI
{
    public class InventoryItemStandardWrapperDragHandler : InventoryItemWrapperDragHandlerBase
    {
        private RectTransform _currentlyDraggingRectTransform;

        public InventoryItemStandardWrapperDragHandler(int priority)
            : base(priority)
        {
            dragLookup = new InventoryUIDragLookup();
        }


        public override bool CanUse(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            return eventData.button == PointerEventData.InputButton.Left;
        }

        public override InventoryUIDragLookup OnBeginDrag(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            currentlyDragging = wrapper;
            _currentlyDraggingRectTransform = wrapper.gameObject.GetComponent<RectTransform>();
            dragLookup.Reset();

            dragLookup.startIndex = (int) wrapper.index;
            dragLookup.startItemCollection = wrapper.itemCollection;

            return dragLookup;
        }

        public override InventoryUIDragLookup OnDrag(PointerEventData eventData)
        {
            if (currentlyDragging == null)
                return dragLookup;

            if (InventorySettingsManager.instance.guiRoot.renderMode == RenderMode.ScreenSpaceCamera)
            {
                var p = eventData.position;
#if UNITY_EDITOR
                // ??
#else
                p.y -= Screen.height; // TODO: Remove -- Why unity?? Why??
#endif
                _currentlyDraggingRectTransform.anchoredPosition = p;
            }
            else
            {
                currentlyDragging.transform.position = eventData.position;
            }

            return dragLookup;
        }

        public override InventoryUIDragLookup OnEndDrag(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            if (currentlyDragging == null)
                return dragLookup;

            if (wrapper != null)
            {
                dragLookup.endIndex = (int)wrapper.index;
                dragLookup.endItemCollection = wrapper.itemCollection;
            }

            UnityEngine.Object.Destroy(currentlyDragging.gameObject); // No longer need it, destroy 
            currentlyDragging = null;

            return dragLookup;
        }
    }
}