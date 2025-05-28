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

    // Hp, Mp �ؽ�Ʈ ǥ�� ����
    private void SetHpText()
    {
        hpText.text = (int)playerData.CurHp + " / " + (int)playerData.MaxHp;
    }

    // �����̴� Value ����
    private void SetHpAmount()
    {
        float hpFillAmount = (float)(playerData.CurHp / playerData.MaxHp);
        hpBar.value = hpFillAmount;
    }
   
}
