using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//  UI_ButtonScale.cs
//  Author: Lu Zexi
//  2015-02-06

namespace UnityEngine.UI.Extensions
{

    [AddComponentMenu("UI/Interactive/Button Scale")]
    public class uButtonScale : uButtonTrigger
    {
        public Transform tweenTarget;
        public Vector3 hover = Vector3.one;
        public Vector3 pressed = new Vector3(0.95f, 0.95f, 0.95f);
        public float duration = 0f;

        Vector3 mScale;
        bool mStarted = false;

        protected override void Awake()
        {
            base.Awake();

            if (!mStarted)
            {
                mStarted = true;
                if (tweenTarget == null) tweenTarget = transform;
                mScale = tweenTarget.localScale;
            }
        }

        void OnDisable()
        {
            if (mStarted && tweenTarget != null)
                tweenTarget.localScale = mScale;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsAvailable())
                uTweenScale.Begin(tweenTarget.gameObject, tweenTarget.transform.localScale, pressed, duration);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (IsAvailable())
                uTweenScale.Begin(tweenTarget.gameObject, tweenTarget.transform.localScale, mScale, duration);
        }
    }
}