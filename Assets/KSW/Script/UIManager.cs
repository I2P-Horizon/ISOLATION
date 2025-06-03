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

    void Start()
    {
        replayButton.onClick.AddListener(() => GameManager.Instance.SceneChange("GameScene"));
        mainButton.onClick.AddListener(() => GameManager.Instance.SceneChange("MainScene"));

        hpSlider.value = PlayerState.Instance.MaxHp;
        satietySlider.value = PlayerState.Instance.MaxSatiety;

        hpSlider.value = PlayerState.Instance.GetCurrentHp();
        satietySlider.value = PlayerState.Instance.GetCurrentSatiety();
    }

    void Update()
    {
        hpSlider.value = PlayerState.Instance.GetCurrentHp();
        satietySlider.value = PlayerState.Instance.GetCurrentSatiety();
    }

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
}