using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformEx 
{
    public static T GetChildComponent<T>(this Transform thisObj, string objectPath) where T : Component
    {
        var child = thisObj.Find(objectPath);
        if (child != null)
            return child.GetComponent<T>();
        return null;
    }
}
