using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                ItemData
                    - EquipmentItemData
                        - WeaponItemData
                        - ArmorItemData
                            - �� ���� : "Top", "Gloves", "Shoes" 
 */

public abstract class ItemData 
{
    [SerializeField] protected int id;                        // ������ id
    [SerializeField] protected string itemName;               // ������ �̸�
    [SerializeField] protected string itemToolTip;            // ������ ����
    [SerializeField] protected string itemIcon;               // ������ ������ �̸�
    [SerializeField] protected string itemPrefab;             // ������ ������ �̸�
    [SerializeField] protected string itemExplanation;        // ������ ����
    [SerializeField] protected int    itemPrice;              // ������ ����

    public int ID => id;
    public string ItemName => itemName;
    public string ItemToolTip => itemToolTip;
    public string ItemIcon => itemIcon;
    public string ItemPrefab => itemPrefab;
    public string ItemExplanation => itemExplanation;
    public int ItemPrice => itemPrice;

    // ������ ����
    public abstract Item CreateItem();  
}
