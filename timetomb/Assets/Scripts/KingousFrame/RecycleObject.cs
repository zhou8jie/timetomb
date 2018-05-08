using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleObject : MonoBehaviour 
{
    public float during = 0;

    float m_timer = 0;
    bool m_isRecycled = false;
	// Use this for initialization
	void Start () 
    {
        m_timer = 0;
        m_isRecycled = false;
	}

    void OnEnable()
    {
        m_timer = 0;
        m_isRecycled = false;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (m_isRecycled)
            return;

        m_timer += Time.deltaTime;
        if (m_timer > during)
        {
            m_isRecycled = true;
            CacheCenter.Instance().RecycleRes(this.gameObject);
        }	
	}
}
