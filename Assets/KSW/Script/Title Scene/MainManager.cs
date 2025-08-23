using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject loadingPanel;

    public Button newGameButton;
    public Button settingButton;
    public Button exitButton;

    public Slider loadingBar;

    private IEnumerator LoadGameScene()
    {
        loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float percent = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = Mathf.Lerp(1, 100, percent);
            yield return null;
        }

        loadingBar.value = 100f;

        yield return new WaitForSeconds(0.1f);

        operation.allowSceneActivation = true;
    }

    private void Exit()
    {
        Application.Quit();
    }

    private IEnumerator StartEffect()
    {
        mainPanel.GetComponent<UIAnimator>().Close();
        StartCoroutine(Fade.Instance.FadeOut(Color.white));
        yield return new WaitForSeconds(0.5f);
        mainPanel.GetComponent<UIAnimator>().Show();
    }

    private void Start()
    {
        StartCoroutine(StartEffect());
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);

        newGameButton.onClick.AddListener(() => StartCoroutine(LoadGameScene()));
        settingButton.onClick.AddListener(() => GameSettings.Instance.GameSettingsUI());
        exitButton.onClick.AddListener(() => Exit());
    }
}