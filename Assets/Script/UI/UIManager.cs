using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static public UIManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple UIManager in scene");
            Destroy(this);
        }
    }

    [SerializeField] private GameObject m_startMenu;
    [SerializeField] private GameObject m_playerSelectionMenu;
    [SerializeField] private GameObject m_gameUI;
    [SerializeField] private GameObject m_creditMenu;
    [SerializeField] private GameObject m_winScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenStartMenu()
    {
        m_startMenu.SetActive(true);
        m_playerSelectionMenu.SetActive(false);
        m_gameUI.SetActive(false);
        m_creditMenu.SetActive(false);
        m_winScreen.SetActive(false);
    }

    public void OpenPlayerSelectionMenu()
    {
        m_startMenu.SetActive(false);
        m_playerSelectionMenu.SetActive(true);
        m_gameUI.SetActive(false);
        m_creditMenu.SetActive(false);
        m_winScreen.SetActive(false);
    }

    public void OpenGameUI()
    {
        m_startMenu.SetActive(false);
        // m_playerSelectionMenu.SetActive(false);
        m_gameUI.SetActive(true);
        m_creditMenu.SetActive(false);
        m_winScreen.SetActive(false);
    }

    public void OpenCreditMenu()
    {
        m_startMenu.SetActive(false);
        m_playerSelectionMenu.SetActive(false);
        m_gameUI.SetActive(false);
        m_creditMenu.SetActive(true);
        m_winScreen.SetActive(false);
    }

    public void OpenWinScreen()
    {
        m_startMenu.SetActive(false);
        m_playerSelectionMenu.SetActive(false);
        m_gameUI.SetActive(false);
        m_creditMenu.SetActive(false);
        m_winScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        // Reload the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
