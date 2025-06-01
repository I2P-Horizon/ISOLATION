using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Pause
    private GameSettings gameSettings;
    public GameObject pauseUI;

    public Button continueButton;
    public Button settingButton;
    public Button exitButton;

    public void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseUI.activeSelf) pauseUI.GetComponent<UIAnimator>().Show();
            else pauseUI.GetComponent<UIAnimator>().Close();
        }
    }

    private void Continue()
    {
        pauseUI.GetComponent<UIAnimator>().Close();
    }

    private void Settings()
    {
        gameSettings.gameSettingsUI.GetComponent<UIAnimator>().Show();
    }

    private void Exit()
    {
        SceneManager.LoadScene("MainScene");
    }
    #endregion

    private void Update()
    {
        Pause();
    }

    private void Start()
    {
        gameSettings = FindFirstObjectByType<GameSettings>();

        continueButton.onClick.AddListener(Continue);
        settingButton.onClick.AddListener(Settings);
        exitButton.onClick.AddListener(Exit);
    }

    private void Awake()
    {
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);
    }
}