using System.Collections;
using System.Collections.Generic;
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
    private void Time()
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

    private void Update()
    {
        PlayerStats();
        Time();
    }

    private void Start()
    {
        ButtonConnection();

        hpSlider.value = Player.Instance.State.MaxHp;
        satietySlider.value = Player.Instance.State.MaxSatiety;
    }
}