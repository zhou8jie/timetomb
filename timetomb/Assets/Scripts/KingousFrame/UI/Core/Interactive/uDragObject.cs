using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Interactive/Drag Object")]
    public class uDragObject : MonoBehaviour, IDragHandler
    {

        public GameObject target;

        private RectTransform _cachedRectTransform;

        RectTransform cachedRectTransform
        {
            get
            {
                if (_cachedRectTransform == null)
                {
                    if (target == null) target = gameObject;
                    _cachedRectTransform = target.GetComponent<RectTransform>();
                } return _cachedRectTransform;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 from = cachedRectTransform.localPosition;
            Vector3 delta = new Vector3(eventData.delta.x, eventData.delta.y, 1);
            Vector3 to = from + delta;
            uSpringPosition.Begin(target, to, delta.magnitude);// = EaseType.ease;
        }
    }
}