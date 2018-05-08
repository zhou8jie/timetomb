using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//  UI_ButtonImage.cs
//  Author: Lu Zexi
//  2015-02-06

namespace UnityEngine.UI.Extensions
{

    [AddComponentMenu("UI/Interactive/Button Image Switch")]
    public class uButtonImage : uButtonTrigger
    {
        /// <summary>
        /// Target with a widget, renderer, or light that will have its Sprite tweened.
        /// </summary>

        public GameObject tweenTarget;

        /// <summary>
        /// Sprite to apply on normal.
        /// </summary>
        public Sprite normal = null;

        /// <summary>
        /// Sprite to apply on the pressed event.
        /// </summary>
        public Sprite pressed = null;

        Sprite mSprite;
        Image mImage;
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
                if (this.mImage != null)
                    this.mImage.sprite = mSprite;
            }
        }

        void Init()
        {
            if (tweenTarget == null) tweenTarget = gameObject;
            Image img = tweenTarget.GetComponent<Image>();
            this.mImage = img;
            if (img != null)
            {
                mSprite = img.sprite;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsAvailable())
                this.mImage.sprite = pressed;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (IsAvailable())
                this.mImage.sprite = normal;
        }
    }
}