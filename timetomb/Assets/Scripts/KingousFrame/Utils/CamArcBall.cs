using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamArcBall : IInputEventHandler {

    private Transform m_Target;
    private Transform m_Self;

    float m_Dis = 0;
    Vector3 m_PressPos = Vector3.zero;

    public CamArcBall(Transform target, Transform self)
    {
        m_Target = target;
        m_Self = self;

        m_Dis = (m_Self.transform.position - m_Target.transform.position).magnitude;
    }

    public void OnPressDown(Vector3 pos)
    {
        m_PressPos = pos;
    }

    public void OnReleaseUp(Vector3 pos)
    {
    }

    public void OnDrag(Vector3 pos)
    {
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
        
        axis.Normalize();

        m_Self.transform.Rotate(axis, -angle, Space.World);

        m_Self.transform.position = m_Target.transform.position + m_Self.transform.forward * (-m_Dis);
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
