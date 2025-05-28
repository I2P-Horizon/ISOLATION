using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                WeaponItem : ���� ������
                
                Equip() : ���� ����
                    - ���� �������� ��ġ��ŭ �÷��̾� �ɷ�ġ ���
                Unequip() : �� ����
                    - ���� �������� ��ġ��ŭ �÷��̾� �ɷ�ġ �϶�
 */

public class WeaponItem : EquipmentItem
{
    public WeaponItemData WeaponData { get; private set; }
    public WeaponItem(WeaponItemData data) : base(data) 
    {
        WeaponData = data;
    }

    // ����
    public override void Equip()
    {
        // ��� ���� �÷��̾� �ɷ�ġ �ݿ�
        DataManager.Instance.GetPlayerData().EquipItem(WeaponData.Damage, WeaponData.Type);
        // ��� ���� �÷��̾� ���� ����
        WeaponManager.Instance.SetWeapon(WeaponData.SubType, WeaponData.ItemPrefab);
    }

    // ���� ����
    public override void Unequip()
    {
        // ��� ���� �÷��̾� �ɷ�ġ �ݿ�
        DataManager.Instance.GetPlayerData().UnequipItem(WeaponData.Damage, WeaponData.Type);
        // �⺻ ���� ����
        WeaponManager.Instance.SetWeapon();
    }
}
