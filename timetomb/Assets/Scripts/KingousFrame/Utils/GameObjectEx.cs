using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectEx
{
    public static GameObject FindChild(this GameObject thisObj, string objectPath)
    {
        var child = thisObj.transform.Find(objectPath);
        if (child != null)
            return child.gameObject;
        return null;
    }

    public static T GetChildComponent<T>(this GameObject thisObj, string objectPath) where T : Component
    {
        var child = thisObj.FindChild(objectPath);
        if (child != null)
            return child.GetComponent<T>();
        return null;
    }
}
