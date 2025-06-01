using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameSettings gameSettings;

    public GameObject pauseUI;

    public void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseUI.activeSelf) pauseUI.GetComponent<UIAnimator>().Show();
            else pauseUI.GetComponent<UIAnimator>().Close();
        }
    }

    public void GameSettingsUI() { gameSettings.GameSettingsUI(); }

    private void Update()
    {
        Pause();
    }

    private void Awake()
    {
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);
    }

    private void Start()
    {
        gameSettings = FindFirstObjectByType<GameSettings>();
    }
}