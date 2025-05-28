using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
            CountableItemData : ������ �ִ� ������ ������
            
            - �� ���Կ� ���� �� �ִ� �ִ� ������ ����
 */
public abstract class CountableItemData : ItemData
{
    [SerializeField] protected int maxAmount;             // �ִ� ����
    public int MaxAmount => maxAmount;
}
