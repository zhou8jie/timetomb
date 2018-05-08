using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResShell
{
    public GameObject res;
    public TrailRenderer trail;
    public ResShell(GameObject res)
    {
        this.res = res;
        this.trail = res.GetComponentInChildren<TrailRenderer>();
    }
}

public class ObjectPool
{
    public string resPath
    {
        protected set;
        get;
    }
    public GameObject res
    {
        protected set;
        get;
    }
    public Transform root
    {
        protected set;
        get;
    }
    public bool manual
    {
        protected set;
        get;
    }

    private LinkedList<ResShell> m_unusedShells = new LinkedList<ResShell>();
    private Dictionary<int, ResShell> m_shells = new Dictionary<int, ResShell>();
    public ObjectPool(string resPath, GameObject res, bool manual)
    {
        if (string.IsNullOrEmpty(resPath) || res == null)
        {
            Debug.LogWarning("object pool create with a null path or null obj...");
            return;
        }
        this.resPath = resPath;
        this.res = res;
        this.manual = manual;

        this.root = new GameObject(res.name).transform;
        var centerRoot = GameObject.Find("/CacheCenter");
        if (centerRoot != null)
            root.transform.parent = centerRoot.transform;
    }
    public void AddObject(int count)
    {
        if (this.res == null)
        {
            Debug.LogWarning("object pool add object failed cause res : " + this.resPath + " is NULL..");
            return;
        }
        for (int i = 0; i < count; i++)
        {
            var item = GameObject.Instantiate(this.res);
            ToolHelper.IdentityLocalTransform(item.transform);
            item.SetActive(false);
            item.transform.SetParent(this.root);

            var shell = new ResShell(item);
            int id = item.GetInstanceID();
            m_shells[id] = shell;
            m_unusedShells.AddLast(shell);
        }
    }
    public GameObject GetObject()
    {
        GameObject ret = null;
        ResShell shell = null;
        if (m_unusedShells.Count == 0)
        {
            ret = GameObject.Instantiate(this.res);
            ToolHelper.IdentityLocalTransform(ret.transform);
            shell = new ResShell(ret);
            int id = ret.GetInstanceID();
            m_shells[id] = shell;
            Debug.LogWarning("object pool instantiate a obj in RUNTIME named : " + this.res);
        }
        else
        {
            shell = m_unusedShells.First.Value;
            ret = shell.res;
            m_unusedShells.RemoveFirst();
        }
        
        ret.transform.SetParent(null);
        ret.SetActive(true);
        if (shell.trail != null)
            shell.trail.Clear();

        return ret;
    }
    public void RecycleObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("object pool cant recycle a null obj cause this pool is for : " + this.res);
            return;
        }
        obj.transform.SetParent(this.root);
        ToolHelper.IdentityLocalTransform(obj.transform);
        obj.SetActive(false);
        int id = obj.GetInstanceID();
        if (!m_shells.ContainsKey(id))
        {
            Debug.LogWarning("object pool doesnt have a shell of obj named:" + obj.name);
            return;
        }
        var shell = m_shells[id];
        m_unusedShells.AddLast(shell);
    }
    public ResShell GetResShell()
    {
        ResShell shell = null;
        if (m_unusedShells.Count == 0)
        {
            var res = GameObject.Instantiate(this.res);
            ToolHelper.IdentityLocalTransform(res.transform);
            shell = new ResShell(res);
            int id = res.GetInstanceID();
            m_shells[id] = shell;
        }
        else
        {
            shell = m_unusedShells.First.Value;
            m_unusedShells.RemoveFirst();
        }
        
        shell.res.transform.SetParent(null);
        shell.res.SetActive(true);
        if (shell.trail != null)
            shell.trail.Clear();
        return shell;
    }
    public void Clear()
    {
        foreach (var item in m_unusedShells)
        {
            GameObject.Destroy(item.res);
        }
        m_unusedShells.Clear();
        m_shells.Clear();
        if (!this.manual)
        {
            this.res = null;
            this.root.parent = null;
            GameObject.Destroy(this.root.gameObject);
        }
        resPath = "";
    }

    public void UnloadAsset()
    {
        this.res = null;
    }
}
