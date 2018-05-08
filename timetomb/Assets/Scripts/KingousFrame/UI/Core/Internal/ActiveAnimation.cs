using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Mainly an internal script used by UIButtonPlayAnimation, but can also be used to call
    /// the specified function on the game object after it finishes animating.
    /// </summary>

    [AddComponentMenu("UI/Interactive/ActiveAnimation")]
    public class ActiveAnimation : MonoBehaviour
    {
        /// <summary>
        /// Event delegates called when the animation finishes.
        /// </summary>

        public UnityEvent onFinished = new UnityEvent();

        Animation mAnim;
        Direction mLastDirection = Direction.Toggle;
        Direction mDisableDirection = Direction.Toggle;
        bool mNotify = false;

        Animator mAnimator;
        string mClip = "";

        float playbackTime
        {
            get
            {
                AnimatorStateInfo state = mAnimator.GetCurrentAnimatorStateInfo(0);
                return Mathf.Clamp01(state.normalizedTime);
            }
        }

        /// <summary>
        /// Whether the animation is currently playing.
        /// </summary>

        public bool isPlaying
        {
            get
            {
                if (mAnim == null)
                {
                    if (mAnimator != null)
                    {
                        if (mLastDirection == Direction.Reverse)
                        {
                            if (playbackTime == 0f) return false;
                        }
                        else if (playbackTime == 1f) return false;
                        return true;
                    }
                    return false;
                }

                foreach (AnimationState state in mAnim)
                {
                    if (!mAnim.IsPlaying(state.name)) continue;

                    if (mLastDirection == Direction.Forward)
                    {
                        if (state.time < state.length) return true;
                    }
                    else if (mLastDirection == Direction.Reverse)
                    {
                        if (state.time > 0f) return true;
                    }
                    else return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Immediately finish playing the animation.
        /// </summary>

        public void Finish()
        {
            if (mAnim != null)
            {
                foreach (AnimationState state in mAnim)
                {
                    if (mLastDirection == Direction.Forward) state.time = state.length;
                    else if (mLastDirection == Direction.Reverse) state.time = 0f;
                }
                mAnim.Sample();
            }
            else if (mAnimator != null)
            {
                mAnimator.Play(mClip, 0, (mLastDirection == Direction.Forward) ? 1f : 0f);
            }
        }

        /// <summary>
        /// Manually reset the active animation to the beginning.
        /// </summary>

        public void Reset()
        {
            if (mAnim != null)
            {
                foreach (AnimationState state in mAnim)
                {
                    if (mLastDirection == Direction.Reverse) state.time = state.length;
                    else if (mLastDirection == Direction.Forward) state.time = 0f;
                }
            }
            else if (mAnimator != null)
            {
                mAnimator.Play(mClip, 0, (mLastDirection == Direction.Reverse) ? 1f : 0f);
            }
        }

        /// <summary>
        /// Notify the target when the animation finishes playing.
        /// </summary>

        void Update()
        {
            float delta = Time.fixedDeltaTime;
            if (delta == 0f) return;

            if (mAnimator != null)
            {
                mAnimator.Update((mLastDirection == Direction.Reverse) ? -delta : delta);
                if (isPlaying) return;
                mAnimator.enabled = false;
                enabled = false;
            }
            else if (mAnim != null)
            {
                bool playing = false;

                foreach (AnimationState state in mAnim)
                {
                    if (!mAnim.IsPlaying(state.name)) continue;
                    float movement = state.speed * delta;
                    state.time += movement;

                    if (movement < 0f)
                    {
                        if (state.time > 0f) playing = true;
                        else state.time = 0f;
                    }
                    else
                    {
                        if (state.time < state.length) playing = true;
                        else state.time = state.length;
                    }
                }

                mAnim.Sample();
                if (playing) return;
                enabled = false;
            }
            else
            {
                enabled = false;
                return;
            }

            if (mNotify)
            {
                mNotify = false;
                if (onFinished != null)
                    onFinished.Invoke();
                if (mDisableDirection != Direction.Toggle && mLastDirection == mDisableDirection)
                    gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Play the specified animation.
        /// </summary>

        void Play(string clipName, Direction playDirection)
        {
            // Determine the play direction
            if (playDirection == Direction.Toggle)
                playDirection = (mLastDirection != Direction.Forward) ? Direction.Forward : Direction.Reverse;

            if (mAnim != null)
            {
                // We will sample the animation manually so that it works when the time is paused
                enabled = true;
                mAnim.enabled = false;

                bool noName = string.IsNullOrEmpty(clipName);

                // Play the animation if it's not playing already
                if (noName)
                {
                    if (!mAnim.isPlaying) mAnim.Play();
                }
                else if (!mAnim.IsPlaying(clipName))
                {
                    mAnim.Play(clipName);
                }

                // Update the animation speed based on direction -- forward or back
                foreach (AnimationState state in mAnim)
                {
                    if (string.IsNullOrEmpty(clipName) || state.name == clipName)
                    {
                        float speed = Mathf.Abs(state.speed);
                        state.speed = speed * (int)playDirection;

                        // Automatically start the animation from the end if it's playing in reverse
                        if (playDirection == Direction.Reverse && state.time == 0f) state.time = state.length;
                        else if (playDirection == Direction.Forward && state.time == state.length) state.time = 0f;
                    }
                }

                // Remember the direction for disable checks in Update()
                mLastDirection = playDirection;
                mNotify = true;
                mAnim.Sample();
            }
            else if (mAnimator != null)
            {
                if (enabled && isPlaying)
                {
                    if (mClip == clipName)
                    {
                        mLastDirection = playDirection;
                        return;
                    }
                }

                enabled = true;
                mNotify = true;
                mLastDirection = playDirection;
                mClip = clipName;
                mAnimator.Play(mClip, 0, (playDirection == Direction.Forward) ? 0f : 1f);

                // NOTE: If you are getting a message "Animator.GotoState: State could not be found"
                // it means that you chose a state name that doesn't exist in the Animator window.
            }
        }

        /// <summary>
        /// Play the specified animation on the specified object.
        /// </summary>

        static public ActiveAnimation Play(Animation anim, string clipName, Direction playDirection,
            EnableCondition enableBeforePlay, DisableCondition disableCondition)
        {
            if (!anim.gameObject.activeSelf)
            {
                // If the object is disabled, don't do anything
                if (enableBeforePlay != EnableCondition.EnableThenPlay) return null;

                // Enable the game object before animating it
                anim.gameObject.SetActive(true);

                // Refresh all panels right away so that there is no one frame delay
            }

            ActiveAnimation aa = anim.GetComponent<ActiveAnimation>();
            if (aa == null) aa = anim.gameObject.AddComponent<ActiveAnimation>();
            aa.mAnim = anim;
            aa.mDisableDirection = (Direction)(int)disableCondition;
            aa.onFinished.RemoveAllListeners();
            aa.Play(clipName, playDirection);

            if (aa.mAnim != null) aa.mAnim.Sample();
            else if (aa.mAnimator != null) aa.mAnimator.Update(0f);
            return aa;
        }

        /// <summary>
        /// Play the specified animation.
        /// </summary>

        static public ActiveAnimation Play(Animation anim, string clipName, Direction playDirection)
        {
            return Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
        }

        /// <summary>
        /// Play the specified animation.
        /// </summary>

        static public ActiveAnimation Play(Animation anim, Direction playDirection)
        {
            return Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
        }

        /// <summary>
        /// Play the specified animation on the specified object.
        /// </summary>

        static public ActiveAnimation Play(Animator anim, string clipName, Direction playDirection,
            EnableCondition enableBeforePlay, DisableCondition disableCondition)
        {
            if (enableBeforePlay != EnableCondition.IgnoreDisabledState && !anim.gameObject.activeSelf)
            {
                // If the object is disabled, don't do anything
                if (enableBeforePlay != EnableCondition.EnableThenPlay) return null;

                // Enable the game object before animating it
                anim.gameObject.SetActive(true);

                // Refresh all panels right away so that there is no one frame delay
            }

            ActiveAnimation aa = anim.GetComponent<ActiveAnimation>();
            if (aa == null) aa = anim.gameObject.AddComponent<ActiveAnimation>();
            aa.mAnimator = anim;
            aa.mDisableDirection = (Direction)(int)disableCondition;
            if (aa.onFinished != null)
                aa.onFinished.RemoveAllListeners();
            aa.Play(clipName, playDirection);

            if (aa.mAnim != null) aa.mAnim.Sample();
            else if (aa.mAnimator != null) aa.mAnimator.Update(0f);
            return aa;
        }
    }
}