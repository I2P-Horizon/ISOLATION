using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider hpSlider;
    public Slider satietySlider;

    void Start()
    {
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
}