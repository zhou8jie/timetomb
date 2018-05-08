using UnityEngine;
using System.Collections;

public static class uSound
{
    static AudioListener mListener;

    static bool mLoaded = false;
    static float mGlobalVolume = 1f;

    /// <summary>
    /// Globally accessible volume affecting all sounds played via GUITools.PlaySound().
    /// </summary>

    static public float soundVolume
    {
        get
        {
            if (!mLoaded)
            {
                mLoaded = true;
                mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
            }
            return mGlobalVolume;
        }
        set
        {
            if (mGlobalVolume != value)
            {
                mLoaded = true;
                mGlobalVolume = value;
                PlayerPrefs.SetFloat("Sound", value);
            }
        }
    }

    /// <summary>
    /// Play the specified audio clip.
    /// </summary>

    static public AudioSource PlaySound(AudioClip clip) { return PlaySound(clip, 1f, 1f); }

    /// <summary>
    /// Play the specified audio clip with the specified volume.
    /// </summary>

    static public AudioSource PlaySound(AudioClip clip, float volume) { return PlaySound(clip, volume, 1f); }

    static float mLastTimestamp = 0f;
    static AudioClip mLastClip;

    /// <summary>
    /// Play the specified audio clip with the specified volume and pitch.
    /// </summary>

    static public AudioSource PlaySound(AudioClip clip, float volume, float pitch)
    {
        float time = Time.time;
        if (mLastClip == clip && mLastTimestamp + 0.1f > time) return null;

        mLastClip = clip;
        mLastTimestamp = time;
        volume *= soundVolume;

        if (clip != null && volume > 0.01f)
        {
            if (mListener == null || !mListener.gameObject.activeInHierarchy)
            {
                AudioListener[] listeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];

                if (listeners != null)
                {
                    for (int i = 0; i < listeners.Length; ++i)
                    {
                        if (listeners[i].gameObject.activeInHierarchy)
                        {
                            mListener = listeners[i];
                            break;
                        }
                    }
                }

                if (mListener == null)
                {
                    Camera cam = Camera.main;
                    if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
                    if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
                }
            }

            if (mListener != null && mListener.enabled && mListener.gameObject.activeInHierarchy)
            {
                AudioSource source = mListener.GetComponent<AudioSource>();
                if (source == null) source = mListener.gameObject.AddComponent<AudioSource>();
                source.PlayOneShot(clip, volume);
                return source;
            }
        }
        return null;
    }
}
