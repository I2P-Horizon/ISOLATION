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

    [Header("버튼 연결")]
    public Button continueButton;
    public Button settingButton;
    public Button exitButton;
    public Button replayButton;
    public Button mainButton;

    [Header("Hp, Satiety")]
    public Image hpSlider;
    public Image satietySlider;

    [Header("시간")]
    public GameObject timeValue_Day;
    public GameObject timeValue_Night;

    [Header("월드 맵")]
    public GameObject worldMap;
    public WorldMapMarker worldMapMarker;

    [Header("인벤토리")]
    public GameObject inventoryUI;

    /// <summary>
    /// 플레이어 상태 실시간 반영
    /// </summary>
    private void PlayerStats()
    {
        hpSlider.fillAmount = Player.Instance.State.GetCurrentHp() / 100;
        satietySlider.fillAmount = Player.Instance.State.GetCurrentSatiety() / 100;
    }

    /// <summary>
    /// 시간 UI
    /// </summary>
    private void TimeUI()
    {
        if (TimeManager.Instance == null || TimeManager.Instance.RealTimePerDaySec <= 0) return;

        float currentTime = TimeManager.Instance.CurrentTime;
        bool isNight = TimeManager.Instance.IsNight;
        float dayTime = TimeManager.Instance.RealTimePerDaySec;
        float halfDay = dayTime / 2f;

        float fillDay = 0f;
        float fillNight = 0f;

        if (!isNight)
        {
            fillDay = (currentTime % halfDay) / halfDay;
            fillNight = 0f;

            if (!timeValue_Day.activeSelf) timeValue_Day.GetComponent<UIAnimator>().Show();
            if (timeValue_Night.activeSelf) timeValue_Night.SetActive(false);
        }

        else
        {
            fillNight = (currentTime % halfDay) / halfDay;
            fillDay = 0f;

            if (!timeValue_Night.activeSelf) timeValue_Night.GetComponent<UIAnimator>().Show();
            if (timeValue_Day.activeSelf) timeValue_Day.SetActive(false);
        }

        timeValue_Day.GetComponent<Image>().fillAmount = fillDay;
        timeValue_Night.GetComponent<Image>().fillAmount = fillNight;
    }

    /// <summary>
    /// 월드 맵
    /// </summary>
    public void WorldMapUI()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!worldMap.activeSelf) worldMap.GetComponent<UIAnimator>().Show();
            else worldMap.GetComponent<UIAnimator>().Close();
        }
    }

    public void InventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventoryUI.activeSelf) inventoryUI.GetComponent<UIAnimator>().Show();
            else inventoryUI.GetComponent<UIAnimator>().Close();
        }
    }

    public void Pause()
    {
        if (!pauseUI.activeSelf) pauseUI.GetComponent<UIAnimator>().Show();
        else if(pauseUI.activeSelf) pauseUI.GetComponent<UIAnimator>().Close();
    }

    private void Continue()
    {
        pauseUI.GetComponent<UIAnimator>().Close();
    }

    private void Settings()
    {
        pauseUI.GetComponent<UIAnimator>().Close();
    }

    private void Exit()
    {
        pauseUI.GetComponent<UIAnimator>().Close();
        SceneManager.LoadScene("MainScene");
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.01f);
        gameOverUI.GetComponent<UIAnimator>().Show();
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

        if (Loading.Instance == null) return;

        if (!Loading.Instance.isLoading)
        {
            PlayerStats();
            WorldMapUI();
            //InventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !Loading.Instance.isLoading) Pause();
    }

    private void Start()
    {
        ButtonConnection();
        PlayerStats();
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