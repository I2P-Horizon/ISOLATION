using System;
using System.Collections;
using UnityEngine;
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
#endregion

public class Settings : MonoBehaviour
{
    public GameObject panel;

    [Header("Screen Settings")]
    [SerializeField] private Selector<Vector2Int> resolution;
    [SerializeField] private Selector<int> fps;
    [SerializeField] private Selector<string> ScreenMode;

    [Header("Button")]
    [SerializeField] private Button nextResButton;
    [SerializeField] private Button prevResButton;

    [SerializeField] private Button nextButton_FPS;
    [SerializeField] private Button prevButton_FPS;

    [SerializeField] private Button nextButton_ScreenMode;
    [SerializeField] private Button prevButton_ScreenMode;

    [SerializeField] private Button saveButton;

    private const string KEY_RES = "settings.resolution";
    private const string KEY_FPS = "settings.fps";
    private const string KEY_SCREENMODE = "settings.screenmode";

    public void Init()
    {
        /* ----------------------------- 불러오기 ----------------------------- */
        resolution.SetIndex(PlayerPrefs.GetInt(KEY_RES, 0));
        fps.SetIndex(PlayerPrefs.GetInt(KEY_FPS, 0));
        ScreenMode.SetIndex(PlayerPrefs.GetInt(KEY_SCREENMODE, 0));

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

        /* ----------------------------- 버튼 연결 ----------------------------- */
        nextResButton.onClick.AddListener(() => resolution.Next());
        prevResButton.onClick.AddListener(() => resolution.Prev());

        nextButton_FPS.onClick.AddListener(() => fps.Next());
        prevButton_FPS.onClick.AddListener(() => fps.Prev());

        nextButton_ScreenMode.onClick.AddListener(() => ScreenMode.Next());
        prevButton_ScreenMode.onClick.AddListener(() => ScreenMode.Prev());

        saveButton.onClick.AddListener(() => save());
    }

    private void save()
    {
        PlayerPrefs.SetInt(KEY_RES, resolution.GetIndex());
        PlayerPrefs.SetInt(KEY_FPS, fps.GetIndex());
        PlayerPrefs.SetInt(KEY_SCREENMODE, ScreenMode.GetIndex());
        PlayerPrefs.Save();

        panel.GetComponent<UIAnimator>().Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) save();
    }

    private void Start()
    {
        saveButton.onClick.AddListener(() => save());
    }
}