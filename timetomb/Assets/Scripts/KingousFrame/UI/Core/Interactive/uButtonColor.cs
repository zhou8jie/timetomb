using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//  UI_ButtonColor.cs
//  Author: Lu Zexi
//  2015-02-06

namespace UnityEngine.UI.Extensions
{

    [AddComponentMenu("UI/Interactive/Button Color")]
    public class uButtonColor : uButtonTrigger
    {
        /// <summary>
        /// Target with a widget, renderer, or light that will have its color tweened.
        /// </summary>

        public GameObject tweenTarget;

        /// <summary>
        /// Color to apply on hover event (mouse only).
        /// </summary>
        public Color hover = Color.white;

        /// <summary>
        /// Color to apply on the pressed event.
        /// </summary>
        public Color pressed = new Color(0.75f, 0.75f, 0.75f, 1f);

        /// <summary>
        /// Duration of the tween process.
        /// </summary>
        public float duration = 0f;

        Color mColor;
        Graphic mGraphic;
        bool mStarted = false;

        protected override void Awake()
        {
            base.Awake();

            if (!mStarted)
            {
                mStarted = true;
                Init();
            }
        }

        void OnDisable()
        {
            if (mStarted && tweenTarget != null)
            {
                if (this.mGraphic != null)
                    this.mGraphic.color = mColor;
            }
        }

        void Init()
        {
            if (tweenTarget == null) tweenTarget = gameObject;
            mGraphic = tweenTarget.GetComponent<Graphic>();
            if (mGraphic != null)
            {
                mColor = mGraphic.color;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsAvailable())
                uTweenColor.Begin(tweenTarget, duration, 0, mGraphic.color, pressed);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (IsAvailable())
                uTweenColor.Begin(tweenTarget, duration, 0, mGraphic.color, mColor);
        }
    }
}