using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GameSettings : MonoBehaviour
{
    #region �̱���

    private static GameSettings instance;

    public static GameSettings Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    #endregion

    #region ��ư ��Ʈ��

    /// <summary>
    /// ���� �� ��ư
    /// </summary>
    private void UpdateSettingsTabButton(Button selected)
    {
        SetButtonColor(videoTabButton, selected == videoTabButton);
        SetButtonColor(graphicsTabButton, selected == graphicsTabButton);
        SetButtonColor(audioTabButton, selected == audioTabButton);
    }

    /// <summary>
    /// �ػ� ���� ��ư ����
    /// </summary>
    /// <param name="selected"></param>
    private void UpdateResolutionButton(Button selected)
    {
        SetButtonColor(R_1920x1080, selected == R_1920x1080);
        SetButtonColor(R_1366x768, selected == R_1366x768);
        SetButtonColor(R_1280x720, selected == R_1280x720);
    }

    /// <summary>
    /// ȭ�� ���� ��ư ����
    /// </summary>
    /// <param name="selected"></param>
    private void UpdateScreenButton(Button selected)
    {
        SetButtonColor(FullScreen, selected == FullScreen);
        SetButtonColor(WindowScreen, selected == WindowScreen);
    }

    /// <summary>
    /// ������ ���� ��ư ����
    /// </summary>
    /// <param name="selected"></param>
    private void UpdateFrameButton(Button selected)
    {
        SetButtonColor(FPS_144, selected == FPS_144);
        SetButtonColor(FPS_60, selected == FPS_60);
        SetButtonColor(FPS_30, selected == FPS_30);
    }

    /// <summary>
    /// ��ư ���� ����
    /// </summary>
    /// <param name="button"></param>
    /// <param name="isSelected"></param>
    private void SetButtonColor(Button button, bool isSelected)
    {
        var image = button.GetComponent<Image>();
        if (image != null) image.color = isSelected ? Color.white : Color.black;
    }

    #endregion

    #region ���� ����

    /// <summary>
    /// �ػ� ����
    /// </summary>
    [Header("�ػ� ����")]
    [SerializeField] private Button R_1920x1080;
    [SerializeField] private Button R_1366x768;
    [SerializeField] private Button R_1280x720;

    private void OnResolution(int width, int height, Button selected)
    {
        Screen.SetResolution(width, height, Screen.fullScreenMode);
        UpdateResolutionButton(selected);
        savedResolution = new Vector2Int(width, height);
        SaveSettings();
    }

    private void Resolution()
    {
        R_1920x1080.onClick.AddListener(() => OnResolution(1920, 1080, R_1920x1080));
        R_1366x768.onClick.AddListener(() => OnResolution(1366, 768, R_1366x768));
        R_1280x720.onClick.AddListener(() => OnResolution(1280, 720, R_1280x720));
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    [Header("������ ����")]
    [SerializeField] private Button FPS_144;
    [SerializeField] private Button FPS_60;
    [SerializeField] private Button FPS_30;

    private void SetFrame(int frame, Button selected)
    {
        Application.targetFrameRate = frame;
        UpdateFrameButton(selected);
        savedFrameRate = frame;
        SaveSettings();
    }

    private void Frame()
    {
        QualitySettings.vSyncCount = 0;
        FPS_144.onClick.AddListener(() => SetFrame(144, FPS_144));
        FPS_60.onClick.AddListener(() => SetFrame(60, FPS_60));
        FPS_30.onClick.AddListener(() => SetFrame(30, FPS_30));
    }

    /// <summary>
    /// ȭ�� ����
    /// </summary>
    [Header("ȭ�� ��� ����")]
    [SerializeField] private Button FullScreen;
    [SerializeField] private Button WindowScreen;

    private void SetScreenMode(FullScreenMode mode, Button selected)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, mode);
        UpdateScreenButton(selected);
        savedScreenMode = mode;
        SaveSettings();
    }

    private void ScreenMode()
    {
        FullScreen.onClick.AddListener(() => SetScreenMode(FullScreenMode.FullScreenWindow, FullScreen));
        WindowScreen.onClick.AddListener(() => SetScreenMode(FullScreenMode.Windowed, WindowScreen));
    }

    #endregion

    #region �׷��� ����
    #endregion

    #region ����� ����
    #endregion

    #region UI ���� & �ʱ�ȭ

    [Header("UI ���� & �ʱ�ȭ")]
    public GameObject gameSettingsUI;
    public GameObject background;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Button videoTabButton;
    [SerializeField] private Button graphicsTabButton;
    [SerializeField] private Button audioTabButton;

    [SerializeField] private GameObject videoSettingTab;
    [SerializeField] private GameObject graphicsSettingTab;
    [SerializeField] private GameObject audioSettingTab;

    private void SettingTab(int tabValue, Button selected)
    {
        UpdateSettingsTabButton(selected);

        switch(tabValue)
        {
            case 1:
                videoSettingTab.SetActive(true);
                graphicsSettingTab.SetActive(false);
                audioSettingTab.SetActive(false);
                break;

            case 2:
                videoSettingTab.SetActive(false);
                graphicsSettingTab.SetActive(true);
                audioSettingTab.SetActive(false);
                break;

            case 3:
                videoSettingTab.SetActive(false);
                graphicsSettingTab.SetActive(false);
                audioSettingTab.SetActive(true);
                break;
        }
    }

    private void Tab()
    {
        SettingTab(1, videoTabButton);
        videoTabButton.onClick.AddListener(() => SettingTab(1, videoTabButton));
        graphicsTabButton.onClick.AddListener(() => SettingTab(2, graphicsTabButton));
        audioTabButton.onClick.AddListener(() => SettingTab(3, audioTabButton));
    }

    public void GameSettingsUI()
    {
        if (!gameSettingsUI.activeSelf) { gameSettingsUI.GetComponent<UIAnimator>().Show(); background.SetActive(true); }
        else { gameSettingsUI.GetComponent<UIAnimator>().Close(); background.SetActive(false); }
    }

    private void Init()
    {
        gameSettingsUI.SetActive(false);
        mainCamera.enabled = false;
    }

    #endregion

    #region ���� �� ����

    private const string KEY_RESOLUTION_WIDTH = "ResolutionWidth";
    private const string KEY_RESOLUTION_HEIGHT = "ResolutionHeight";
    private const string KEY_FRAMERATE = "FrameRate";
    private const string KEY_SCREENMODE = "ScreenMode";

    private Vector2Int savedResolution;
    private int savedFrameRate;
    private FullScreenMode savedScreenMode;

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(KEY_RESOLUTION_WIDTH, savedResolution.x);
        PlayerPrefs.SetInt(KEY_RESOLUTION_HEIGHT, savedResolution.y);
        PlayerPrefs.SetInt(KEY_FRAMERATE, savedFrameRate);
        PlayerPrefs.SetInt(KEY_SCREENMODE, (int)savedScreenMode);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        int width = PlayerPrefs.GetInt(KEY_RESOLUTION_WIDTH, 1920);
        int height = PlayerPrefs.GetInt(KEY_RESOLUTION_HEIGHT, 1080);
        int frameRate = PlayerPrefs.GetInt(KEY_FRAMERATE, 60);
        FullScreenMode mode = (FullScreenMode)PlayerPrefs.GetInt(KEY_SCREENMODE, (int)FullScreenMode.FullScreenWindow);

        savedResolution = new Vector2Int(width, height);
        savedFrameRate = frameRate;
        savedScreenMode = mode;
    }

    private void ApplySavedSettings()
    {
        Screen.SetResolution(savedResolution.x, savedResolution.y, savedScreenMode);
        Application.targetFrameRate = savedFrameRate;

        if (savedResolution.x == 1920) UpdateResolutionButton(R_1920x1080);
        else if (savedResolution.x == 1366) UpdateResolutionButton(R_1366x768);
        else UpdateResolutionButton(R_1280x720);

        if (savedFrameRate == 144) UpdateFrameButton(FPS_144);
        else if (savedFrameRate == 60) UpdateFrameButton(FPS_60);
        else UpdateFrameButton(FPS_30);

        if (savedScreenMode == FullScreenMode.FullScreenWindow) UpdateScreenButton(FullScreen);
        else UpdateScreenButton(WindowScreen);
    }

    #endregion

    private void Start()
    {
        Resolution();
        ScreenMode();
        Frame();
        Tab();
        Init();

        LoadSettings();
        ApplySavedSettings();
    }
}