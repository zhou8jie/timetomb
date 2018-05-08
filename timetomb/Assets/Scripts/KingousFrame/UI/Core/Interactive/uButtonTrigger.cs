using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    public abstract class uButtonTrigger : MonoBehaviour, uIPointHandler
    {
        private Selectable mSelectable;

        protected virtual void Awake()
        {
            mSelectable = GetComponent<Selectable>();
        }

        protected bool IsAvailable()
        {
            return enabled && (mSelectable == null || mSelectable.interactable);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {

        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {

        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {

        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {

        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {

        }
    }
}
