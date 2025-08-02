using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                ArmorItemData : 방어구 아이템 데이터
                
                아이템 id : 20001 ~ 29999
                생성자 : 데이터를 받아 초기화
                CreateItem() : 초기화된 데이터로 아이템 객체 생성
 */

public class ArmorItemData : EquipmentItemData
{
    [SerializeField] private int defense;       // 방어력
    [SerializeField] private string type;       // 장비타입
    [SerializeField] private string subType;    // 장비 세부타입

    public int Defense => defense;
    public string Type => type;
    public string SubType => subType;

    public ArmorItemData(ArmorItemDTO dto)
    {
        this.id = dto.id;
        this.itemName = dto.itemName;
        this.itemToolTip = dto.itemToolTip;
        this.itemExplanation = dto.itemExplanation;
        this.itemIcon = dto.itemIcon;
        this.itemPrice = dto.itemPrice;
        this.defense = dto.defense;
        this.type = dto.type;
        this.subType = dto.subType;
    }

    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }
}
