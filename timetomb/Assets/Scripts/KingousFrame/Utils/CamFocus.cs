using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFocus : IInputEventHandler
{

    private Transform m_Target;
    private Transform m_Self;
    private float m_Rate = 1f;

    float m_Dis = 0;
    Vector3 m_PressPos = Vector3.zero;

    public CamFocus(Transform target, Transform self, float rate)
    {
        m_Target = target;
        m_Self = self;
        m_Rate = rate;

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
        Vector3 deltaPos = pos - m_PressPos;
        deltaPos.x /= Screen.width;
        deltaPos.y /= Screen.height;
        deltaPos.z = 1;

        deltaPos.x *= m_Rate;
        deltaPos.y *= m_Rate;

        Vector3 dir = deltaPos.normalized;

        Vector3 p1 = Vector3.forward;
        Vector3 p2 = dir;

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

        m_Self.transform.Rotate(axis, angle, Space.World);

        m_Self.transform.position = m_Target.transform.position + m_Self.transform.forward * (-m_Dis);
        m_Self.transform.LookAt(m_Target, Vector3.up);
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
