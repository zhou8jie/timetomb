using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingousFramework;
using DG.Tweening;

public class GameGlobal : MonoBehaviour
{
    public Transform timeTombTran = null;
    private GameMode m_Mode = null;
    private Timer m_Timer = new Timer();
    private LevelManager m_LevelManager = null;
    private InputManager m_InputManager = null;

    public InputManager inputMgr
    {
        get { return m_InputManager; }
    }

    public GameMode mode
    {
        get { return m_Mode; }
    }
    public Timer timer
    {
        get { return m_Timer; }
    }
    public LevelManager levelMgr
    {
        get { return m_LevelManager; }
    }

    public static GameGlobal s_Inst = null;
    public static GameGlobal Instance()
    {
        return s_Inst;
    }

    void Awake()
    {
        s_Inst = this;
        DOTween.Init();
    }

    void Start ()
    {
        m_InputManager = GetComponentInChildren<InputManager>();
        m_LevelManager = GameObject.Find("/Levels").GetComponent<LevelManager>();
        m_Mode = new GameMode();
        m_Mode.ChangeGameState(GameMode.State_Main);
	}
	
	void Update ()
    {
        float dt = Time.deltaTime;
        m_Mode.UpdateGameMode();
        m_Timer.Tick(dt);
	}
}
