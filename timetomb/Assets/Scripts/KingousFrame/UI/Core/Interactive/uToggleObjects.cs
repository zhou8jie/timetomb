using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.UI.Extensions
{
    public class uToggleObjects : MonoBehaviour
    {
        public List<GameObject> activate;
        public List<GameObject> deactivate;

        public bool effectOnDeactive = true;

        void Awake()
        {
            Toggle toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnToggle);
            OnToggle(toggle.isOn);
        }

        public void OnToggle(bool v)
        {
            if (enabled || effectOnDeactive)
            {
                for (int i = 0; i < activate.Count; ++i)
                    activate[i].SetActive(v);

                for (int i = 0; i < deactivate.Count; ++i)
                    deactivate[i].SetActive(!v);
            }
        }
    }
}
