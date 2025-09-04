using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 설치 아이템 정보
/// </summary>
public class PlaceableItemData : CountableItemData
{
    public string PrefabName { get; private set; } // 설치될 프리팹 이름

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