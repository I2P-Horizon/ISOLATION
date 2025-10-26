using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    public int GetIndex() => currentIndex;
    public void SetIndex(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, options.Length - 1);
        ApplyCurrentOption();
    }


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

    private void ApplyCurrentOption()
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

public class Settings : MonoBehaviour
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

    [Header("Message")]
    public GameObject message;
    public Button saveButton;

    private bool isSaving = false;

    private const string KEY_RES = "settings.resolution";
    private const string KEY_FPS = "settings.fps";
    private const string KEY_SCREENMODE = "settings.screenmode";
    private const string KEY_GRAPHICS = "settings.graphics";
    private const string KEY_AA = "settings.aa";
    private const string KEY_SHADOW = "settings.shadow";

    public void Init()
    {
        /* ----------------------------- 불러오기 ----------------------------- */
        resolution.SetIndex(PlayerPrefs.GetInt(KEY_RES, 0));
        fps.SetIndex(PlayerPrefs.GetInt(KEY_FPS, 0));
        ScreenMode.SetIndex(PlayerPrefs.GetInt(KEY_SCREENMODE, 0));
        graphics.SetIndex(PlayerPrefs.GetInt(KEY_GRAPHICS, 0));
        antiAliasing.SetIndex(PlayerPrefs.GetInt(KEY_AA, 0));
        shawdow.SetIndex(PlayerPrefs.GetInt(KEY_SHADOW, 0));

        /* ----------------------------- 옵션 적용 ----------------------------- */
        resolution.onApply = (res) => { Screen.SetResolution(res.x, res.y, Screen.fullScreen); };
        fps.onApply = (fpsValue) => { Application.targetFrameRate = fpsValue; };

        ScreenMode.onApply = (mode) =>
        {
            Vector2Int res = resolution.GetCurrent();
            switch (mode)
            {
                case "Full":
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Screen.SetResolution(res.x, res.y, true);
                    break;
                case "Windowed":
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Screen.SetResolution(res.x, res.y, false);
                    break;
            }
        };

        graphics.onApply = (level) =>
        {
            int index = Array.IndexOf(graphics.options, level);
            if (index >= 0) QualitySettings.SetQualityLevel(index);
        };

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

        shawdow.onApply = (shawdowLevel) =>
        {
            switch (shawdowLevel)
            {
                case "ON": QualitySettings.shadows = ShadowQuality.All; break;
                case "OFF": QualitySettings.shadows = ShadowQuality.Disable; break;
            }
        };

        /* ----------------------------- 버튼 연결 ----------------------------- */
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

        saveButton.onClick.AddListener(() => StartCoroutine(Save()));

        /* 탭 버튼 연결 */
        buttons[0].onClick.AddListener(ScreenTab);
        buttons[1].onClick.AddListener(GraphicsTab);
        buttons[2].onClick.AddListener(AudioTab);
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

    public IEnumerator Save()
    {
        if (!isSaving)
        {
            isSaving = true;

            /* 현재 인덱스 저장 */
            PlayerPrefs.SetInt(KEY_RES, resolution.GetIndex());
            PlayerPrefs.SetInt(KEY_FPS, fps.GetIndex());
            PlayerPrefs.SetInt(KEY_SCREENMODE, ScreenMode.GetIndex());
            PlayerPrefs.SetInt(KEY_GRAPHICS, graphics.GetIndex());
            PlayerPrefs.SetInt(KEY_AA, antiAliasing.GetIndex());
            PlayerPrefs.SetInt(KEY_SHADOW, shawdow.GetIndex());
            PlayerPrefs.Save();

            /* 메시지 표시 */
            message.GetComponent<UIAnimator>().Show();
            yield return new WaitForSeconds(1.5f);
            message.GetComponent<UIAnimator>().Close();
            yield return new WaitForSeconds(0.1f);

            isSaving = false;
        }
    }
}