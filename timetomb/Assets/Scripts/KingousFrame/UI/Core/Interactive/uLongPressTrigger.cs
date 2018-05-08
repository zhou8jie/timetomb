using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Interactive/LongPressTrigger")]
    public class uLongPressTrigger : MonoBehaviour, uIPointHandler
    {
        public float interval = 0.5f;
        private bool isPressDown = false;
        private float pressDownTime;

        [SerializeField]
        UnityEvent m_OnLongpress = new UnityEvent();

        void Update()
        {
            if (isPressDown)
            {
                if (Time.time - pressDownTime > interval)
                {
                    m_OnLongpress.Invoke();
                    isPressDown = false;
                }
            }

        }


        public void OnPointerClick(PointerEventData eventData)
        {
            isPressDown = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pressDownTime = Time.time;
            isPressDown = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
           
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPressDown = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressDown = false;
        }
    }
}
