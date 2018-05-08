using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    public class uScrollMovement : UIBehaviour
    {
        public enum Axis
        {
            Horizontal,
            Vertical,
            Both,
        }

        public Axis axis = Axis.Both;
        public float thershold = 0.01f;
        public UnityEvent onEndMove;



        private ScrollRect _scrollRect;

        protected override void Awake()
        {
            base.Awake();

            _scrollRect = GetComponent<ScrollRect>();

            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            }
        }

        private void OnScrollValueChanged(Vector2 arg0)
        {
            enabled = true;
        }

        void Update()
        {
            if (!_scrollRect.enabled) return;

            switch (axis)
            {
                case Axis.Both:
                    if (_scrollRect.normalizedPosition.x >= 0 && _scrollRect.normalizedPosition.x <= 1
                        && _scrollRect.normalizedPosition.y >= 0 && _scrollRect.normalizedPosition.y <= 1)
                    {
                        if (_scrollRect.velocity.magnitude < thershold)
                        {
                            EndMove();
                        }
                    }
                    break;
                default:
                    float normalizedPosition = _scrollRect.normalizedPosition[(int)axis];
                    float velocity = _scrollRect.velocity[(int)axis];
                    if (normalizedPosition >= 0 && normalizedPosition <= 1 && velocity < thershold)
                    {
                        EndMove();
                    }
                    break;
            }
        }

        public void EndMove()
        {
            enabled = false;

            if (onEndMove != null)
            {
                onEndMove.Invoke();
            }
        }
    }
}
