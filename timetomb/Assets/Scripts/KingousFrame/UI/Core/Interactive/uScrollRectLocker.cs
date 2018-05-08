using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Interactive/ScrollRect Locker")]
    public class uScrollRectLocker : UIBehaviour, ICanvasRaycastFilter
    {
        [SerializeField]
        private Vector2 _normalizedPosition;
        [SerializeField]
        private float _speed = 4f;

        private ScrollRect _scrollRect;

        protected override void Awake()
        {
            base.Awake();

            _scrollRect = GetComponent<ScrollRect>();
        }

        void Update()
        {
            if (Vector2.Distance(_normalizedPosition, _scrollRect.normalizedPosition) <= 0.001f) return;
            if (_speed > 0)
            {
                _scrollRect.normalizedPosition = Vector2.Lerp(_scrollRect.normalizedPosition, _normalizedPosition, Time.deltaTime * _speed);
            }
            else
            {
                _scrollRect.normalizedPosition = _normalizedPosition;
            }
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return !enabled;
        }
    }
}
