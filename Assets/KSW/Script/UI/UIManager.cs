using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject background;
    public GameObject gameOverUI;

    public Button replayButton;
    public Button mainButton;

    public Slider hpSlider;
    public Slider satietySlider;

    public GameObject timeValue1;
    public GameObject timeValue2;

    public GameObject worldMap;
    public WorldMapMarker worldMapMarker;

    public GameObject inventoryUI;

    #region 플레이어 스텟 (PlayerStats)
    private void PlayerStats()
    {
        // 플레이어 스텟 실시간 반영
        hpSlider.value = Player.Instance.State.GetCurrentHp();
        satietySlider.value = Player.Instance.State.GetCurrentSatiety();
    }
    #endregion

    #region 버튼 연결 (ButtonConnection)
    private void ButtonConnection()
    {
        // Button 연결
        replayButton.onClick.AddListener(() => GameManager.Instance.SceneChange("GameScene"));
        mainButton.onClick.AddListener(() => GameManager.Instance.SceneChange("MainScene"));
    }
    #endregion

    #region 팝업 (PopUpShow/PopUpClose)
    public void PopUpShow(GameObject ui)
    {
        background.SetActive(true);
        ui.GetComponent<UIAnimator>().Show();
    }

    public void PopUpClose(GameObject ui)
    {
        background.SetActive(false);
        ui.GetComponent<UIAnimator>().Close();
    }
    #endregion

    #region 시계 (Time)
    private void TimeUI()
    {
        if (TimeManager.instance == null || TimeManager.instance.RealTimePerDaySec <= 0) return;

        float currentTime = TimeManager.instance.CurrentTime;
        float halfDay = TimeManager.instance.RealTimePerDaySec / 2f;

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
    #endregion

    #region 미니맵 (MiniMap)
    #endregion

    #region 프레임 표시
    
    public Text fpsText;

    float deltaTime = 0.0f;

    private void FPS(int active)
    {
        if (active == 1)
        {
            fpsText.gameObject.SetActive(true);
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = ""+Mathf.Ceil(fps);
        }

        else fpsText.gameObject.SetActive(false);
    }

    #endregion

    #region 월드 맵

    private float lastRenderTime;
    public float renderCooldown = 1f;

    public void WorldMapUI()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            bool isActive = worldMap.activeSelf;
            worldMap.SetActive(!isActive);

            if (!isActive)
            {
                worldMapMarker.enabled = true;

                if (Time.time - lastRenderTime > renderCooldown)
                {
                    worldMapMarker.mapCamera.Render();
                    lastRenderTime = Time.time;
                }
            }

            else worldMapMarker.enabled = false;
        }
    }

    #endregion

    #region 인벤토리

    public void InventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryUI.activeSelf) inventoryUI.SetActive(false);
            else inventoryUI.SetActive(true);
        }
    }

    #endregion

    private void Update()
    {
        PlayerStats();
        TimeUI();
        WorldMapUI();
        InventoryUI();
        FPS(GameSettings.Instance.frameText);
    }

    private void Start()
    {
        ButtonConnection();

        hpSlider.value = Player.Instance.State.MaxHp;
        satietySlider.value = Player.Instance.State.MaxSatiety;
    }
}