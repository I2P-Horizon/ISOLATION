using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                WeaponItem : 무기 아이템
                
                Equip() : 무기 장착
                    - 무기 데이터의 수치만큼 플레이어 능력치 상승
                Unequip() : 방어구 해제
                    - 무기 데이터의 수치만큼 플레이어 능력치 하락
 */

public class WeaponItem : EquipmentItem
{
    public WeaponItemData WeaponData { get; private set; }
    public WeaponItem(WeaponItemData data) : base(data) 
    {
        WeaponData = data;
    }

    // 장착
    public override void Equip()
    {
    }

    // 장착 해제
    public override void Unequip()
    {
    }
}
