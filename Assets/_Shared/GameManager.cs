using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager  {

    public event System.Action<PlayerScript> OnLocalPlayerJoined;
    private GameObject gameObject;

    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = new GameManager();
                m_Instance.gameObject = new GameObject("_gameManager");
                m_Instance.gameObject.AddComponent<InputController>();
                m_Instance.gameObject.AddComponent<Timer>();
            }
            return m_Instance;
        }
    }

    private InputController m_inputController;
    public InputController InputController
    {
        get
        {
            if(m_inputController == null)
                m_inputController = gameObject.GetComponent<InputController>();
            return m_inputController;
        }
    }
    private Timer m_Timer;
    public Timer Timer
    {
        get
        {
            if (m_Timer == null)
                m_Timer = gameObject.GetComponent<Timer>();
            return m_Timer;
        }
    }

    private PlayerScript m_LocalPlayer;
    public PlayerScript LocalPlayer
    {
        get
        {
            return m_LocalPlayer;
        }
        set
        {
            m_LocalPlayer = value;
            if (OnLocalPlayerJoined != null)
            {
                OnLocalPlayerJoined(m_LocalPlayer);
            }
                
        }
    }
}
