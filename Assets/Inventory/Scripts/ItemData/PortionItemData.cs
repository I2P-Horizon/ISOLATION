using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                PortionItemData : 포션 아이템 데이터
                
                아이템 id : 10001 ~ 19999
                생성자 : 데이터를 받아 초기화
                CreateItem() : 초기화된 데이터로 아이템 객체 생성
 */

public class PortionItemData : CountableItemData
{
    [SerializeField] private float value;              // 회복량
    [SerializeField] private string portionType;       // 포션 종류

    public float Value => value;
    public string PortionType => portionType;

    public PortionItemData(PortionItemDTO dto)
    {
        this.id = dto.id;
        this.itemName = dto.itemName;
        this.itemToolTip = dto.itemToolTip;
        this.itemExplanation = dto.itemExplanation;
        this.itemIcon = dto.itemIcon;
        this.itemPrice = dto.itemPrice;
        this.maxAmount = dto.maxAmount;
        this.value = dto.value;
        this.portionType = dto.portionType;
    }

    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}
