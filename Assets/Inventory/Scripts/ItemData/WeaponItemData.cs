using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                WeaponItemData : 무기 아이템 데이터
                
                아이템 id : 30001 ~ 39999
                생성자 : 데이터를 받아 초기화
                CreateItem() : 초기화된 데이터로 아이템 객체 생성
 */

public class WeaponItemData : EquipmentItemData
{
    [SerializeField] private int damage;        // 무기 데미지
    [SerializeField] private float rate;        // 공격속도
    [SerializeField] private string type;       // 장비 타입
    [SerializeField] private string subType;    // 장비 세부 타입

    public int Damage => damage;
    public float Rate => rate;
    public string Type => type;
    public string SubType => subType;
    public WeaponItemData(WeaponItemDTO dto)
    {
        this.id = dto.id;
        this.itemName = dto.itemName;
        this.itemToolTip = dto.itemToolTip;
        this.itemExplanation = dto.itemExplanation;
        this.itemIcon = dto.itemIcon;
        this.itemPrefab = dto.itemPrefab;
        this.itemPrice = dto.itemPrice;
        this.damage = dto.damage;
        this.rate = dto.rate;
        this.type = dto.type;
        this.subType = dto.subType;
    }

    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}
