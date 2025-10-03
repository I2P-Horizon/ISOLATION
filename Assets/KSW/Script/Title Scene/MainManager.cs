using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public GameObject mainPanel;

    public GameObject HUD;
    public GameObject OVERLAY;

    public Button newGameButton;
    public Button settingButton;
    public Button exitButton;

    private void NewStart()
    {
        HUD.SetActive(false);
        OVERLAY.SetActive(false);
        StartCoroutine(Loading.Instance.LoadGameScene());
    }

    private void Exit()
    {
        Application.Quit();
    }

    private IEnumerator StartEffect()
    {
        mainPanel.GetComponent<UIAnimator>().Close();
        StartCoroutine(Fade.Instance.FadeIn(Color.white));
        yield return new WaitForSeconds(0.5f);
        mainPanel.GetComponent<UIAnimator>().Show();
    }

    private void Start()
    {
        StartCoroutine(StartEffect());
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

        newGameButton.onClick.AddListener(() => NewStart());
        settingButton.onClick.AddListener(() => GameSettings.Instance.GameSettingsUI());
        exitButton.onClick.AddListener(() => Exit());
    }
}