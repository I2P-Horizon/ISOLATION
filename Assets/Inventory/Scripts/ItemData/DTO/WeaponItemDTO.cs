using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                WeaponItemData ����ȭ ���� Ŭ����
 */

[System.Serializable]
public class WeaponItemDTO
{
    public int id;                      // ������ id
    public string itemName;             // ������ �̸�
    public string itemToolTip;          // ������ ����
    public string itemExplanation;      // ������ ȿ�� ���� �ؽ�Ʈ
    public string itemIcon;             // ������ ������ �̸�
    public string itemPrefab;           // ������ ������ �̸�
    public int itemPrice;               // ������ ����
    public int damage;                  // ���� ������
    public float rate;                  // ���ݼӵ�
    public string type;                 // ��� Ÿ��
    public string subType;              // ��� ���� Ÿ��

}
