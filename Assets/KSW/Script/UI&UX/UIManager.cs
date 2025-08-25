using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;

    [Header("일시 정지 UI")]
    public GameObject pauseUI;

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    [Header("팝업 반투명 배경")]
    public GameObject background;

    [Header("버튼 연결")]
    public Button continueButton;
    public Button settingButton;
    public Button exitButton;
    public Button replayButton;
    public Button mainButton;

    [Header("Hp, Satiety")]
    public Slider hpSlider;
    public Slider satietySlider;

    [Header("시간")]
    public GameObject timeValue1;
    public GameObject timeValue2;

    [Header("월드 맵")]
    public GameObject worldMap;
    public WorldMapMarker worldMapMarker;

    [Header("인벤토리")]
    public GameObject inventoryUI;

    private int backgroundCount = 0;

    public GameObject renderLoding;

    /// <summary>
    /// 반투명 배경 컨트롤
    /// </summary>
    public void ShowBackground()
    {
        backgroundCount++;
        background.SetActive(true);
    }

    public void HideBackground()
    {
        backgroundCount--;
        if (backgroundCount <= 0)
        {
            backgroundCount = 0;
            background.SetActive(false);
        }
    }

    /// <summary>
    /// 팝업창 컨트롤
    /// </summary>
    /// <param name="ui"></param>
    public void PopUpShow(GameObject ui)
    {
        ShowBackground();
        ui.GetComponent<UIAnimator>().Show();
    }

    public void PopUpClose(GameObject ui)
    {
        StartCoroutine(PopUpCloseRoutine(ui));
    }

    private IEnumerator PopUpCloseRoutine(GameObject ui)
    {
        ui.GetComponent<UIAnimator>().Close();
        yield return new WaitForSeconds(0f);
        HideBackground();
    }

    /// <summary>
    /// 플레이어 상태 실시간 반영
    /// </summary>
    private void PlayerStats()
    {
        hpSlider.value = Player.Instance.State.GetCurrentHp();
        satietySlider.value = Player.Instance.State.GetCurrentSatiety();
    }

    /// <summary>
    /// 시간 UI
    /// </summary>
    private void TimeUI()
    {
        if (TimeManager.Instance == null || TimeManager.Instance.RealTimePerDaySec <= 0) return;

        float currentTime = TimeManager.Instance.CurrentTime;
        float halfDay = TimeManager.Instance.RealTimePerDaySec / 2f;

        float fill1 = 0f;
        float fill2 = 0f;

        if (currentTime < halfDay)
        {
            fill1 = currentTime / halfDay;
            fill2 = 0f;
        }
        else
        {
            fill1 = 1f;
            fill2 = (currentTime - halfDay) / halfDay;

            if (fill2 >= 1f)
            {
                fill1 = 0f;
                fill2 = 0f;
            }
        }

        timeValue1.GetComponent<Image>().fillAmount = fill1;
        timeValue2.GetComponent<Image>().fillAmount = fill2;
    }

    /// <summary>
    /// 월드 맵
    /// </summary>
    public void WorldMapUI()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!worldMap.activeSelf)
            {
                GlobalUIController.Instance.PopUpShow(worldMap);
            }

            else
            {
                GlobalUIController.Instance.PopUpClose(worldMap);
            }
        }
    }

    public void InventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    public void Pause()
    {
        if (!pauseUI.activeSelf && !GameSettings.Instance.gameSettingsUI.activeSelf)
        {
            GlobalUIController.Instance.PopUpShow(pauseUI);
        }

        else if(pauseUI.activeSelf && !GameSettings.Instance.gameSettingsUI.activeSelf)
        {
            GlobalUIController.Instance.PopUpClose(pauseUI);
        }

        else
        {
            GlobalUIController.Instance.PopUpClose(GameSettings.Instance.gameSettingsUI);
        }
    }

    private void Continue()
    {
        GlobalUIController.Instance.PopUpClose(pauseUI);
    }

    private void Settings()
    {
        GlobalUIController.Instance.PopUpShow(GameSettings.Instance.gameSettingsUI);
        GlobalUIController.Instance.PopUpClose(pauseUI);
    }

    private void Exit()
    {
        GlobalUIController.Instance.PopUpClose(pauseUI);
        SceneManager.LoadScene("MainScene");
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.01f);
        GlobalUIController.Instance.PopUpShow(gameOverUI);
        Time.timeScale = 0;
    }

    private void ButtonConnection()
    {
        replayButton.onClick.AddListener(() => GameManager.Instance.SceneChange("GameScene"));
        mainButton.onClick.AddListener(() => GameManager.Instance.SceneChange("MainScene"));

        continueButton.onClick.AddListener(Continue);
        settingButton.onClick.AddListener(Settings);
        exitButton.onClick.AddListener(Exit);
    }

    private void Update()
    {
        TimeUI();

        if (!Loading.Instance.isLoading)
        {
            PlayerStats();
            WorldMapUI();
            InventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !Loading.Instance.isLoading) Pause();
    }

    private void Start()
    {
        ButtonConnection();

        hpSlider.value = Player.Instance.State.MaxHp;
        satietySlider.value = Player.Instance.State.MaxSatiety;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else Destroy(this.gameObject);
    }
}