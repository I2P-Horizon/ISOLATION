using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                PortionItem : ���� ������
                
                Use() : ���� ���
                    - ���� �ϳ� ����
                    - �÷��̾� ü�� �� ���� ȸ��
 */

public class PortionItem : CountableItem, IUsableItem
{
    public PortionItemData PortionData { get; private set; }
    public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) 
    {
        PortionData = data;
    }

    // ���� ���
    public bool Use()
    {
        Amount--;

        DataManager.Instance.GetPlayerData().UsePortion(PortionData.Value, PortionData.PortionType);

        return true;
    }
}
