using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
                PortionItemData ����ȭ ���� Ŭ����
 */

[System.Serializable]
public class PortionItemDTO 
{
    public int id;                      // ������ id
    public string itemName;             // ������ �̸�
    public string itemToolTip;          // ������ ����
    public string itemExplanation;      // ������ ȿ�� ���� �ؽ�Ʈ
    public string itemIcon;             // ������ ������ �̸�
    public int itemPrice;               // ������ ����
    public int maxAmount;               // �ִ� ������
    public string portionType;          // ���� ����
    public float value;                 // ȸ����
    
}
