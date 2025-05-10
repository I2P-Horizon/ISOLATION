using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#region 버튼 컨트롤러
[System.Serializable]
public class ButtonController
{
    [SerializeField] private GameObject[] checkmarks;
    private int currentIndex = 0;

    /// <summary>
    /// 선택된 항목만 체크 표시
    /// </summary>
    /// <param name="index"></param>
    private void SetActiveOnly(int index)
    {
        for (int i = 0; i < checkmarks.Length; i++) checkmarks[i].SetActive(i == index);
    }

    /// <summary>
    /// 중복 클릭 방지, 해당 인덱스의 체크마크를 활성화
    /// </summary>
    /// <param name="index"></param>
    public void OnClickSetting(int index)
    {
        if (index == currentIndex) return;
        SetActiveOnly(index);
        currentIndex = index;
    }

    /// <summary>
    /// 기본 선택 인덱스를 기준으로 체크마크 초기화
    /// </summary>
    /// <param name="defaultIndex"></param>
    public void Init(int defaultIndex = 0)
    {
        currentIndex = defaultIndex;
        SetActiveOnly(currentIndex);
    }
}
#endregion

#region 해상도 컨트롤러
public class ResolutionController
{
    public Dropdown resolutionDropdown;

    List<Resolution> resolutions = new List<Resolution>();

    int resolutionNum;
    int optionNum = 0;

    int[,] allowedResolutions = new int[,]
    {
        {1920, 1080},
        {1600, 900},
        {1366, 768},
        {1280, 720},
    };

    public void InitUI()
    {
        var filteredResolutions = Screen.resolutions
            .Where(res => IsAllowedResolution(res.width, res.height))
            .GroupBy(res => new { res.width, res.height })
            .Select(g => g.First())
            .OrderByDescending(res => res.width * res.height)
            .ToList();

        resolutions = filteredResolutions;
        resolutionDropdown.options.Clear();
        optionNum = 0;

        foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + "x" + item.height + " ";
            resolutionDropdown.options.Add(option);
            if (item.width == Screen.width && item.height == Screen.height) resolutionDropdown.value = optionNum;
            optionNum++;
        }

        resolutionDropdown.RefreshShownValue();
    }

    bool IsAllowedResolution(int width, int height)
    {
        for (int i = 0; i < allowedResolutions.GetLength(0); i++) if (allowedResolutions[i, 0] == width && allowedResolutions[i, 1] == height) return true;
        return false;
    }

    public void DropdownOptionChange(int x)
    {
        resolutionNum = x;
        var selected = resolutions[resolutionNum];
        Screen.SetResolution(selected.width, selected.height, FullScreenMode.Windowed);
    }
}
#endregion

#region 화면 컨트롤러
public class ScreenModeController
{
    private FullScreenMode currentMode;

    public ScreenModeController() { currentMode = Screen.fullScreenMode; }

    public void SetFullScreen(bool isFullScreen)
    {
        currentMode = isFullScreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.fullScreenMode = currentMode;
    }
}
#endregion

#region 그래픽 컨트롤러
public class GraphicsController
{
    private int currentQuality;

    public void SetQuality(int index)
    {
        currentQuality = index;
        QualitySettings.SetQualityLevel(currentQuality, true);
    }

    public void Init(int defaultIndex = 0)
    {
        currentQuality = defaultIndex;
        QualitySettings.SetQualityLevel(currentQuality, true);
    }
}
#endregion

public class GameSettings : MonoBehaviour
{
    [SerializeField] private ButtonController screenSettings;
    [SerializeField] private ButtonController graphicsSettings;

    [SerializeField] private Dropdown resolutionDropdown;
    private ResolutionController resolutionController;
    private ScreenModeController screenModeController;
    private GraphicsController graphicsController;

    /// <summary>
    /// 해상도 설정
    /// </summary>
    /// <param name="x"></param>
    public void OnClickResolution(int x)
    {
        resolutionController.DropdownOptionChange(x);
    }

    /// <summary>
    /// 화면 설정
    /// </summary>
    /// <param name="index"></param>
    public void OnClickScreenSetting(int index)
    {
        screenSettings.OnClickSetting(index);

        bool isFullScreen = (index == 0);
        screenModeController.SetFullScreen(isFullScreen);
    }

    /// <summary>
    /// 그래픽 설정
    /// </summary>
    /// <param name="index"></param>
    public void OnClickGraphicsSetting(int index)
    {
        graphicsSettings.OnClickSetting(index);
        graphicsController.SetQuality(index);
    }

    private void Start()
    {
        screenSettings.Init(0);
        graphicsSettings.Init(0);

        resolutionController = new ResolutionController();
        screenModeController = new ScreenModeController();
        graphicsController = new GraphicsController();

        resolutionController.resolutionDropdown = resolutionDropdown;
        resolutionController.InitUI();
        resolutionDropdown.onValueChanged.AddListener(resolutionController.DropdownOptionChange);
    }
}