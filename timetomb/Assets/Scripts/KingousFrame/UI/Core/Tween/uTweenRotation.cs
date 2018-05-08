using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Tween/Tween Rotation")]
    public class uTweenRotation : uTweener
    {
        public Vector3 from;
        public Vector3 to;

        Transform mRectTransfrom;

        public Transform target;

        public Transform cacheRectTransfrom
        {
            get
            {
                if (target == null)
                {
                    mRectTransfrom = GetComponent<Transform>();
                }
                else
                {
                    mRectTransfrom = target;
                }
                return mRectTransfrom;
            }
        }

        public Quaternion value
        {
            get
            {
                return cacheRectTransfrom.localRotation;
            }
            set
            {
                cacheRectTransfrom.localRotation = value;
            }
        }

        public override void SampleCurrentAsStart()
        {
            from = value.eulerAngles;
        }

        protected override void OnUpdate(float _factor, bool _isFinished)
        {
            value = Quaternion.Euler(Vector3.Lerp(from, to, _factor));
        }

        public static uTweenRotation Begin(GameObject go, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f)
        {
            uTweenRotation comp = uTweener.Begin<uTweenRotation>(go, duration);
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
    }
}