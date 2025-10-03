using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public GameObject mainPanel;

    public Button newGameButton;
    public Button settingButton;
    public Button exitButton;

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

        newGameButton.onClick.AddListener(() => StartCoroutine(Loading.Instance.LoadGameScene()));
        settingButton.onClick.AddListener(() => GameSettings.Instance.GameSettingsUI());
        exitButton.onClick.AddListener(() => Exit());
    }
}