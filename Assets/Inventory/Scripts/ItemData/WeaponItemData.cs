using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                WeaponItemData : ���� ������ ������
                
                ������ id : 30001 ~ 39999
                ������ : �����͸� �޾� �ʱ�ȭ
                CreateItem() : �ʱ�ȭ�� �����ͷ� ������ ��ü ����
 */

public class WeaponItemData : EquipmentItemData
{
    [SerializeField] private int damage;        // ���� ������
    [SerializeField] private float rate;        // ���ݼӵ�
    [SerializeField] private string type;       // ��� Ÿ��
    [SerializeField] private string subType;    // ��� ���� Ÿ��

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
