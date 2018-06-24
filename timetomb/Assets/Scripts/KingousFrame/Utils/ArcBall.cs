using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcBall : IInputEventHandler {

    Transform m_Target = null;
    Vector3 m_PressPos = Vector3.zero;

    public bool lockAxisY { set; get; }
    public bool lockAxisX { set; get; }
    public bool enable { set; get; }

    public ArcBall(Transform target)
    {
        m_Target = target;
        lockAxisX = false;
        lockAxisY = false;
        enable = true;
    }

    public void OnPressDown(Vector3 pos)
    {
        if (!enable)
            return;

        m_PressPos = pos;
    }

    public void OnReleaseUp(Vector3 pos)
    {
        if (!enable)
            return;
    }

    public void OnDrag(Vector3 pos)
    {
        if (!enable)
            return;

        Vector3 p1 = GetBallPos(m_PressPos);
        Vector3 p2 = GetBallPos(pos);

        Vector3 axis = Vector3.zero;
        float angle = 0;
        if (p1 != p2)
        {
            axis = Vector3.Cross(p1, p2);
            angle = Vector3.Angle(p1, p2);
        }
        if (axis == Vector3.zero)
            return;

        if (lockAxisY)
            axis = Vector3.up * Mathf.Sign(axis.y);
        if (lockAxisX)
            axis = Vector3.right * Mathf.Sign(axis.x);
        
        axis.Normalize();

        m_Target.transform.Rotate(axis, angle, Space.Self);

        m_PressPos = pos;
    }

    Vector3 GetBallPos(Vector3 scrPos)
    {
        if (scrPos.x > Screen.width)
            scrPos.x = Screen.width;
        if (scrPos.y > Screen.height)
            scrPos.y = Screen.height;
            
        float x = scrPos.x / Screen.width * 2f - 1f;
        float y = scrPos.y / Screen.height * 2f - 1f;
        Vector3 ret = new Vector3(x, y, 0);
        float delta = 1f - x * x - y * y;
        if (delta < 0)
        {
            return ret.normalized;
        }
        else
        {
            float z = -Mathf.Sqrt(delta);
            ret.z = z;
            return ret;
        }
    }
}
