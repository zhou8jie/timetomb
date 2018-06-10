using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettlement : UIBase
{
    public Button btnReturn;

    protected override void Awake()
    {
        btnReturn.onClick.AddListener(() => OnClickReturn());
    }

    public void OnClickReturn()
    {
        hide();
        GameGlobal.Instance().mode.ChangeGameState(GameMode.State_Main);
    }
}
