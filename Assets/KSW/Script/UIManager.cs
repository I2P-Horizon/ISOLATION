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

    public Image fillA;
    public Image fillB;

    #region 팝업 띄움/닫음

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

    #region 시계 UI 관리
    public void TimeUI()
    {
        float time = TimeManager.instance.CurrentTime;
        float maxTime = TimeManager.instance.MaxTime;

        float cycleDuration = maxTime / 2f;
        float hour = time % maxTime;

        if (hour < cycleDuration)
        {
            fillA.fillAmount = hour / cycleDuration;
            fillB.fillAmount = 0f;
        }

        else if (hour < maxTime)
        {
            fillA.fillAmount = 1f;
            fillB.fillAmount = (hour - cycleDuration) / cycleDuration;
        }

        else { fillA.fillAmount = 0f; fillB.fillAmount = 0f; }
    }

    #endregion

    void Update()
    {
        // PlayerState 접근해서 실시간으로 UI 업데이트
        hpSlider.value = Player.Instance.State.GetCurrentHp();
        satietySlider.value = Player.Instance.State.GetCurrentSatiety();

        TimeUI();
    }

    void Start()
    {
        // 버튼 연결
        replayButton.onClick.AddListener(() => GameManager.Instance.SceneChange("GameScene"));
        mainButton.onClick.AddListener(() => GameManager.Instance.SceneChange("MainScene"));

        // PlayerState 접근해서 UI 연결
        hpSlider.value = Player.Instance.State.MaxHp;
        satietySlider.value = Player.Instance.State.MaxSatiety;
    }
}