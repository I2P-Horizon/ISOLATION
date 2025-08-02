using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                PortionItemData : ���� ������ ������
                
                ������ id : 10001 ~ 19999
                ������ : �����͸� �޾� �ʱ�ȭ
                CreateItem() : �ʱ�ȭ�� �����ͷ� ������ ��ü ����
 */

public class PortionItemData : CountableItemData
{
    [SerializeField] private float value;              // ȸ����
    [SerializeField] private string portionType;       // ���� ����

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
