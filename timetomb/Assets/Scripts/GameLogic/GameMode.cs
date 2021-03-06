﻿using System.Collections;
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

    public static float ReadyCameraSize = 10;
    public static float SolvedCameraSize = 15;

    public string curState
    {
        get;
        protected set;
    }

    FSM m_FSM = new FSM();

    public GameMode()
    {
        m_FSM.addEnterState(State_Main, EnterMainState);
        m_FSM.addEnterState(State_EnterLevel, EnterEnterLevelState);
        m_FSM.addEnterState(State_Play, EnterPlayState);
        m_FSM.addEnterState(State_Solved, EnterSolvedState);
        m_FSM.addEnterState(State_Settlement, EnterSettlementState);
        m_FSM.addLeaveState(State_Settlement, LeaveSettlementState);
    }

    void EnterMainState()
    {
        UIManager.get().show("UIMain", 2f);
        GameGlobal.Instance().levelMgr.EnableLevels(false);
        CameraController.Instance().cam.orthographicSize = ReadyCameraSize;
    }

    void EnterEnterLevelState()
    {
        UIManager.get().find<UIMain>("UIMain").fadeOut(2f);
        GameGlobal.Instance().timer.AddTimer(2f, 2f, false, () => {
            ChangeGameState(GameMode.State_Play);
        });
    }

    void EnterPlayState()
    {
        GameGlobal.Instance().levelMgr.EnableLevels(true);
        GameGlobal.Instance().levelMgr.StartCurLevel();
    }

    void EnterSolvedState()
    {
        GameGlobal.Instance().levelMgr.FinishCurLevel();
        GameGlobal.Instance().timer.AddTimer(1f, 1f, false, () =>
        {
            ChangeGameState(GameMode.State_Settlement);
        });
    }

    void EnterSettlementState()
    {
        UIManager.get().show("UISettlement");
    }

    void LeaveSettlementState()
    {
        GameGlobal.Instance().levelMgr.LeaveCurLevel();
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
