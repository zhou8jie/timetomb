using UnityEngine;
using System.Collections;
using System;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Interactive/Scrollbar Auto-hide")]
    [RequireComponent(typeof(ScrollRect))]
    public class uScrollbarAutoHide : MonoBehaviour
    {
        [SerializeField]
        private float _fadeTimeout = 2;
        [SerializeField]
        private float _tweenSpeed = 8;

        private ScrollRect _scrollRect;
        private CanvasGroup _horizontalCanvasGroup;
        private CanvasGroup _verticalCanvasGroup;

        private float _timeout = 0;
        private float _targetAlpha = 1;
        private Vector2 _lastScrollValue;

        void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();

            if (_scrollRect.horizontalScrollbar != null)
            {
                _horizontalCanvasGroup = _scrollRect.horizontalScrollbar.gameObject.GetComponent<CanvasGroup>();
                if (_horizontalCanvasGroup == null) _horizontalCanvasGroup = _scrollRect.horizontalScrollbar.gameObject.AddComponent<CanvasGroup>();
                _horizontalCanvasGroup.alpha = _targetAlpha;
            }

            if (_scrollRect.verticalScrollbar != null)
            {
                _verticalCanvasGroup = _scrollRect.verticalScrollbar.gameObject.GetComponent<CanvasGroup>();
                if (_verticalCanvasGroup == null) _verticalCanvasGroup = _scrollRect.verticalScrollbar.gameObject.AddComponent<CanvasGroup>();
                _verticalCanvasGroup.alpha = _targetAlpha;
            }

            if (_verticalCanvasGroup == null && _horizontalCanvasGroup == null)
                Destroy(this);
            else {
                _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
                _lastScrollValue = _scrollRect.normalizedPosition;
                enabled = false;
            }
            _timeout = _fadeTimeout;
        }

        private void OnScrollValueChanged(Vector2 v)
        {
            if (Vector2.Distance(_lastScrollValue, v) < 0.0001f) return;
            _lastScrollValue = v;
            enabled = true;
            _timeout = _fadeTimeout;
            _targetAlpha = 1;
        }

        void Update()
        {
            float delta = Time.deltaTime * _tweenSpeed;
            if (_timeout <= 0) _targetAlpha = 0;
            else _timeout -= Time.deltaTime;

            if (_horizontalCanvasGroup != null)
                _horizontalCanvasGroup.alpha = Mathf.Lerp(_horizontalCanvasGroup.alpha, _targetAlpha, delta);
            if (_verticalCanvasGroup != null)
                _verticalCanvasGroup.alpha = Mathf.Lerp(_verticalCanvasGroup.alpha, _targetAlpha, delta);

            if (_targetAlpha == 0)
            {
                if (Mathf.Max(
                    _horizontalCanvasGroup == null ? 0 : Mathf.Abs(_horizontalCanvasGroup.alpha - _targetAlpha),
                    _verticalCanvasGroup == null ? 0 : Mathf.Abs(_verticalCanvasGroup.alpha - _targetAlpha)) < 0.001f)
                {
                    enabled = false;
                }
            }
        }
    }
}