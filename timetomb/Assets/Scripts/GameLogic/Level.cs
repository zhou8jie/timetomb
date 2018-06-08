using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour, IInputEventHandler
{
    public Stars theStars = null;
    private ArcBall m_ArcBall = null;

    void Awake()
    {
        m_ArcBall = new ArcBall(this.transform);
    }

    public float GetFitRate(float delta)
    {
        var angle = Quaternion.Angle(theStars.transform.rotation, Quaternion.Euler(theStars.targetEuler));
        if (angle < delta)
        {
            return 0;
        }
        return angle / 360f;
    }

    public bool IsSolved()
    {
        return false;
    }

    public void Begin()
    {
        this.gameObject.SetActive(true);
    }

    public void OnPressDown(Vector3 pos)
    {
        if (m_ArcBall != null)
            m_ArcBall.OnPressDown(pos);
    }

    public void OnReleaseUp(Vector3 pos)
    {
        if (m_ArcBall != null)
            m_ArcBall.OnReleaseUp(pos);
    }

    public void OnDrag(Vector3 pos)
    {
        if (m_ArcBall != null)
            m_ArcBall.OnDrag(pos);
    }
}
