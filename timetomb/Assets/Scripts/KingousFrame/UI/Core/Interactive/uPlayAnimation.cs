using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{

    /// <summary>
    /// Play the specified animation on click.
    /// </summary>

    [ExecuteInEditMode]
    [AddComponentMenu("UI/Interactive/Play Animation")]
    public class uPlayAnimation : uTrigger
    {
        /// <summary>
        /// Target animation to activate.
        /// </summary>
        public Animation target;

        /// <summary>
        /// Target animator system.
        /// </summary>
        public Animator animator;

        /// <summary>
        /// Optional clip name, if the animation has more than one clip.
        /// </summary>
        public string clipName;

        /// <summary>
        /// Direction to tween in 
        /// </summary>
        public Direction direction;

        /// <summary>
        /// Whether the animation's position will be reset on play or will continue from where it left off.
        /// </summary>
        public bool resetOnPlay = true;

        /// <summary>
        /// What to do if the target game object is currently disabled.
        /// </summary>
        public EnableCondition ifDisabledOnPlay = EnableCondition.DoNothing;

        /// <summary>
        /// What to do with the target when the animation finishes.
        /// </summary>
        public DisableCondition disableWhenFinished = DisableCondition.DoNotDisable;

        /// <summary>
        /// Called when the animation finishes.
        /// </summary>
        public UnityEvent onFinished;

        private ActiveAnimation mCurrentAnim;

        bool dualState { get { return trigger == Trigger.OnHoverTrue || trigger == Trigger.OnPressTrue; } }

        protected override void OnTriggered(Trigger trigger)
        {
            Play();
        }

        /// <summary>
        /// Start playing the animation.
        /// </summary>
        public void Play()
        {
            Play(direction);
        }

        private void Play(Direction dir)
        {
            if (mCurrentAnim != null)
            {
                if (mCurrentAnim.onFinished != null)
                    mCurrentAnim.onFinished.RemoveListener(OnFinished);
                mCurrentAnim = null;
            }

            if (target || animator)
            {
                mCurrentAnim = target ?
                    ActiveAnimation.Play(target, clipName, dir, ifDisabledOnPlay, disableWhenFinished) :
                    ActiveAnimation.Play(animator, clipName, dir, ifDisabledOnPlay, disableWhenFinished);

                if (mCurrentAnim != null)
                {
                    if (resetOnPlay) mCurrentAnim.Reset();
                    mCurrentAnim.onFinished.AddListener(OnFinished);
                }
            }
        }

        /// <summary>
        /// Play the tween forward.
        /// </summary>
        public void PlayForward() { Play(Direction.Forward); }

        /// <summary>
        /// Play the tween in reverse.
        /// </summary>
        public void PlayReverse() { Play(Direction.Reverse); }

        /// <summary>
        /// Callback triggered when each tween executed by this script finishes.
        /// </summary>
        void OnFinished()
        {
            if (mCurrentAnim != null)
            {
                mCurrentAnim.onFinished.RemoveListener(OnFinished);
                mCurrentAnim = null;

                if (onFinished != null)
                {
                    onFinished.Invoke();
                }
            }
        }
    }

}
