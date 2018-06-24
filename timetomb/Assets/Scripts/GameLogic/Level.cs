using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingousFramework;
using DG.Tweening;

public class Level : MonoBehaviour, IInputEventHandler
{
    public Vector3 initEuler;
    public Vector3 targetEuler;
    public bool lockX = false;
    public bool lockY = false;

    protected ArcBall m_ArcBall = null;

    protected Transform m_AllStars = null;
    protected List<Transform> m_Stars = new List<Transform>();
    protected List<Material> m_StarsMat = new List<Material>();
    protected List<LineRenderer> m_StarLines = new List<LineRenderer>();
    protected List<Material> m_StarLinesMat = new List<Material>();
    protected List<Transform> m_StarSlots = new List<Transform>();
    protected List<Material> m_StarSlotsMat = new List<Material>();
    protected Transform m_StarBG = null;
    protected Material m_StarBGMat = null;

    public static readonly string State_Beginning = "Beginning";
    public static readonly string State_Solving = "Solving";
    public static readonly string State_Solved = "Solved";

    protected FSM m_FSM = new FSM();

    void Start()
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
            var rd = objStarLines.transform.GetChild(i).GetComponent<LineRenderer>();
            m_StarLines.Add(rd);
            var mat = rd.material;
            m_StarLinesMat.Add(mat);
            mat.SetFloat("_Intensity", 0);
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
        m_StarBGMat = m_StarBG.GetComponent<MeshRenderer>().material;
        m_StarBGMat.SetFloat("_Intensity", 0);


        m_ArcBall = new ArcBall(m_AllStars);
        m_ArcBall.lockAxisX = lockX;
        m_ArcBall.lockAxisY = lockY;
        m_ArcBall.enable = false;
    }

    private void Update()
    {
        m_FSM.updateState();
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
        m_FSM.addUpdateState(State_Solving, UpdateSolving);
        m_FSM.addEnterState(State_Solved, EnterSolved);
        m_FSM.changeToState(State_Beginning);
    }

    public void End()
    {
        m_FSM.changeToState(State_Solved);
    }

    public void Reset()
    {
        DOTween.KillAll();
        m_StarBGMat.SetFloat("_Intensity", 0);
        foreach (var item in m_StarLinesMat)
        {
            item.SetFloat("_Intensity", 0f);
        }

        foreach (var item in m_StarSlots)
        {
            item.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
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
            var delay = Random.Range(0, 0.5f);
            tw.SetDelay(delay);
            GameGlobal.Instance().timer.AddTimer(delay+2f, delay+2f, false, () => {
                GameGlobal.Instance().soundMgr.PlayFx("sfx_star_born");
            });

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
        DOTween.KillAll();

        var cnt = m_StarLines.Count;
        for (int i = 0; i < cnt; i++)
        {
            var line = m_StarLines[i];
            var pos1 = m_Stars[i].position;
            var pos2 = m_Stars[i + 1].position;
            line.SetPosition(0, pos1 + new Vector3(0, 0, 10f));
            line.SetPosition(1, pos2 + new Vector3(0, 0, 10f));
            m_StarLinesMat[i].DOFloat(0.7f, "_Intensity", 1f);
        }
     }

    void UpdateSolving()
    {
        var cnt = m_StarLines.Count;
        for (int i = 0; i < cnt; i++)
        {
            var line = m_StarLines[i];
            var pos1 = m_Stars[i].position;
            var pos2 = m_Stars[i + 1].position;
            line.SetPosition(0, pos1 + new Vector3(0, 0, 10f));
            line.SetPosition(1, pos2 + new Vector3(0, 0, 10f));
        }
    }

    void EnterSolved()
    {
        // 不受控制，自动旋转到最佳位置
        m_ArcBall.enable = false;
        {
            var tw = transform.DORotate(this.targetEuler, 2f);
            // 播放通关音乐

            tw.OnComplete(() =>
            {
                // 拉远摄像机
                CameraController.Instance().cam.DOOrthoSize(GameMode.SolvedCameraSize, 20f);
            });
        }
        // 星光槽闪并且向外扩散
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

        // 星座颜色变化
        {
            var tw = m_StarBGMat.DOFloat(0.5f, "_Intensity", 3f);
            tw.OnComplete(() =>
            {
                m_StarBGMat.SetFloat("_Intensity", 0.5f);
                var seq1 = DOTween.Sequence();
                var tw1 = m_StarBGMat.DOFloat(0.6f, "_Intensity", 2f);
                var tw2 = m_StarBGMat.DOFloat(0.5f, "_Intensity", 2f);
                seq1.Append(tw1);
                seq1.Append(tw2);
                seq1.SetLoops(-1);
            });
        }

        GameGlobal.Instance().soundMgr.PlayFx("sfx_victory");
    }

    public void OnPressDown(Vector3 pos)
    {
        if (m_ArcBall != null)
            m_ArcBall.OnPressDown(pos);

        GameGlobal.Instance().soundMgr.PlayFx("sfx_ding");
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
