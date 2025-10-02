using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                ItemData
                    - EquipmentItemData
                        - WeaponItemData
                        - ArmorItemData
                            - 방어구 종류 : "Top", "Gloves", "Shoes" 
 */

public abstract class ItemData 
{
    [SerializeField] protected int id;                        // 아이템 id
    [SerializeField] protected string itemName;               // 아이템 이름
    [SerializeField] protected string itemToolTip;            // 아이템 툴팁
    [SerializeField] protected string itemIcon;               // 아이템 아이콘 이름
    [SerializeField] protected string itemPrefab;             // 아이템 프리팹 이름
    [SerializeField] protected string itemExplanation;        // 아이템 설명
    [SerializeField] protected int    itemPrice;              // 아이템 가격

    public int ID => id;
    public string ItemName => itemName;
    public string ItemToolTip => itemToolTip;
    public string ItemIcon => itemIcon;
    public string ItemPrefab => itemPrefab;
    public string ItemExplanation => itemExplanation;
    public int ItemPrice => itemPrice;

    // 아이템 생성
    public abstract Item CreateItem();  
}
