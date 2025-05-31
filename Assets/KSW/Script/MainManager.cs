using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public Button newGameButton;
    public Button settingButton;
    public Button exitButton;

    private void NewGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void Setting()
    {
        GameSettings.Instance.gameSettingsUI.SetActive(true);
    }

    private void Exit()
    {

    }

    private void Start()
    {
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);

        newGameButton.onClick.AddListener(NewGame);
        settingButton.onClick.AddListener(Setting);
    }
}
