using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ - ��� ������
/// </summary>
public class MaterialItem : CountableItem, IUsableItem
{
    public MaterialItem(MaterialItemData data, int amount = 1) : base(data, amount) { }

    public bool Use()
    {
        Amount--;

        return true;
    }
}