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

    #region �÷��̾� ���� (PlayerStats)
    private void PlayerStats()
    {
        // �÷��̾� ���� �ǽð� �ݿ�
        hpSlider.value = Player.Instance.State.GetCurrentHp();
        satietySlider.value = Player.Instance.State.GetCurrentSatiety();
    }
    #endregion

    #region ��ư ���� (ButtonConnection)
    private void ButtonConnection()
    {
        // Button ����
        replayButton.onClick.AddListener(() => GameManager.Instance.SceneChange("GameScene"));
        mainButton.onClick.AddListener(() => GameManager.Instance.SceneChange("MainScene"));
    }
    #endregion

    #region �˾� (PopUpShow/PopUpClose)
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

    #region �ð� (Time)
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

    #region �̴ϸ� (MiniMap)
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