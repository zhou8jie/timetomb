using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Interactive/Trigger")]
    public class uTrigger : MonoBehaviour, uIPointHandler, uISelectionHandler
    {
        /// <summary>
        /// Which event will trigger the tween
        /// </summary>
        public Trigger trigger = Trigger.OnClick;

        [System.Serializable]
        public class TriggerEvent : UnityEvent<Trigger> { }

        public TriggerEvent onTriggered;

        private Selectable mSelectable;
        private Toggle mToggle;
        private bool mToggleValue;

        protected virtual void Start()
        {
            if (trigger == Trigger.OnToggle ||
                trigger == Trigger.OnToggleTrue ||
                trigger == Trigger.OnToggleFalse)
            {
                if (mToggle == null) mToggle = GetComponent<Toggle>();
                if (mToggle != null)
                {
                    mToggle.onValueChanged.AddListener(onToggleValueChanged);
                    mToggleValue = mToggle.isOn;
                }
            }

            mSelectable = GetComponent<Selectable>();
        }

        protected virtual void OnEnable()
        {
            TriggerPlay(Trigger.OnActivate);
            TriggerPlay(Trigger.OnActivateTrue);
        }

        protected virtual void OnDisable()
        {
            TriggerPlay(Trigger.OnActivate);
            TriggerPlay(Trigger.OnActivateFalse);
        }

        private void onToggleValueChanged(bool arg0)
        {
            if (mToggleValue != arg0)
            {
                mToggleValue = arg0;
                TriggerPlay(Trigger.OnToggle);
            }
            if (arg0) TriggerPlay(Trigger.OnToggleTrue);
            else TriggerPlay(Trigger.OnToggleFalse);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnHover);
            TriggerPlay(Trigger.OnHoverTrue);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnPress);
            TriggerPlay(Trigger.OnPressTrue);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnClick);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnPress);
            TriggerPlay(Trigger.OnPressFalse);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnHover);
            TriggerPlay(Trigger.OnHoverFalse);
        }

        public void OnSelect(BaseEventData eventData)
        {
            TriggerPlay(Trigger.OnSelect);
            TriggerPlay(Trigger.OnSelectTrue);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            TriggerPlay(Trigger.OnSelect);
            TriggerPlay(Trigger.OnSelectFalse);
        }

        private void TriggerPlay(Trigger trigger)
        {
            if (this.trigger == trigger)
            {
                if (enabled && (mSelectable == null || mSelectable.interactable))
                {
                    //Debug.Log(string.Format("name:{0}, trigger:{1}", name, trigger.ToString()));
                    OnTriggered(trigger);
                    if (onTriggered != null) { onTriggered.Invoke(trigger); }
                }
            }
        }

        protected virtual void OnTriggered(Trigger trigger)
        {

        }
    }
}
