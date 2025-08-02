using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                EquipmentItem : ���� ������
                
            - Equip() : ��� ����
            - UnEquip() : ��� ����
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
