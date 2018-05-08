using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Tween/Tween Path")]
    public class uTweenPath : uTweenValue
    {
        public RectTransform target;
        public List<Vector3> paths;

        int mIndex = -1;
        int mPathsCount = 0;
        bool mCache = false;

        void Cache()
        {
            mCache = true;
            if (paths.Count > 1)
            {
                mPathsCount = paths.Count - 1;
            }
            if (target == null)
            {
                target = GetComponent<RectTransform>();
            }
            from = 0;
            to = mPathsCount;
        }

        protected override void ValueUpdate(float _factor, bool _isFinished)
        {
            if (!mCache) { Cache(); }
            pathIndex = Mathf.FloorToInt(_factor);
        }

        public override void SampleCurrentAsStart()
        {

        }

            int pathIndex
        {
            get { return mIndex; }
            set
            {
                if (mIndex != value)
                {
                    mIndex = value;
                    uTweenPosition tw = uTweenPosition.Begin(target.gameObject, target.localPosition, paths[mIndex], duration / paths.Count);
                    tw.style = Style.Once;
                    tw.ignoreTimeScale = ignoreTimeScale;
                }
            }
        }

    }
}
