using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public Button newGameButton;
    public Button settingButton;
    public Button exitButton;

    public GameObject loadingPanel;
    public Slider loadingBar;

    private void Start()
    {
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);

        newGameButton.onClick.AddListener(() => StartCoroutine(LoadGameScene()));
        settingButton.onClick.AddListener(() => Setting());
        exitButton.onClick.AddListener(() => Exit());
    }

    private void Setting()
    {
        GameSettings.Instance.gameSettingsUI.SetActive(true);
    }

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
}