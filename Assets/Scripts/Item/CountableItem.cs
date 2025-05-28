using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                CountableItem : �� �� �ִ� ������
                
                SetAmount(int amount) : 
                    - amount�� 0~MaxAmount �̸� amount�� ��ȯ
                    - amount�� ������ ����� 0 �Ǵ� MaxAmount�� ��ȯ
 */
public abstract class CountableItem : Item
{
    public CountableItemData CountableData { get; private set; }

    // ���� ������ ����
    public int Amount { get; protected set; }
    // �� ������ �ִ� ����
    public int MaxAmount => CountableData.MaxAmount;
    // ������ ����á���� ����
    public bool IsMax => Amount >= CountableData.MaxAmount;
    // ������ �ִ��� ����
    public bool IsEmpty => Amount <= 0;


    public CountableItem(CountableItemData data, int amount = 1) : base(data)
    {
        CountableData = data;
        SetAmount(amount);
    }

    // �� ������ ���� ���� ����
    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }

    // ���� ��ġ�� �� �ʰ��� ��ȯ
    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Amount + amount;
        SetAmount(nextAmount);

        // �ִ�ġ�� �ʰ��� �ʰ��� ��ȯ, �ʰ����������� 0 ��ȯ
        return (nextAmount > MaxAmount) ? (nextAmount - MaxAmount) : 0;
    }
}
