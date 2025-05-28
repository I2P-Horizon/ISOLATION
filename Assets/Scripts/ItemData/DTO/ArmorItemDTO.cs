using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                ArmorItemData ����ȭ ���� Ŭ����
 */
[System.Serializable]
public class ArmorItemDTO
{
    public int id;                      // ������ id
    public string itemName;             // ������ �̸�
    public string itemToolTip;          // ������ ����
    public string itemExplanation;      // ������ ȿ�� ���� �ؽ�Ʈ
    public string itemIcon;             // ������ ������ �̸�
    public int itemPrice;               // ������ ����
    public int defense;                 // ����
    public string type;                 // ���Ÿ��
    public string subType;              // ��񼼺�Ÿ��
}
