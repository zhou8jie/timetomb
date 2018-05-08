using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.UI.Extensions
{
    public class uToggleComponents : MonoBehaviour
    {
        public List<MonoBehaviour> activate;
        public List<MonoBehaviour> deactivate;

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
                {
                    MonoBehaviour comp = activate[i];
                    comp.enabled = v;
                }

                for (int i = 0; i < deactivate.Count; ++i)
                {
                    MonoBehaviour comp = deactivate[i];
                    comp.enabled = !v;
                }
            }
        }
    }
}
