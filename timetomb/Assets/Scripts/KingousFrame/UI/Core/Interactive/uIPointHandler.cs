using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI.Extensions
{
    public interface uIPointHandler :
        IPointerEnterHandler,
        IPointerDownHandler,
        IPointerClickHandler,
        IPointerUpHandler,
        IPointerExitHandler
    {
    }
}