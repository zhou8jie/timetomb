using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KingousFramework
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource BGAudio;
        public AudioSource FxAudio;
        public string Path = "AudioClips/";
        
        private static SoundManager s_Instance = null;
        public SoundManager Instance()
        {
            return s_Instance;
        }

        private Dictionary<string, AudioClip> m_Clips = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            s_Instance = this;
        }

        public bool PlayBGM(string name)
        {
            var clip = _GetAudioClip(name);
            if (clip != null)
            {
                BGAudio.clip = clip;
                BGAudio.Play();
                return true;
            }
            else
                return false;
        }

        public void StopBGM()
        {
            if (BGAudio.isPlaying)
                BGAudio.Stop();
        }

        public void PauseBGM()
        {
            if (BGAudio.isPlaying)
                BGAudio.Pause();
        }

        public void ResumeBGM()
        {
            if (BGAudio.isPlaying)
                BGAudio.UnPause();
        }

        public void CrossFadeBGM(string name, float during)
        {
            float half = during / 2f;
            StartCoroutine(_FadeOutBGM(name, half));
        }

        IEnumerator _FadeOutBGM(string name, float during)
        {
            while (BGAudio.volume > 0)
            {
                BGAudio.volume -= Time.deltaTime * 1f / during;
                yield return 0;
            }
            BGAudio.volume = 0;
            yield return _FadeInBGM(name, during);
        }

        IEnumerator _FadeInBGM(string name, float during)
        {
            PlayBGM(name);
            while (BGAudio.volume < 1f)
            {
                BGAudio.volume += Time.deltaTime * 1f / during;
                yield return 0;
            }
            BGAudio.volume = 1f;
            yield return 0;
        }

        public bool PlayFx(string name)
        {
            var clip = _GetAudioClip(name);
            if (clip != null)
            {
                FxAudio.PlayOneShot(clip);
                return true;
            }
            return false;
        }

        AudioClip _GetAudioClip(string name)
        {
            if (m_Clips.ContainsKey(name))
                return m_Clips[name];
            var clip = Resources.Load<AudioClip>(Path + name);
            if (clip == null)
            {
                Debug.LogError("Cant find audio clip named : " + Path + name);
                return null;
            }
            m_Clips[name] = clip;
            return clip;
        }
    }
}