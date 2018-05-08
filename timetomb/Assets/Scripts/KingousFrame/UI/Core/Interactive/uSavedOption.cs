using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Attach this script to a popup list, the parent of a group of toggles, or to a toggle itself to save its state.
    /// </summary>

    [AddComponentMenu("UI/Interactive/Saved Option")]
    public class uSavedOption : MonoBehaviour
    {
        /// <summary>
        /// PlayerPrefs-stored key for this option.
        /// </summary>

        public string keyName;

        string key { get { return (string.IsNullOrEmpty(keyName)) ? ("uGUI State: " + name) : keyName; } }

        Dropdown mDropDown;
        Toggle mToggle;
        Slider mSlider;
        InputField mInput;

        /// <summary>
        /// Cache the components and register a listener callback.
        /// </summary>

        void Awake()
        {
            mDropDown = GetComponent<Dropdown>();
            mToggle = GetComponent<Toggle>();
            mSlider = GetComponent<Slider>();
            mInput = GetComponent<InputField>();
        }

        /// <summary>
        /// Load and set the state of the toggles.
        /// </summary>

        void OnEnable()
        {
            if (mDropDown != null)
            {
                mDropDown.onValueChanged.AddListener(SaveState);
                int v = PlayerPrefs.GetInt(key, mDropDown.value);
                mDropDown.value = v;
            }
            else if (mToggle != null)
            {
                mToggle.onValueChanged.AddListener(SaveState);
                mToggle.isOn = PlayerPrefs.GetInt(key, mToggle.isOn ? 1 : 0) != 0;
            }
            else if (mSlider != null)
            {
                mSlider.onValueChanged.AddListener(SaveState);
                mSlider.value = PlayerPrefs.GetFloat(key, mSlider.value);
            }
            else if (mInput != null)
            {
                mInput.onEndEdit.AddListener(SaveState);
                mInput.text = PlayerPrefs.GetString(key, mInput.text);
            }
        }

        /// <summary>
        /// Save the state on destroy.
        /// </summary>
        void OnDisable()
        {
            if (mToggle != null && mToggle.onValueChanged != null) mToggle.onValueChanged.RemoveListener(SaveState);
            else if (mDropDown != null && mDropDown.onValueChanged != null) mDropDown.onValueChanged.RemoveListener(SaveState);
            else if (mSlider != null && mSlider.onValueChanged != null) mSlider.onValueChanged.RemoveListener(SaveState);
            else if (mInput != null && mInput.onEndEdit != null) mInput.onEndEdit.RemoveListener(SaveState);
        }

        /// <summary>
        /// Save the state.
        /// </summary>
        public void SaveState(bool v) { PlayerPrefs.SetInt(key, v ? 1 : 0); }

        /// <summary>
        /// Save the selection.
        /// </summary>
        public void SaveState(int v) { PlayerPrefs.SetInt(key, v); }

        /// <summary>
        /// Save the current progress.
        /// </summary>
        public void SaveState(float v) { PlayerPrefs.SetFloat(key, v); }

        /// <summary>
        /// Save the current text
        /// </summary>
        /// <param name="v"></param>
        private void SaveState(string v) { PlayerPrefs.SetString(key, v); }
    }

}
