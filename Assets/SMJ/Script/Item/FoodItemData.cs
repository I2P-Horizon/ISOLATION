using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public struct ItemEffect
{
    public ConditionType type;  // 상태 이상 종류
    public float chance;        // 발동 확률
    public float duration;      // 지속 시간 (-1이면 무한 지속)
    public bool isRemoval;     // true면 해제, false면 적용
}

public class FoodItemData : CountableItemData
{
    public float SatietyValue { get; private set; } // 포만감 회복량
    public float HydrationValue { get; private set; } // 수분 회복량
    public List<ItemEffect> Effects { get; private set; } // 상태 이상 효과 리스트

    public FoodItemData(FoodItemDTO dto)
    {
        this.id = dto.id;
        this.itemName = dto.itemName;
        this.itemToolTip = dto.itemToolTip;
        this.itemExplanation = dto.itemExplanation;
        this.itemIcon = dto.itemIcon;
        this.itemPrice = dto.itemPrice;
        this.maxAmount = dto.maxAmount;

        this.SatietyValue = dto.value;
        this.HydrationValue = dto.hydrationValue;

        this.Effects = dto.effects ?? new List<ItemEffect>();
    }

    public override Item CreateItem()
    {
        return new FoodItem(this);
    }
}