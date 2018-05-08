using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Tween/Tween ScrollRect")]
    public class uTweenScrollRect : uTweener
    {

        public Vector2 from;
        public Vector2 to;

        ScrollRect mScrollRect;

        public ScrollRect cachedScrollRect { get { if (mScrollRect == null) mScrollRect = GetComponent<ScrollRect>(); return mScrollRect; } }
        public Vector2 value
        {
            get { return cachedScrollRect.normalizedPosition; }
            set { cachedScrollRect.normalizedPosition = value; }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from + factor * (to - from);
        }

        public override void SampleCurrentAsStart()
        {
            from = value;
        }

        public static uTweenScrollRect Begin(GameObject go, Vector2 from, Vector2 to, float duration = 1f, float delay = 0f)
        {
            uTweenScrollRect comp = uTweener.Begin<uTweenScrollRect>(go, duration);
            comp.from = from;
            comp.to = to;
            comp.duration = duration;
            comp.delay = delay;
            if (duration <= 0)
            {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }

        [ContextMenu("Set 'From' to current value")]
        public override void SetStartToCurrentValue() { from = value; }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue() { to = value; }

        [ContextMenu("Assume value of 'From'")]
        public override void SetCurrentValueToStart() { value = from; }

        [ContextMenu("Assume value of 'To'")]
        public override void SetCurrentValueToEnd() { value = to; }

    }
}
