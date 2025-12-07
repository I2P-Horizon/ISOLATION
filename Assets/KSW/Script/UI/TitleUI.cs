using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Canvas")]
    [SerializeField] private GameObject HUD;

    [Header("Button")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button s_BackButton;
    [SerializeField] private Button s_SaveButton;

    [SerializeField] private Settings settings;

    private void PlayButton()
    {
        mainPanel.GetComponent<UIAnimator>().Close();
        StartCoroutine(Loading.Instance.LoadGameScene());
    }

    private void SettingsButton()
    {
        if (!settingsPanel.activeSelf) settingsPanel.GetComponent<UIAnimator>().Show();
        else settingsPanel.GetComponent<UIAnimator>().Close();
    }

    private void ExitButton()
    {
        Application.Quit();
    }

    private IEnumerator StartEffect()
    {
        mainPanel.GetComponent<UIAnimator>()?.Close();
        StartCoroutine(Fade.Instance.FadeIn(Color.black));
        yield return new WaitForSeconds(0.5f);
        mainPanel.GetComponent<UIAnimator>()?.Show();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene("TitleBackground");
        }
    }

    private void Start()
    {
        StartCoroutine(StartEffect());
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

        // Main 화면 버튼
        newGameButton.onClick.AddListener(PlayButton);
        settingButton.onClick.AddListener(SettingsButton);
        exitButton.onClick.AddListener(ExitButton);

        s_BackButton.onClick.AddListener(SettingsButton);

        // Settings 초기화
        settings.Init();
    }
}