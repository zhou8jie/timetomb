using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Tween/Tween Anchored Position")]
    public class uTweenPosition : uTweener
    {

        public Vector3 from;
        public Vector3 to;

        Transform mTransform;

        public Transform cachedTransform { get { if (mTransform == null) mTransform = GetComponent<Transform>(); return mTransform; } }
        public Vector3 value
        {
            get { return cachedTransform.localPosition; }
            set { cachedTransform.localPosition = value; }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from + factor * (to - from);
        }

        public override void SampleCurrentAsStart()
        {
            from = value;
        }

        public static uTweenPosition Begin(GameObject go, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f)
        {
            uTweenPosition comp = uTweener.Begin<uTweenPosition>(go, duration);
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
