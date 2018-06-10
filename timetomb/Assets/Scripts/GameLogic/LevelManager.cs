using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    Level m_CurLevel = null;
    Level[] m_Levels = null;

    void Awake()
    {
        m_Levels = GetComponentsInChildren<Level>();
    }

    public void EnableLevels(bool enable)
    {
        gameObject.SetActive(enable);
    }

    public void StartCurLevel()
    {
        int lvlIdx = GameModule.Instance().curLevelIdx;
        m_CurLevel = m_Levels[lvlIdx];
        m_CurLevel.Begin();
        GameGlobal.Instance().inputMgr.AddEventHandler(m_CurLevel);
    }

    public void FinishCurLevel()
    {
        if (m_CurLevel == null)
            return;
        GameGlobal.Instance().inputMgr.RemoveEventHandler(m_CurLevel);
        m_CurLevel = null;
    }

    public void Update()
    {
        var mode = GameGlobal.Instance().mode;
        if (mode == null)
            return;

        var curState = mode.curState;
        if (curState == GameMode.State_Play)
        {
            if (m_CurLevel.IsSolved())
            {
                Debug.LogError("solved...");
                mode.ChangeGameState(GameMode.State_Solved);
            }
        }
        else if (curState == GameMode.State_Solved)
        {

        }
    }
}
