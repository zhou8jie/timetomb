using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingousFramework;
using DG.Tweening;

public class Level : MonoBehaviour, IInputEventHandler
{
    public Vector3 initEuler;
    public Vector3 targetEuler;

    protected ArcBall m_ArcBall = null;

    protected Transform m_AllStars = null;
    protected List<Transform> m_Stars = new List<Transform>();
    protected List<Material> m_StarsMat = new List<Material>();
    protected List<Transform> m_StarLines = new List<Transform>();
    protected List<Material> m_StarLinesMat = new List<Material>();
    protected List<Transform> m_StarSlots = new List<Transform>();
    protected List<Material> m_StarSlotsMat = new List<Material>();
    protected Transform m_StarBG = null;
    
    public static readonly string State_Beginning = "Beginning";
    public static readonly string State_Solving = "Solving";
    public static readonly string State_Solved = "Solved";

    protected FSM m_FSM = new FSM();

    void Awake()
    {
        var objStars = gameObject.FindChild("Stars");
        m_AllStars = objStars.transform;
        var cnt = objStars.transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            var star = objStars.transform.GetChild(i);
            var billboard = star.gameObject.AddComponent<Billboard>();
            billboard.target = CameraController.Instance().cam.transform;
            m_Stars.Add(star);
            m_StarsMat.Add(star.GetComponent<MeshRenderer>().material);
        }

        var objStarLines = gameObject.FindChild("StarLines");
        cnt = objStarLines.transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            m_StarLines.Add(objStarLines.transform.GetChild(i));
        }

        var objStarSlots = gameObject.FindChild("StarSlots");
        cnt = objStarSlots.transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            m_StarSlots.Add(objStarSlots.transform.GetChild(i));
            var mat = m_StarSlots[i].GetComponent<MeshRenderer>().material;
            m_StarSlotsMat.Add(mat);
        }

        m_StarBG = gameObject.FindChild("StarBg/Bg").transform;
        ToolHelper.SetMaterialParamsFloat(m_StarBG.gameObject, "_Intensity", 0);


        m_ArcBall = new ArcBall(m_AllStars);
        m_ArcBall.lockAxisY = true;
        m_ArcBall.enable = false;
    }

    private void Update()
    {
        var cnt = m_StarSlotsMat.Count;
        for (int i = 0; i < cnt; i++)
        {
            //var dis = _GetViewDisBetweenStarAndSlot(i);
            //m_StarSlotsMat[i].SetFloat("_Intensity", 0.5f);
        }
    }

    float _GetViewDisBetweenStarAndSlot(int i)
    {
        var starPos = m_Stars[i].position;
        var slotPos = m_StarSlots[i].position;
        starPos.z = 0;
        slotPos.z = 0;
        return ToolHelper.GetManhattanDis(starPos, slotPos);
    }

    public float GetFitRate(float delta)
    {
        var angle = Quaternion.Angle(m_AllStars.transform.rotation, Quaternion.Euler(this.targetEuler));
        if (angle < delta)
        {
            return 0;
        }
        return angle / 360f;
    }

    public bool IsSolved()
    {
        if (GetFitRate(5f) == 0)
            return true;
        return false;
    }

    public void Begin()
    {
        this.gameObject.SetActive(true);
        m_AllStars.rotation = Quaternion.Euler(this.initEuler);

        m_FSM.addEnterState(State_Beginning, EnterBeginning);
        m_FSM.addEnterState(State_Solving, EnterSolving);
        m_FSM.addEnterState(State_Solved, EnterSolved);
        m_FSM.changeToState(State_Beginning);
    }

    public void End()
    {
        m_FSM.changeToState(State_Solved);
    }

    void EnterBeginning()
    {
        // 星光槽闪两下，保持在一个位置
        foreach (var item in m_StarSlotsMat)
        {
            item.SetFloat("_Intensity", 0.3f);
            var seq = DOTween.Sequence();
            var tw1 = item.DOFloat(0.5f, "_Intensity", 2f);
            var tw2 = item.DOFloat(0.3f, "_Intensity", 2f);
            seq.Append(tw1);
            seq.Append(tw2);
            seq.SetLoops(-1);
        }
        // 星星从小到大的蹦出来，慢慢变暗，蹦的时候发出声音
        foreach (var item in m_Stars)
        {
            item.transform.localScale = Vector3.zero;
            var tw = item.transform.DOScale(Vector3.one, 3f);
            tw.SetEase(Ease.InElastic);
            tw.SetDelay(Random.Range(0, 0.5f));
        }
        // 延迟进入可操作状态
        GameGlobal.Instance().timer.AddTimer(5f, 5f, false, () => {
            m_FSM.changeToState(State_Solving);
        });
    }

    void EnterSolving()
    {
        // 随着旋转
        m_ArcBall.enable = true;
        DOTween.PauseAll();
    }

    void EnterSolved()
    {
        // 星光槽闪并且向外扩散
        // 星光背景显现出来
        // 星空背景颜色变化
        // 播放通关音乐
        m_ArcBall.enable = false;
        for (int i = 0; i < m_StarSlots.Count; i++)
        {
            var mat = m_StarSlotsMat[i];
            var trans = m_StarSlots[i];
            mat.SetFloat("_Intensity", 1f);
            var tw1 = mat.DOFloat(0f, "_Intensity", 2f);
            tw1.SetLoops(-1);
            var tw2 = trans.DOScale(2f, 2f);
            tw2.SetLoops(-1);
        }
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
