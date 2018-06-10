using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KingousFramework
{

    public delegate void EnterState();
    public delegate void UpdateState();
    public delegate void LeaveState();


    public class FSM
    {
        Dictionary<string, EnterState> m_enterStates = new Dictionary<string, EnterState>();
        Dictionary<string, UpdateState> m_updateStates = new Dictionary<string, UpdateState>();
        Dictionary<string, LeaveState> m_leaveStates = new Dictionary<string, LeaveState>();

        string m_curState = "";

        public bool addEnterState(string name, EnterState state)
        {
            if (state == null)
                return false;

            if (m_enterStates.ContainsKey(name))
                return false;
            m_enterStates[name] = state;
            return true;
        }

        public bool addUpdateState(string name, UpdateState state)
        {
            if (state == null)
                return false;

            if (m_updateStates.ContainsKey(name))
                return false;
            m_updateStates[name] = state;
            return true;
        }

        public bool addLeaveState(string name, LeaveState state)
        {
            if (state == null)
                return false;

            if (m_leaveStates.ContainsKey(name))
                return false;

            m_leaveStates[name] = state;
            return true;
        }

        public bool changeToState(string state)
        {
            if (string.IsNullOrEmpty(state))
                return false;
            if (!m_enterStates.ContainsKey(state))
                return false;
            if (m_curState == state)
                return false;

            if (!string.IsNullOrEmpty(m_curState))
            {
                if (m_leaveStates.ContainsKey(m_curState))
                    m_leaveStates[m_curState]();
            }

            m_enterStates[state]();
            m_curState = state;
            return true;
        }

        public void updateState()
        {
            if (!string.IsNullOrEmpty(m_curState) && m_updateStates.ContainsKey(m_curState))
            {
                m_updateStates[m_curState]();
            }
        }
    }
}
