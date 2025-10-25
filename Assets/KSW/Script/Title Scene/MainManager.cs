using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#region Selector
[Serializable]
public class Selector<T>
{
    /* 선택지 목록 */
    public T[] options;
    /* 선택된 항목 표시용 UI */
    public Text displayText;
    /* 선택 시 적용할 액션 */
    public Action<T> onApply;
    /* 현재 인덱스 */
    private int currentIndex = 0;

    public T GetCurrent() => options[currentIndex];

    public void Next()
    {
        currentIndex = (currentIndex + 1) % options.Length;
        Apply();
    }

    public void Prev()
    {
        currentIndex = (currentIndex - 1 + options.Length) % options.Length;
        Apply();
    }

    private void Apply()
    {
        onApply?.Invoke(GetCurrent());
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (GetCurrent() is Vector2Int res) displayText.text = $"{res.x} X {res.y}";
        else displayText.text = GetCurrent().ToString();
    }
}
#endregion

#region Settings
[Serializable]
public class Settings
{
    public GameObject[] tabs;
    public Button[] buttons;

    [Header("Screen Settings")]
    public Selector<Vector2Int> resolution;
    public Button nextResButton;
    public Button prevResButton;

    public Selector<int> fps;
    public Button nextButton_FPS;
    public Button prevButton_FPS;

    public Selector<string> ScreenMode;
    public Button nextButton_ScreenMode;
    public Button prevButton_ScreenMode;

    [Header("Graphics Settings")]
    public Selector<string> graphics;
    public Button nextGraphicsButton;
    public Button prevGraphicsButton;

    public Selector<string> antiAliasing;
    public Button nextButton_AntiAliasing;
    public Button prevButton_AntiAliasing;

    public Selector<string> shawdow;
    public Button nextButton_Shawdow;
    public Button prevButton_Shawdow;

    public void Init()
    {
        /* 화면 해상도 적용 */
        resolution.onApply = (res) =>
        {
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        };

        /* FPS 적용 */
        fps.onApply = (fps) =>
        {
            Application.targetFrameRate = fps;
        };

        /* 화면 모드 적용 */
        ScreenMode.onApply = (mode) =>
        {
            switch (mode)
            {
                case "Full":
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case "Windowed":
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }

            Vector2Int res = resolution.GetCurrent();
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        };

        /* 그래픽 품질 적용 */
        graphics.onApply = (level) =>
        {
            int index = Array.IndexOf(graphics.options, level);
            if (index >= 0) QualitySettings.SetQualityLevel(index);
        };

        /* 안티앨리어싱 적용 */
        antiAliasing.onApply = (aa) =>
        {
            int value = 0;
            switch (aa)
            {
                case "Off": value = 0; break;
                case "2x": value = 2; break;
                case "4x": value = 4; break;
                case "8x": value = 8; break;
            }
            QualitySettings.antiAliasing = value;
        };

        /* 그림자 적용 */
        shawdow.onApply = (shawdowLevel) =>
        {
            switch (shawdowLevel)
            {
                case "ON": QualitySettings.shadows = ShadowQuality.All; break;
                case "OFF": QualitySettings.shadows = ShadowQuality.Disable; break;
            }
        };

        /* 버튼 연결 */
        nextResButton.onClick.AddListener(() => resolution.Next());
        prevResButton.onClick.AddListener(() => resolution.Prev());

        nextButton_FPS.onClick.AddListener(() => fps.Next());
        prevButton_FPS.onClick.AddListener(() => fps.Prev());

        nextButton_ScreenMode.onClick.AddListener(() => ScreenMode.Next());
        prevButton_ScreenMode.onClick.AddListener(() => ScreenMode.Prev());

        nextGraphicsButton.onClick.AddListener(() => graphics.Next());
        prevGraphicsButton.onClick.AddListener(() => graphics.Prev());

        nextButton_AntiAliasing.onClick.AddListener(() => antiAliasing.Next());
        prevButton_AntiAliasing.onClick.AddListener(() => antiAliasing.Prev());

        nextButton_Shawdow.onClick.AddListener(() => shawdow.Next());
        prevButton_Shawdow.onClick.AddListener(() => shawdow.Prev());

        /* 탭 버튼 연결 */
        buttons[0].onClick.AddListener(ScreenTab);
        buttons[1].onClick.AddListener(GraphicsTab);
        buttons[2].onClick.AddListener(AudioTab);

        //resolution.Next();
        //graphics.Next();
        //antiAliasing.Next();
    }

    public void ScreenTab()
    {
        for (int i = 0; i < tabs.Length; i++)
            tabs[i].SetActive(i == 0);
        tabs[0].GetComponent<UIAnimator>()?.Show();
    }

    public void GraphicsTab()
    {
        for (int i = 0; i < tabs.Length; i++)
            tabs[i].SetActive(i == 1);
        tabs[1].GetComponent<UIAnimator>()?.Show();
    }

    public void AudioTab()
    {
        for (int i = 0; i < tabs.Length; i++)
            tabs[i].SetActive(i == 2);
        tabs[2].GetComponent<UIAnimator>()?.Show();
    }

    public void Save()
    {
        /* 일괄 저장 추가 예정. */
    }
}
#endregion

#region MainManager
public class MainManager : MonoBehaviour
{
    [Header("Main Camera Position")]
    [SerializeField] private Vector3 MainPosition = Vector3.zero;
    [SerializeField] private Vector3 MainRotation = Vector3.zero;

    [Header("Settings Camera Position")]
    [SerializeField] private Vector3 SettingsPosition = new Vector3(3.35f, 0, 0);
    [SerializeField] private Vector3 SettingsRotation = new Vector3(0f, 90f, 0f);

    [Header("Panel")]
    public GameObject mainPanel;
    public GameObject mainCamera;

    [Header("Canvas")]
    public GameObject HUD;
    public GameObject OVERLAY;

    [Header("Button")]
    public Button newGameButton;
    public Button settingButton;
    public Button exitButton;
    public Button s_BackButton;
    public Button s_SaveButton;

    public Settings settings;

    private bool isConversion = false;
    private float moveDuration = 1f;

    private void NewStart()
    {
        HUD.SetActive(false);
        OVERLAY.SetActive(false);
        StartCoroutine(Loading.Instance.LoadGameScene());
    }

    private void SettingsScreen()
    {
        if (isConversion) return;
        StartCoroutine(MoveCameraSmoothly(SettingsPosition, Quaternion.Euler(SettingsRotation)));
    }

    private void Back()
    {
        if (isConversion) return;
        StartCoroutine(MoveCameraSmoothly(MainPosition, Quaternion.Euler(MainRotation)));
    }

    private IEnumerator MoveCameraSmoothly(Vector3 targetPosition, Quaternion targetRotation)
    {
        isConversion = true;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
        isConversion = false;
    }

    private void Exit()
    {
        Application.Quit();
    }

    private IEnumerator StartEffect()
    {
        mainPanel.GetComponent<UIAnimator>()?.Close();
        StartCoroutine(Fade.Instance.FadeIn(Color.white));
        yield return new WaitForSeconds(0.5f);
        mainPanel.GetComponent<UIAnimator>()?.Show();
    }

    private void Start()
    {
        StartCoroutine(StartEffect());

        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

        // Main 화면 버튼
        newGameButton.onClick.AddListener(NewStart);
        settingButton.onClick.AddListener(SettingsScreen);
        exitButton.onClick.AddListener(Exit);
        s_BackButton.onClick.AddListener(Back);

        // Settings 초기화
        settings.Init();

        // 카메라 초기 위치
        mainCamera.transform.position = Vector3.zero;
        mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
#endregion