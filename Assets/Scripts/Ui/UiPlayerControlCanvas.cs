using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bronz.Ui
{
    public class UiPlayerControlCanvas : MonoBehaviour
    {
        public static Action OnActionButtonPointerDown;
        public static Action OnActionButtonPointerUp;
        public static Action OnMoreButtonClicked;

        [SerializeField] private Button actionButton;
        [SerializeField] private Button moreButton;
        private EventTrigger trigger;

        private void Start()
        {
            trigger = actionButton.GetComponent<EventTrigger>();
            var pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((e) => OnActionButtonPointerDown?.Invoke());
            trigger.triggers.Add(pointerDown);

            var pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((e) => OnActionButtonPointerUp?.Invoke());
            trigger.triggers.Add(pointerUp);

            moreButton.onClick.AddListener(() => OnMoreButtonClicked?.Invoke());
        }
    }
}