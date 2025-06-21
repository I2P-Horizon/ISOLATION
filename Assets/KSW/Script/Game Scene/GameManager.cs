using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;

    public GameObject pauseUI;

    public Button continueButton;
    public Button settingButton;
    public Button exitButton;

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

    #region Pause
    public void Pause() { if (!pauseUI.activeSelf) uiManager.PopUpShow(pauseUI); else uiManager.PopUpClose(pauseUI); }

    private void Continue() { Pause(); }

    private void Settings() { uiManager.PopUpShow(GameSettings.Instance.gameSettingsUI); }

    private void Exit() { SceneManager.LoadScene("MainScene"); }
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
        if (Input.GetKeyDown(KeyCode.Escape)) Pause();
    }

    private void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);

        Time.timeScale = 1;

        continueButton.onClick.AddListener(Continue);
        settingButton.onClick.AddListener(Settings);
        exitButton.onClick.AddListener(Exit);
    }
}