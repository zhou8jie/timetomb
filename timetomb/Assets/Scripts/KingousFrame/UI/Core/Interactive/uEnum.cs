using UnityEngine;
using System.Collections;

namespace UnityEngine.UI.Extensions
{
    public enum EnableCondition
    {
        DoNothing = 0,
        EnableThenPlay,
        IgnoreDisabledState,
    }

    public enum DisableCondition
    {
        DisableAfterReverse = -1,
        DoNotDisable = 0,
        DisableAfterForward = 1,
    }

    public enum Direction
    {
        Reverse = -1,
        Toggle = 0,
        Forward = 1
    }

    public enum ActionOnActivation
    {
        ContinueFromCurrent,
        Reset,
        ResetIfNotPlaying,
        SampleCurrentThenPlay,//对当前值取样,并以当前值为起始值播放动画
    }

    public enum Trigger
    {
        OnHover,
        OnHoverTrue,
        OnHoverFalse,

        OnPress,
        OnPressTrue,
        OnPressFalse,

        OnClick,
        
        OnToggle,
        OnToggleTrue,
        OnToggleFalse,

        OnDoubleClick,

        OnSelect,
        OnSelectTrue,
        OnSelectFalse,

        OnActivate,
        OnActivateTrue,
        OnActivateFalse,
    }
}