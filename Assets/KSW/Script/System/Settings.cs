using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        saveButton.onClick.AddListener(() => StartCoroutine(Save()));



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

    public IEnumerator Save()
    {
        if (!isSaving)
        {
            isSaving = true;
            message.GetComponent<UIAnimator>().Show();
            yield return new WaitForSeconds(1.5f);
            message.GetComponent<UIAnimator>().Close();
            yield return new WaitForSeconds(0.1f);
            isSaving = false;
        }
    }
}