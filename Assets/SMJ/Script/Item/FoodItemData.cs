using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditorInternal;
using UnityEngine;

public class FoodItemData : CountableItemData
{
    public float Value { get; private set; } // 포만감 회복량

    public FoodItemData(FoodItemDTO dto)
    {
        this.id = dto.id;
        this.itemName = dto.itemName;
        this.itemToolTip = dto.itemToolTip;
        this.itemExplanation = dto.itemExplanation;
        this.itemIcon = dto.itemIcon;
        this.itemPrice = dto.itemPrice;
        this.maxAmount = dto.maxAmount;

        this.Value = dto.value;
    }

    public override Item CreateItem()
    {
        return new FoodItem(this);
    }
}