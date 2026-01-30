using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private Image _hpSlider;
    [SerializeField] private Image _satietySlider;
    [SerializeField] private Image _hydrationSlider;

    private void Update()
    {
        //if (Loading.Instance.isLoading) return;

        _hpSlider.fillAmount = Player.Instance.State.GetCurrentHp() / 100;
        _satietySlider.fillAmount = Player.Instance.State.GetCurrentSatiety() / 100;
        _hydrationSlider.fillAmount = Player.Instance.State.GetCurrentHydration() / 100;
    }
}