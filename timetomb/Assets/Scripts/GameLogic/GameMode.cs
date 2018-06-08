using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingousFramework;

public class GameMode
{
    public static readonly string State_Main = "Main";
    public static readonly string State_EnterLevel = "EnterLevel";
    public static readonly string State_Play = "Play";
    public static readonly string State_Solved = "Solved";
    public static readonly string State_Settlement = "Settlement";

    public string curState
    {
        get;
        protected set;
    }

    FSM m_FSM = new FSM();

    public GameMode()
    {
        m_FSM.addEnterState(State_Main, EnterMain);
        m_FSM.addEnterState(State_EnterLevel, EnterEnterLevelState);
        m_FSM.addEnterState(State_Solved, EnterSolvedState);
    }

    void EnterMain()
    {
        UIManager.get().show("UIMain");
    }

    void EnterEnterLevelState()
    {
        UIManager.get().find<UIMain>("UIMain").ToTransparent(1f);
        GameGlobal.Instance().timer.AddTimer(1f, 1f, false, () => {
            ChangeGameState(GameMode.State_Play);
        });
    }

    void EnterPlayState()
    {
        UIManager.get().find("UIMain").hide();
        GameGlobal.Instance().levelMgr.StartCurLevel();
    }

    void UpdatePlayState()
    {

    }

    void EnterSolvedState()
    {
        GameGlobal.Instance().timer.AddTimer(1f, 1f, false, () =>
        {
            ChangeGameState(GameMode.State_Settlement);
        });
    }

    public void ChangeGameState(string state)
    {
        curState = state;
        m_FSM.changeToState(state);
    }

    public void UpdateGameMode()
    {
        m_FSM.updateState();
    }
}
