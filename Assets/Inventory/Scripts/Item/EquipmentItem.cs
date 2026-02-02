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

    public float CurrentDurability { get; private set; }

    public EquipmentItem(EquipmentItemData data) : base(data)
    {
        EquipmentData = data;
        CurrentDurability = data.MaxDurability;
    }

    public virtual void Equip() { }
    public virtual void Unequip() { }

    public bool DecreaseDurability(float amout)
    {
        if (EquipmentData.MaxDurability <= 0) return false;

        CurrentDurability -= amout;

        if (CurrentDurability <= 0)
        {
            CurrentDurability = 0;
            return true;
        }

        return false;
    }
}
