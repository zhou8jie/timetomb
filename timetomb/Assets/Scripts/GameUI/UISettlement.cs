using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettlement : UIBase
{
    public void OnClickReturn()
    {
        GameGlobal.Instance().mode.ChangeGameState(GameMode.State_Main);
        hide();
    }
}
