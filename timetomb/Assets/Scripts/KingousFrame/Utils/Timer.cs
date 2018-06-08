using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KingousFramework
{
    public class Timer
    {
        List<TimerTask> m_Tasks = new List<TimerTask>();
        List<TimerTask> m_TasksToRemove = new List<TimerTask>();

        public delegate void TimerCallback();
        public class TimerTask
        {
            public float interval = 0;
            public bool isLoop = false;
            public float during = 0;
            public bool isAlive = true;
            public TimerCallback callback = null;

            float m_AliveDuring = 0;
            float m_IntervalDuring = 0;
            public TimerTask(float during, float interval, bool isLoop, TimerCallback callback)
            {
                this.during = during;
                this.interval = interval;
                this.isLoop = isLoop;
                this.callback = callback;
            }

            public void Tick(float dt)
            {
                if (!isAlive)
                    return;

                m_AliveDuring += dt;
                m_IntervalDuring += dt;
                if (m_IntervalDuring > interval)
                {
                    if (callback != null)
                        callback();
                }
                if (m_AliveDuring >= during)
                {
                    isAlive = false;
                }
            }
        }

        public void AddTimer(float during, float interval, bool isLoop, TimerCallback callback)
        {
            m_Tasks.Add(new TimerTask(during, interval, isLoop, callback));
        }

        public void Tick(float dt)
        {
            int cnt = m_Tasks.Count;
            for (int i = 0; i < cnt; i++)
            {
                var item = m_Tasks[i];
                if (item.isAlive)
                    item.Tick(dt);
                else
                    m_TasksToRemove.Add(item);
            }

            cnt = m_TasksToRemove.Count;
            for (int i = 0; i < cnt; i++)
            {
                m_Tasks.Remove(m_TasksToRemove[i]);
            }
        }
    }
}
