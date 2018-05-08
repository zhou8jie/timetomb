using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Interactive/Play Sound")]
    public class uPlaySound : uTrigger
    {
        public AudioClip audioClip;

        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0f, 2f)]
        public float pitch = 1f;

        protected override void OnTriggered(Trigger trigger)
        {
            PlaySound();
        }

        private void PlaySound()
        {
            if (audioClip != null)
            {
                uSound.PlaySound(audioClip, volume, pitch);
            }
        }
    }
}