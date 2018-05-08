using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHelper
{
    public static Transform FindChildRecursive(Transform trans, string name)
    {
        Transform ret = null;
        for (int i = 0; i < trans.childCount; i++)
        {
            var item = trans.GetChild(i);
            if (item.name == name)
            {
                ret = item;
                break;
            }
            var deep = FindChildRecursive(item, name);
            if (deep != null)
            {
                ret = deep;
                break;
            }
        }
        return ret;
    }
    
    public static Vector3 RotateTowards(Vector3 from, Vector3 to, float angle)
    {
        return Vector3.RotateTowards(from, to, Mathf.Deg2Rad * angle, 0);
    }

    public static void IdentityLocalTransform(Transform trans)
    {
        if (trans == null)
            return;

        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    public static bool IsInCameraView(Vector3 pos, Camera camera)
    {
        Vector3 projPos = camera.WorldToScreenPoint(pos);
        if (projPos.x < 0 || projPos.x > camera.pixelWidth || projPos.y < 0 || projPos.y > camera.pixelHeight)
            return false;
        return true;
    }

}
