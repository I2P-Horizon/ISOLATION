using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ - ��ġ ������
/// </summary>
public class PlaceableItem : CountableItem, IUsableItem
{
    public PlaceableItem(PlaceableItemData data, int amount = 1) : base(data, amount) { }

    public bool Use()
    {
        Amount--;

        // ���� ��ġ �ý��� ����

        return true;
    }
}