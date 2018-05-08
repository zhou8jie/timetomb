using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.UI
{
    public interface IUpdateElement
    {
        /// <summary>
        /// if true, the element will update next time
        /// </summary>
        /// <returns></returns>
        bool PerformUpdate();
    }
}
