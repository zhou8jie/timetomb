using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheCenter
{
    public static int LOD = 0;

    protected Dictionary<string, ObjectPool> m_pools = new Dictionary<string, ObjectPool>();
    protected Dictionary<int, ObjectPool> m_activeObjects = new Dictionary<int,ObjectPool>();
    protected Transform m_root;
    protected static CacheCenter s_inst = null;
    private CacheCenter()
    {
        GameObject root = new GameObject("CacheCenter");
        m_root = root.transform;
    }
    public static CacheCenter Instance()
    {
        if (s_inst == null)
            s_inst = new CacheCenter();
        return s_inst;
    }
    public void Release()
    {
        GameObject.Destroy(Instance().m_root.gameObject);
        foreach (var item in m_pools)
        {
            item.Value.UnloadAsset();
        }
        s_inst = null;
    }
    public void Clear()
    {
        var clearList = new List<string>();
        foreach (var item in m_pools)
        {
            item.Value.Clear();
            if (!item.Value.manual)
                clearList.Add(item.Key);
        }

        foreach (var item in m_activeObjects)
        {
            item.Value.Clear();
        }

        foreach (var item in clearList)
        {
            m_pools.Remove(item);
        }
        m_activeObjects.Clear();
    }
    public GameObject LoadGameRes(string path)
    {
        var res = Resources.Load(path) as GameObject;
        if (res == null)
        {
            Debug.LogErrorFormat("Cannot find res named {0}", path);
            return null;
        }
        return res;
    }
    public void InitResPool(string path, GameObject res, bool manual)
    {
        if (string.IsNullOrEmpty(path) || res == null)
        {
            Debug.LogWarning("cant init res pool with null path or null res..");
            return;
        }
        if (m_pools.ContainsKey(path))
        {
            Debug.LogWarning("res pool already exist a path named : " + path);
            return;
        }
        ObjectPool pool = new ObjectPool(path, res, manual);
        m_pools[path] = pool;
    }
    public void CacheRes(string path, int count)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("cant cache res pool with null path ..");
            return;
        }
        if (!m_pools.ContainsKey(path))
        {
            Debug.LogWarning("res pool named doesnt exist...");
            return;
        }
        m_pools[path].AddObject(count);
    }
    public GameObject GetResWithSize(string path, float sz)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("cant get res pool with null path ..");
            return null;
        }
        if (!m_pools.ContainsKey(path))
        {
            Debug.LogWarning("res pool doesnt exist : " + path);
            return null;
        }
        var shell = m_pools[path].GetResShell();
        int id = shell.res.GetInstanceID();
        m_activeObjects[id] = m_pools[path];
        return shell.res;
    }
    public GameObject GetRes(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("cant get res pool with null path ..");
            return null;
        }
        if (!m_pools.ContainsKey(path))
        {
            Debug.LogWarning("res pool doesnt exist : " + path);
            return null;
        }

        var shell = m_pools[path].GetResShell();
        int id = shell.res.GetInstanceID();
        m_activeObjects[id] = m_pools[path];
        return shell.res;
    }
    public ResShell GetResShell(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("cant get res pool with null path ..");
            return null;
        }
        if (!m_pools.ContainsKey(path))
        {
            Debug.LogWarning("res pool named doesnt exist...");
            return null;
        }
        var ret = m_pools[path].GetResShell();
        int id = ret.res.GetInstanceID();

        m_activeObjects[id] = m_pools[path];
        return ret;
    }
    public void RecycleRes(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("cant recycle res with null path ..");
            return;
        }
        int id = obj.GetInstanceID();
        if (!m_activeObjects.ContainsKey(id))
        {
            Debug.LogWarning("cant recycle res that didnt get from cache center named : " + obj.name);
            return;
        }
        m_activeObjects[id].RecycleObject(obj);
    }
}
