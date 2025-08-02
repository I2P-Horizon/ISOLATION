using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                ArmorItem : 방어구 아이템
                
                Equip() : 방어구 장착
                    - 방어구 데이터의 수치만큼 플레이어 능력치 상승
                Unequip() : 방어구 해제
                    - 방어구 데이터의 수치만큼 플레이어 능력치 하락
 */

public class ArmorItem : EquipmentItem
{
    public ArmorItemData ArmorData { get; private set; }
    public ArmorItem(ArmorItemData data) : base(data)
    {
        ArmorData = data;
    }
    
    public override void Equip()
    {
        DataManager.Instance.GetPlayerData().EquipItem(ArmorData.Defense, ArmorData.Type);
    }

    public override void Unequip()
    {
        DataManager.Instance.GetPlayerData().UnequipItem(ArmorData.Defense, ArmorData.Type);
    }
}
