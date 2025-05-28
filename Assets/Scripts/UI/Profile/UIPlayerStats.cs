using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private Slider hpBar;

    [SerializeField] private Text hpText;

    private PlayerData playerData;

    private void Start()
    {
        playerData = DataManager.Instance.GetPlayerData();
        SetHpText();
        SetHpAmount();
    }

    private void Update()
    {
        SetHpText();
        SetHpAmount();
    }

    // Hp, Mp 텍스트 표기 형식
    private void SetHpText()
    {
        hpText.text = (int)playerData.CurHp + " / " + (int)playerData.MaxHp;
    }

    // 슬라이더 Value 조절
    private void SetHpAmount()
    {
        float hpFillAmount = (float)(playerData.CurHp / playerData.MaxHp);
        hpBar.value = hpFillAmount;
    }
   
}
