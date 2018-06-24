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
        GameModule.Instance().curLevelIdx = 0;
    }

    public void EnableLevels(bool enable)
    {
        gameObject.SetActive(enable);
        if (enable)
        {
            var curLevel = GameModule.Instance().curLevelIdx;
            for (int i = 0; i < m_Levels.Length; i++)
            {
                m_Levels[i].gameObject.SetActive(false);
            }
            m_Levels[curLevel].gameObject.SetActive(true);
        }
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
        m_CurLevel.End();
    }

    public void LeaveCurLevel()
    {
        m_CurLevel.Reset();
        GameGlobal.Instance().inputMgr.RemoveEventHandler(m_CurLevel);
        m_CurLevel = null;

        GameModule.Instance().curLevelIdx++;
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
                mode.ChangeGameState(GameMode.State_Solved);
            }
        }
        else if (curState == GameMode.State_Solved)
        {

        }
    }
}
