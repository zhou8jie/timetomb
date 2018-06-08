using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    public new Camera camera { get { return _camera; } }
    [SerializeField]
    private Canvas _canvas;
    public Canvas canvas { get { return _canvas; } }

    void Awake()
    {
        _camera = _camera ?? GetComponentInChildren<Camera>();
        _canvas = _canvas ?? GetComponentInChildren<Canvas>();
    }
}
