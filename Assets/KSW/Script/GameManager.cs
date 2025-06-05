using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InventorySystem;

public class GameManager : MonoBehaviour
{
    #region ΩÃ±€≈Ê
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else Destroy(this.gameObject);
    }
    #endregion

    private UIManager uiManager;

    #region Pause
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
        GameSettings.Instance.gameSettingsUI.GetComponent<UIAnimator>().Show();
    }

    private void Exit()
    {
        SceneManager.LoadScene("MainScene");
    }
    #endregion

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.01f);
        uiManager.PopUpShow(uiManager.gameOverUI);
        Time.timeScale = 0;
    }

    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        Pause();
    }

    private void Start()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);
        uiManager = FindFirstObjectByType<UIManager>();
        continueButton.onClick.AddListener(Continue);
        settingButton.onClick.AddListener(Settings);
        exitButton.onClick.AddListener(Exit);
    }
}