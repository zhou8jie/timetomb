// #define GAME_MEDIA


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if GAME_MEDIA
using Game.Media;
#endif


namespace UnityEngine.UI.Extensions
{

    /// <summary>
    /// Plays the specified sound.
    /// </summary>
    [AddComponentMenu("UI/Interactive/Button Play Sound")]
    public class uButtonPlaySound : uButtonTrigger
    {
        public AudioClip audioClip;

        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0f, 2f)]
        public float pitch = 1f;

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsAvailable())
                uSound.PlaySound(audioClip, volume, pitch);
        }
    }
}