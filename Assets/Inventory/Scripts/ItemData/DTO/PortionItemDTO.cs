using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
                PortionItemData 직렬화 전용 클래스
 */

[System.Serializable]
public class PortionItemDTO 
{
    public int id;                      // 아이템 id
    public string itemName;             // 아이템 이름
    public string itemToolTip;          // 아이템 툴팁
    public string itemExplanation;      // 아이템 효과 설명 텍스트
    public string itemIcon;             // 아이템 아이콘 이름
    public int itemPrice;               // 아이템 가격
    public int maxAmount;               // 최대 소지량
    public string portionType;          // 포션 종류
    public float value;                 // 회복량
    
}
