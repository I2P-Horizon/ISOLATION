using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                ArmorItem : �� ������
                
                Equip() : �� ����
                    - �� �������� ��ġ��ŭ �÷��̾� �ɷ�ġ ���
                Unequip() : �� ����
                    - �� �������� ��ġ��ŭ �÷��̾� �ɷ�ġ �϶�
 */

public class ArmorItem : EquipmentItem
{
    public ArmorItemData ArmorData { get; private set; }
    public ArmorItem(ArmorItemData data) : base(data)
    {
        ArmorData = data;
    }
    
    public override void Equip()
    {
        DataManager.Instance.GetPlayerData().EquipItem(ArmorData.Defense, ArmorData.Type);
    }

    public override void Unequip()
    {
        DataManager.Instance.GetPlayerData().UnequipItem(ArmorData.Defense, ArmorData.Type);
    }
}
