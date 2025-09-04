using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ������ ����
/// </summary>
public class MaterialItemData : CountableItemData
{
    public MaterialItemData(MaterialItemDTO dto)
    {
        this.id = dto.id;
        this.itemName = dto.itemName;
        this.itemToolTip = dto.itemToolTip;
        this.itemExplanation = dto.itemExplanation;
        this.itemIcon = dto.itemIcon;
        this.itemPrice = dto.itemPrice;
        this.maxAmount = dto.maxAmount;
    }

    public override Item CreateItem()
    {
        return new MaterialItem(this);
    }
}