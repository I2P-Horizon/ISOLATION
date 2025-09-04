using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ġ ������ ����
/// </summary>
public class PlaceableItemData : CountableItemData
{
    public string PrefabName { get; private set; } // ��ġ�� ������ �̸�

    public PlaceableItemData(PlaceableItemDTO dto)
    {
        this.id = dto.id;
        this.itemName = dto.itemName;
        this.itemToolTip = dto.itemToolTip;
        this.itemExplanation = dto.itemExplanation;
        this.itemIcon = dto.itemIcon;
        this.itemPrice = dto.itemPrice;
        this.maxAmount = dto.maxAmount;
        this.PrefabName = dto.prefabName;
    }

    public override Item CreateItem()
    {
        return new PlaceableItem(this);
    }
}