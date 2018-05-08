using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Interactive/Play Tween")]
    public class uPlayTween : uTrigger
    {
        /// <summary>
        /// Target on which there is one or more tween
        /// </summary>
        public GameObject target;

        /// <summary>
        /// If there are multiple tweens, you can choose which ones get activated by changing their group.
        /// </summary>

        public int tweenGroup = 0;

        /// <summary>
        /// Direction to tween in 
        /// </summary>
        public Direction playDirection = Direction.Forward;

        /// <summary>
        /// Decide what to do when activated
        /// </summary>
        public ActionOnActivation actionOnActivation = ActionOnActivation.ContinueFromCurrent;

        /// <summary>
        /// What to do if the tweenTarget game object is currently disabled.
        /// </summary>

        public EnableCondition ifDisabledOnPlay = EnableCondition.DoNothing;

        /// <summary>
        /// What to do with the tweenTarget after the tween finishes.
        /// </summary>

        public DisableCondition disableWhenFinished = DisableCondition.DoNotDisable;

        /// <summary>
        /// Whether the tweens on the child game objects will be considered.
        /// </summary>

        public bool includeChildren = false;

        /// <summary>
        /// called when the animation finishes.
        /// </summary>
        public UnityEvent onFinished;

        private uTweener[] mTweens;
        private int mActive;

        void Update()
        {
            if (disableWhenFinished != DisableCondition.DoNotDisable && mTweens != null)
            {
                bool isFinished = true;
                bool properDirection = true;

                for (int i = 0, imax = mTweens.Length; i < imax; ++i)
                {
                    uTweener tw = mTweens[i];
                    if (tw.tweenGroup != tweenGroup) continue;

                    if (tw.enabled)
                    {
                        isFinished = false;
                        break;
                    }
                    else if ((int)tw.direction != (int)disableWhenFinished)
                    {
                        properDirection = false;
                    }
                }

                if (isFinished)
                {
                    if (properDirection) target.SetActive(false);
                    mTweens = null;
                }
            }
        }

        protected override void OnTriggered(Trigger trigger)
        {
            Play();
        }

        /// <summary>
        /// Play this instance.
        /// </summary>
        public void Play()
        {
            mActive = 0;
            GameObject go = (target == null) ? gameObject : target;

            if (!go.activeSelf)
            {
                // If the object is disabled, don't do anything
                if (ifDisabledOnPlay != EnableCondition.EnableThenPlay) return;

                // Enable the game object before tweening it
                go.SetActive(true);
            }

            // Gather the tweening components
            mTweens = includeChildren ? go.GetComponentsInChildren<uTweener>() : go.GetComponents<uTweener>();

            if (mTweens.Length == 0)
            {
                // No tweeners found -- should we disable the object?
                if (disableWhenFinished != DisableCondition.DoNotDisable)
                    target.SetActive(false);
            }
            else
            {
                bool activated = false;

                // Run through all located tween components
                for (int i = 0, imax = mTweens.Length; i < imax; ++i)
                {
                    uTweener tw = mTweens[i];

                    // If the tweener's group matches, we can work with it
                    if (tw.tweenGroup == tweenGroup)
                    {
                        // Ensure that the game objects are enabled
                        if (!activated && !go.activeSelf)
                        {
                            activated = true;
                            go.SetActive(true);
                        }

                        ++mActive;

                        // Toggle or activate the tween component
                        if (playDirection == Direction.Toggle)
                        {
                            // Listen for tween finished messages
                            tw.onFinished.AddListener(OnFinished);
                            tw.Toggle();
                        }
                        else
                        {
                            if (actionOnActivation == ActionOnActivation.Reset || (actionOnActivation == ActionOnActivation.ResetIfNotPlaying && !tw.enabled))
                            {
                                tw.Play(playDirection == Direction.Forward);
                                tw.ResetToBeginning();
                            }
                            if (actionOnActivation == ActionOnActivation.SampleCurrentThenPlay && !tw.enabled)
                            {
                                tw.SampleCurrentAsStart();
                                tw.ResetToBeginning();
                            }
                            // Listen for tween finished messages
                            tw.onFinished.AddListener(OnFinished);
                            tw.Play(playDirection == Direction.Forward);
                        }
                    }
                }
            }
        }

        private void OnFinished()
        {
            if (--mActive == 0)
            {
                onFinished.Invoke();
            }
        }
    }
}