using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
            EquipmentItemData : 방어구 아이템 데이터
            
 */

public enum EquipmentType
{
    RightHand,
    LeftHand,
    Face,
    Back,
    None
}

public abstract class EquipmentItemData : ItemData
{
    public EquipmentType TargetSlot { get; protected set; }

    protected float maxDurability;   
    public float MaxDurability => maxDurability;
}
