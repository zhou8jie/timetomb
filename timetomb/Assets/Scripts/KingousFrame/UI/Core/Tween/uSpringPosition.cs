using UnityEngine;
using UnityEngine.Events;


namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Spring-like motion -- the farther away the object is from the target, the stronger the pull.
    /// </summary>

    [AddComponentMenu("UI/Tween/Spring Position")]
    public class uSpringPosition : MonoBehaviour
    {
        static public uSpringPosition current;

        /// <summary>
        /// Target position to tween to.
        /// </summary>

        public Vector3 target = Vector3.zero;

        /// <summary>
        /// Strength of the spring. The higher the value, the faster the movement.
        /// </summary>

        public float strength = 10f;

        /// <summary>
        /// Is the calculation done in world space or local space?
        /// </summary>

        public bool worldSpace = false;

        /// <summary>
        /// Whether the time scale will be ignored. Generally UI components should set it to 'true'.
        /// </summary>

        public bool ignoreTimeScale = false;

        /// <summary>
        /// Whether the parent scroll view will be updated as the object moves.
        /// </summary>

        public bool updateScrollView = false;

        /// <summary>
        /// Delegate to trigger when the spring finishes.
        /// </summary>

        public UnityEvent onFinished;

        Transform mTrans;
        float mThreshold = 0f;
        //ScrollRect mSv;

        /// <summary>
        /// Cache the transform.
        /// </summary>

        void Start()
        {
            mTrans = transform;
            //if (updateScrollView) mSv = gameObject.GetComponentInParent<ScrollRect>();
        }

        /// <summary>
        /// Advance toward the target position.
        /// </summary>

        void Update()
        {
            float delta = ignoreTimeScale ? Time.fixedDeltaTime : Time.deltaTime;

            if (worldSpace)
            {
                if (mThreshold == 0f) mThreshold = (target - mTrans.position).sqrMagnitude * 0.001f;
                mTrans.position = uMath.SpringLerp(mTrans.position, target, strength, delta);

                if (mThreshold >= (target - mTrans.position).sqrMagnitude)
                {
                    mTrans.position = target;
                    NotifyListeners();
                    enabled = false;
                }
            }
            else
            {
                if (mThreshold == 0f) mThreshold = (target - mTrans.localPosition).sqrMagnitude * 0.00001f;
                mTrans.localPosition = uMath.SpringLerp(mTrans.localPosition, target, strength, delta);

                if (mThreshold >= (target - mTrans.localPosition).sqrMagnitude)
                {
                    mTrans.localPosition = target;
                    NotifyListeners();
                    enabled = false;
                }
            }

            // Ensure that the scroll bars remain in sync
            //if (mSv != null) mSv.UpdateScrollbars(true);
        }

        /// <summary>
        /// Notify all finished event listeners.
        /// </summary>

        void NotifyListeners()
        {
            current = this;

            if (onFinished != null) onFinished.Invoke();

            current = null;
        }

        /// <summary>
        /// Start the tweening process.
        /// </summary>

        static public uSpringPosition Begin(GameObject go, Vector3 pos, float strength)
        {
            uSpringPosition sp = go.GetComponent<uSpringPosition>();
            if (sp == null) sp = go.AddComponent<uSpringPosition>();
            sp.target = pos;
            sp.strength = strength;
            if (sp.onFinished == null)
                sp.onFinished = new UnityEvent();
            else
                sp.onFinished.RemoveAllListeners();

            if (!sp.enabled)
            {
                sp.mThreshold = 0f;
                sp.enabled = true;
            }
            return sp;
        }
    }
}