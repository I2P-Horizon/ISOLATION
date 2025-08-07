using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                EquipmentItem : 장착 아이템
                
            - Equip() : 장비 장착
            - UnEquip() : 장비 해제
 */
public abstract class EquipmentItem : Item, IEquipableItem
{
    public EquipmentItemData EquipmentData { get; private set; }

    public EquipmentItem(EquipmentItemData data) : base(data)
    {
        EquipmentData = data;
    }

    public virtual void Equip() { }
    public virtual void Unequip() { }
}
