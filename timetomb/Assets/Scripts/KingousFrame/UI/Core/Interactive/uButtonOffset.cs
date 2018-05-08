using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    /// <summary>
    /// Simple example script of how a button can be offset visibly when the mouse hovers over it or it gets pressed.
    /// </summary>
    [AddComponentMenu("UI/Interactive/Button Offset")]
    public class uButtonOffset : uButtonTrigger
    {
        public Transform tweenTarget;
        public Vector3 hover = Vector3.zero;
        public Vector3 pressed = new Vector3(2f, -2f);
        public float duration = 0f;

        Vector3 mPos;
        bool mStarted = false;

        protected override void Awake()
        {
            base.Awake();

            if (!mStarted)
            {
                mStarted = true;
                if (tweenTarget == null) tweenTarget = transform;
                mPos = tweenTarget.localPosition;
            }
        }

        void OnDisable()
        {
            if (mStarted && tweenTarget != null)
            {
                tweenTarget.localPosition = mPos;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsAvailable())
                uTweenAnchoredPosition.Begin(tweenTarget.gameObject, tweenTarget.transform.localPosition, mPos + pressed, duration);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (IsAvailable())
                uTweenAnchoredPosition.Begin(tweenTarget.gameObject, tweenTarget.transform.localPosition, mPos, duration);
        }
    }
}