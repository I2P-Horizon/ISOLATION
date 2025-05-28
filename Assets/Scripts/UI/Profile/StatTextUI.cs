using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatTextUI : MonoBehaviour
{
    [Header("Connected Texts")]
    [SerializeField] private Text damageText;
    [SerializeField] private Text hpText;
    [SerializeField] private Text speedText;
    [SerializeField] private Text defenseText;

    private void Update()
    {
        damageText.text = string.Format("{0}", Mathf.FloorToInt(DataManager.Instance.GetPlayerData().Damage));
        hpText.text = string.Format("{0}", Mathf.FloorToInt(DataManager.Instance.GetPlayerData().CurHp));
        speedText.text = string.Format("{0}%", Mathf.RoundToInt(DataManager.Instance.GetPlayerData().Speed));
        defenseText.text = string.Format("{0}", Mathf.FloorToInt(DataManager.Instance.GetPlayerData().Defense));
    }
}