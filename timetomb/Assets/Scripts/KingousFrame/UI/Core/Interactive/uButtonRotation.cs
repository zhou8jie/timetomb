using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    /// <summary>
    /// Simple example script of how a button can be offset visibly when the mouse hovers over it or it gets pressed.
    /// </summary>
    [AddComponentMenu("UI/Interactive/Button Rotation")]
    public class uButtonRotation : uButtonTrigger
    {
        public Transform tweenTarget;
        public Vector3 hover = Vector3.zero;
        public Vector3 pressed = new Vector3(2f, -2f);
        public float duration = 0f;

        Vector3 mRot;
        bool mStarted = false;

        protected override void Awake()
        {
            base.Awake();

            if (!mStarted)
            {
                mStarted = true;
                if (tweenTarget == null) tweenTarget = transform;
                mRot = tweenTarget.eulerAngles;
            }
        }

        void OnDisable()
        {
            if (mStarted && tweenTarget != null)
            {
                tweenTarget.eulerAngles = mRot;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsAvailable())
                uTweenRotation.Begin(tweenTarget.gameObject, tweenTarget.transform.eulerAngles, mRot + pressed, duration);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (IsAvailable())
                uTweenRotation.Begin(tweenTarget.gameObject, tweenTarget.transform.eulerAngles, mRot, duration);
        }
    }
}