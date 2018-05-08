using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxShell
{
    public int id = 0;
    public float during = 0;
    public GameObject res = null;
    public bool active = false;
    public bool loop = false;
    public Transform attTrans = null;
    public void Update()
    {
        if (attTrans != null && res != null)
        {
            res.transform.position = attTrans.position;
            res.transform.rotation = attTrans.rotation;
            res.transform.localScale = Vector3.one;
        }
    }
    public void Reset()
    {
        during = 0;
        if (res != null)
            CacheCenter.Instance().RecycleRes(res);
        res = null;
        active = false;
        loop = false;
        attTrans = null;
    }
}

public class FxManager
{
    public static int MAX_COUNT = 256;

    private List<FxShell> m_fxPool = new List<FxShell>(MAX_COUNT);    // 同时最大数量
    private int m_curIdx = 0;
    private static int s_ID = 0;
    private static FxManager s_inst = null;
    public static FxManager Instance()
    {
        if (s_inst == null)
            s_inst = new FxManager();
        return s_inst;
    }
    public void Release()
    {
        int count = m_fxPool.Count;
        for (int i = 0; i < count; i++)
        {
            m_fxPool[i].Reset();
        }
        m_fxPool.Clear();
        s_inst = null;
    }
    private FxManager()
    {
        for (int i = 0; i < MAX_COUNT; i++)
        {
            FxShell shell = new FxShell();
            m_fxPool.Add(shell);
        }
    }
    public int PlayAtPos(string path, Vector3 pos, Quaternion rot, float during)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("res neednot be null...");
            return -1;
        }
        FxShell shell = _findAFreeShell();
        if (shell == null)
        {
            Debug.LogWarning("cant find a free fx...");
            return -1;
        }
        var res = CacheCenter.Instance().GetRes(path);
        if (res == null)
        {
            Debug.LogWarning("cant find a fx named : " + path);
            return -1;
        }

        res.transform.position = pos;
        res.transform.rotation = Quaternion.identity;
        res.transform.localScale = Vector3.one;

        s_ID++;
        shell.res = res;
        shell.during = during;
        shell.active = true;
        shell.id = s_ID;
        if (during <= 0)
            shell.loop = true;

        return s_ID;
    }
    public int PlayLink(string path, Transform oriTrans, Transform tarTrans, float during)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("res neednot be null...");
            return -1;
        }
        if (oriTrans == null || tarTrans == null)
        {
            Debug.LogWarning("trans neednot be null...");
            return -1;
        }
        FxShell shell = _findAFreeShell();
        if (shell == null)
        {
            Debug.LogWarning("cant find a free fx...");
            return -1;
        }

        var resShell = CacheCenter.Instance().GetResShell(path);
        if (resShell == null)
        {
            Debug.LogWarning("cant find a fx named : "+ path);
            return -1;
        }

        s_ID++;
        shell.res = resShell.res;
        shell.during = during;
        shell.active = true;
        shell.id = s_ID;

        if (during <= 0)
            shell.loop = true;

        return s_ID++;
    }
    public int PlayAsChild(string path, Transform trans, Vector3 localPos, Quaternion localRot, float during, bool attached, bool uniupdate)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("path cant be null...");
            return -1;
        }
        if (trans == null)
        {
            Debug.LogWarning("trans cant be null...");
            return -1;
        }
        FxShell shell = _findAFreeShell();
        if (shell == null)
        {
            Debug.LogWarning("cant find a free fx...");
            return -1;
        }

        var res = CacheCenter.Instance().GetRes(path);
        if (res == null)
        {
            Debug.LogWarning("cant find a fx named : " + path);
            return -1;
        }

        if (attached)
        {
            if (!uniupdate)
                res.transform.parent = trans;
            else
                res.transform.parent = null;
        }
        else
            res.transform.parent = trans;

        res.transform.localPosition = localPos;
        res.transform.localRotation = Quaternion.identity;
        res.transform.localScale = Vector3.one;

        s_ID++;
        shell.res = res;
        shell.during = during;
        shell.active = true;
        shell.id = s_ID;
        if (attached && uniupdate)
            shell.attTrans = trans;
        if (!attached)
        {
            res.transform.parent = null;
            if (!res.activeInHierarchy)
                res.SetActive(true);
        }

        if (during <= 0)
            shell.loop = true;

        return s_ID++;
    }
    public void Stop(int id)
    {
        var fx = m_fxPool.Find((FxShell shell) => shell.id == id);
        if (fx == null)
            return;

        fx.Reset();
    }
    public void Update(float dt)
    {
        int count = m_fxPool.Count;
        for (int i = 0; i < count; i++)
        {
            if (!m_fxPool[i].active)
                continue;
            if (m_fxPool[i].loop)
                continue;
            m_fxPool[i].during -= dt;
            m_fxPool[i].Update();

            if (m_fxPool[i].during <= 0)
                m_fxPool[i].Reset();
        }
    }
    private FxShell _findAFreeShell()
    {
        int count = 0;
        do
        {
            count++;
            int idx = m_curIdx % MAX_COUNT;
            m_curIdx++;
            if (!m_fxPool[idx].active)
                return m_fxPool[idx];
        } while (count < MAX_COUNT);
        return null;
    }
}
