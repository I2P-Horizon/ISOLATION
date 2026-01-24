using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private Image _hpSlider;
    [SerializeField] private Image _satietySlider;

    private void Update()
    {
        if (Loading.Instance.isLoading) return;

        _hpSlider.fillAmount = Player.Instance.State.GetCurrentHp() / 100;
        _satietySlider.fillAmount = Player.Instance.State.GetCurrentSatiety() / 100;
    }
}