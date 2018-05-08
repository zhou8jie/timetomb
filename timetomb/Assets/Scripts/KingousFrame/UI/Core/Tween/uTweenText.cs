using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Tween/Tween Text")]

    public class uTweenText : uTweenValue
    {

        private Text mText;
        public Text cacheText
        {
            get
            {
                if (mText == null)
                {
                    mText = GetComponent<Text>();
                }
                return mText;
            }
        }

        /// <summary>
        /// number after the digit point
        /// </summary>
        public int digits;

        protected override void ValueUpdate(float value, bool isFinished)
        {
            cacheText.text = (System.Math.Round(value, digits)).ToString();
        }

        public override void SampleCurrentAsStart()
        {
            if (cacheText != null)
            {
                int value;
                if (int.TryParse(cacheText.text, out value))
                {
                    from = value;
                }
            }
        }

        public static uTweenText Begin(Text label, float duration, float delay, float from, float to)
        {
            uTweenText comp = uTweener.Begin<uTweenText>(label.gameObject, duration);
            comp.from = from;
            comp.to = to;
            comp.delay = delay;

            if (duration <= 0)
            {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }
    }
}
