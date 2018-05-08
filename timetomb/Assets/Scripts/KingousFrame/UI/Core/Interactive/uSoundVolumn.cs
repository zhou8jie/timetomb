using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Slider))]
    [AddComponentMenu("UI/Interactive/Sound Volume")]
    public class uSoundVolume : MonoBehaviour
    {
        void Awake()
        {
            Slider slider = GetComponent<Slider>();
            slider.value = uSound.soundVolume;
            slider.onValueChanged.AddListener(OnChange);
        }

        void OnChange(float v)
        {
            uSound.soundVolume = v;
        }
    }
}
