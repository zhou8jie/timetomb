using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IInputEventHandler
{
    void OnPressDown(Vector3 pos);
    void OnReleaseUp(Vector3 pos);
    void OnDrag(Vector3 pos);
}

public class InputManager : MonoBehaviour {

    public enum EInputState
    {
        Invalid,
        Down,
        Up,
    }
    EInputState m_InputState = EInputState.Invalid;
    List<IInputEventHandler> m_EventHandleres = new List<IInputEventHandler>();

    public void AddEventHandler(IInputEventHandler handler)
    {
        m_EventHandleres.Add(handler);
    }
    public void RemoveEventHandler(IInputEventHandler handler)
    {
        m_EventHandleres.Remove(handler);
    }
	
	void Update () {
        if (m_EventHandleres.Count > 0)
            UpdateInput();
	}

    void UpdateInput()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.OSXEditor
            || Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.OSXPlayer
            || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                m_InputState = EInputState.Down;
                foreach (var item in m_EventHandleres)
                {
                    item.OnPressDown(Input.mousePosition);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (m_InputState != EInputState.Down)
                    return;

                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                m_InputState = EInputState.Up;
                foreach (var item in m_EventHandleres)
                {
                    item.OnReleaseUp(Input.mousePosition);
                }
            }

            if (m_InputState == EInputState.Down)
            {
                foreach (var item in m_EventHandleres)
                {
                    item.OnDrag(Input.mousePosition);
                }
            }
        }
        else
        {
            bool foundTouch = false;
            Touch thisTouch = new Touch();
            for (int i = 0; i < Input.touches.Length; i++)
            {
                thisTouch = Input.touches[i];
                foundTouch = true;
            }

            if (foundTouch)
            {
                Vector2 screenPos = thisTouch.position;
                switch (thisTouch.phase)
                {
                    case TouchPhase.Began:
                        {
                            if (EventSystem.current.IsPointerOverGameObject(thisTouch.fingerId))
                                return;

                            m_InputState = EInputState.Down;
                            foreach (var item in m_EventHandleres)
                            {
                                item.OnPressDown(thisTouch.position);
                            }
                        }
                        break;
                    case TouchPhase.Ended:
                        {
                            if (m_InputState != EInputState.Down)
                                return;

                            if (EventSystem.current.IsPointerOverGameObject(thisTouch.fingerId))
                                return;

                            m_InputState = EInputState.Up;
                            foreach (var item in m_EventHandleres)
                            {
                                item.OnReleaseUp(thisTouch.position);
                            }
                        }
                        break;
                    case TouchPhase.Moved:
                        if (m_InputState == EInputState.Down)
                        {
                            if (EventSystem.current.IsPointerOverGameObject(thisTouch.fingerId))
                                return;

                            foreach (var item in m_EventHandleres)
                            {
                                item.OnDrag(thisTouch.position);
                            }
                        }
                        break;
                    case TouchPhase.Stationary:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
