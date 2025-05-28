using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Text hpText;

    private BossMonster boss;

    private void Awake()
    {
        boss = GetComponentInParent<BossMonster>();
    }

    private void Start()
    {
        SetHpText();
        SetHpAmount();
    }

    private void Update()
    {
        SetHpText();
        SetHpAmount();
    }

    private void SetHpText()
    {
        if (boss.curHp <= 0)
            hpText.text = 0 + " / " + (int)boss.maxHp;
        else
        {
            hpText.text = (int)boss.curHp + " / " + (int)boss.maxHp;
        }
    }

    private void SetHpAmount()
    {
        float hpFillAmount = (float)(boss.curHp / boss.maxHp);
        hpBar.value = hpFillAmount;
    }
}
