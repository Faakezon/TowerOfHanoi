using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public bool useSnapFunctionality = true;

    private Toggle toggle;
    private bool GameWon = false;

    #region DesignOptions
    public float snapSpeedTouch = 80;
    public float snapSpeedMouse = 20;
    public float snapRangeTouch = 1;
    public float snapRangeMouse = 1;
    #endregion

    void Awake()
    {
        InitGameManager();
    }

    private void InitGameManager()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        GetAndSetSnappyToggle();
        HandleGameWon();
    }

    #region WinningTheGame
    private void HandleGameWon()
    {
        if (GameWon)
        {
            if (UserInputManager.instance.IsAnyInput())
            {
                UserInputManager.instance.AllowInput = false;
                RingManager.instance.ResetList();
                SceneManager.LoadScene(0);
                GameWon = false;
            }
        }
    }
    public void WonGame()
    {
        AudioManager.instance.PlaySfx("WonGame");
        GameWon = true;
    }
    #endregion

    #region MenuSelection
    public void PlayGame()
    {
        AudioManager.instance.PlaySfx("PickUp");
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void GetAndSetSnappyToggle()
    {
        //Will be true every time we get back to game menu
        if (SceneManager.GetActiveScene().name == "MenuScene" && toggle == null)
        {
            toggle = GameObject.Find("Toggle").GetComponent<Toggle>();
            toggle.isOn = useSnapFunctionality;
        }
        useSnapFunctionality = toggle.isOn;
    }
    #endregion
}
