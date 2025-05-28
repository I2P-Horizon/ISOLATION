using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                WeaponItemData 직렬화 전용 클래스
 */

[System.Serializable]
public class WeaponItemDTO
{
    public int id;                      // 아이템 id
    public string itemName;             // 아이템 이름
    public string itemToolTip;          // 아이템 툴팁
    public string itemExplanation;      // 아이템 효과 설명 텍스트
    public string itemIcon;             // 아이템 아이콘 이름
    public string itemPrefab;           // 아이템 프리팹 이름
    public int itemPrice;               // 아이템 가격
    public int damage;                  // 무기 데미지
    public float rate;                  // 공격속도
    public string type;                 // 장비 타입
    public string subType;              // 장비 세부 타입

}
