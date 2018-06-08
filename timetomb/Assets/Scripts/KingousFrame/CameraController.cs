using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ECameraShockType
{
    Swing,
    ZoomIn,
    ZoomOut,
    Shake,
    Recoil,
}

public class CameraData
{
    public Vector3 camRockerPos = Vector3.zero;
    public Vector3 camEuler = Vector3.zero;

    public CameraData(Vector3 pos, Vector3 euler)
    {
        camRockerPos = pos;
        camEuler = euler;
    }
}

public class CameraController : MonoBehaviour, IInputEventHandler {

    private Transform m_Rocker = null;
    private Camera m_Camera = null;
    private Transform m_Shake = null;

    private CamArcBall m_Arcball = null;
    private CamFocus m_Focus = null;

    public static CameraData PREPARE_CAM = new CameraData(new Vector3(-20, 50, -55), new Vector3(45, 0, 0));
    public static CameraData BUILD_CAM = new CameraData(new Vector3(-20, 70, -30), new Vector3(70, 0, 0));
    public static CameraData BATTLE_CAM = new CameraData(new Vector3(-20, 45, -40), new Vector3(50, 0, 0));
    public static CameraData SKILL_CAM = new CameraData(new Vector3(-20, 60, -24), new Vector3(70, 0, 0));

    private static CameraController s_Instance = null;
    public static CameraController Instance()
    {
        return s_Instance;
    }

    public Camera cam
    {
        get { return m_Camera; }
    }

    void Awake()
    {
        s_Instance = this;

        m_Rocker = transform;
        m_Shake = transform.Find("CamShake");
        m_Camera = transform.GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {

    }

    public void ChangeToArcballCamera(Transform target)
    {
        if (m_Arcball == null)
        {
            m_Arcball = new CamArcBall(target, m_Rocker);
        }
    }

    public void ChangeToFocusCamera(Transform target, float rate)
    {
        if (m_Focus == null)
        {
            m_Focus = new CamFocus(target, m_Rocker, rate);
        }
    }

    public void ChangeToPresetCamera(CameraData data, float during=1f)
    {
        var pos = data.camRockerPos;
        var euler = data.camEuler;
        pos.x = transform.position.x;
        var tweener = m_Rocker.transform.DOMove(pos, during);
        tweener.SetEase(Ease.InOutQuart);

        tweener = m_Camera.transform.DORotate(euler, during);
        tweener.SetEase(Ease.InOutQuart);
    }

    public void MoveCamera(Vector3 deltaPos)
    {
        m_Rocker.transform.position += deltaPos;
    }

    public void PlayShock(ECameraShockType type)
    {
        switch (type)
        {
            case ECameraShockType.Swing:
                DoSwing();
                break;
            case ECameraShockType.ZoomIn:
                DoZoomIn();                
                break;
            case ECameraShockType.ZoomOut:
                DoZoomOut();
                break;
            case ECameraShockType.Shake:
                DoShake();
                break;
            case ECameraShockType.Recoil:
                DoRecoil();
                break;
            default:
                break;
        }
    }
    void DoShake()
    {
        var seq = DOTween.Sequence();
        var tween = m_Shake.transform.DOLocalMoveX(-0.5f, 0.02f);
        tween.SetEase(Ease.InBounce);
        seq.Append(tween);
        tween = m_Shake.transform.DOLocalMoveY(-1f, 0.04f);
        seq.Append(tween);
        tween = m_Shake.transform.DOLocalMoveX(0.5f, 0.04f);
        seq.Append(tween);
        tween = m_Shake.transform.DOLocalMoveY(0f, 0.04f);
        seq.Append(tween);
        tween = m_Shake.transform.DOLocalMoveX(0f, 0.02f);
        seq.Append(tween);
    }
    void DoZoomIn()
    {
        var seq = DOTween.Sequence();
        var tween = m_Camera.transform.DOLocalMoveZ(0.5f, 0.05f);
        tween.SetLoops(2, LoopType.Yoyo);
        tween.SetEase(Ease.InBounce);
        seq.Append(tween);
    }

    void DoZoomOut()
    {
        var seq = DOTween.Sequence();
        var tween = m_Camera.transform.DOLocalMoveZ(-0.5f, 0.05f);
        tween.SetLoops(2, LoopType.Yoyo);
        tween.SetEase(Ease.InBounce);
        seq.Append(tween);
    }

    void DoRecoil()
    {
        var seq = DOTween.Sequence();
        var tween = m_Shake.transform.DOLocalMoveY(-0.3f, 0.05f);
        tween.SetLoops(2, LoopType.Yoyo);
        tween.SetEase(Ease.InBounce);
        seq.Append(tween);
    }

    void DoSwing()
    {
        var seq = DOTween.Sequence();
        var tween = m_Shake.transform.DOLocalRotate(new Vector3(0, 0, -1f), 0.05f);
        tween.SetLoops(2, LoopType.Yoyo);
        tween.SetEase(Ease.InBounce);
        seq.Append(tween);
        tween = m_Shake.transform.DOLocalRotate(new Vector3(0, 0, 1f), 0.05f);
        tween.SetLoops(2, LoopType.Yoyo);
        tween.SetEase(Ease.InBounce);
        seq.Append(tween);
    }

    public void OnPressDown(Vector3 pos)
    {
        if (m_Arcball != null)
            m_Arcball.OnPressDown(pos);
        if (m_Focus != null)
            m_Focus.OnPressDown(pos);
    }

    public void OnReleaseUp(Vector3 pos)
    {
        if (m_Arcball != null)
            m_Arcball.OnReleaseUp(pos);
        if (m_Focus != null)
            m_Focus.OnReleaseUp(pos);
    }

    public void OnDrag(Vector3 pos)
    {
        if (m_Arcball != null)
            m_Arcball.OnDrag(pos);
        if (m_Focus != null)
            m_Focus.OnDrag(pos);
    }
}
