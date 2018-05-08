using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    public UILayerId layerId { get { return _layerId; } }

    public Canvas canvas { get { return _canvas; } }
    public GraphicRaycaster raycaster { get { return _raycaster; } }

    public bool isShow { get { return _isShow; } }

    [SerializeField]
    private UILayerId _layerId;

    private Canvas _canvas;
    private GraphicRaycaster _raycaster;

    private bool _isShow = false;

    public void initialize()
    {
        _canvas = gameObject.AddComponent<Canvas>();
        _raycaster = gameObject.AddComponent<GraphicRaycaster>();

        onCreate();
    }

    public void show(bool needAdd=true)
    {
        if (needAdd)
            UIManager.get().add(this);
        onShow();
    }

    public void hide()
    {
        UIManager.get().remove(this);
        onHide();
        Object.Destroy(gameObject);
    }

    protected virtual void onCreate() { }
    protected virtual void onShow() { }
    protected virtual void onHide() { }

    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    protected virtual void OnDestroy() { }
}
