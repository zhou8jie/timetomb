using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Very basic script that will activate or deactivate an object (and all of its children) when clicked.
    /// </summary>

    [AddComponentMenu("UI/Interactive/Button Activate")]
    public class uButtonActivate : uButtonTrigger
    {
        public GameObject target;
        public bool state = true;
        public bool resetOnDisabled = true;

        private bool mStartState;

        protected override void Awake()
        {
            if (target == null) target = gameObject;
            mStartState = target.activeSelf;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsAvailable()) target.SetActive(state);
        }

        void OnDisable()
        {
            if (resetOnDisabled && target != null)
            {
                target.SetActive(mStartState);
            }
        }
    }
}