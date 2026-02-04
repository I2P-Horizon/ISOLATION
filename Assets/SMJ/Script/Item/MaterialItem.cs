using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 수량 아이템 - 재료 아이템
/// </summary>
public class MaterialItem : CountableItem, IUsableItem
{
    public MaterialItem(MaterialItemData data, int amount = 1) : base(data, amount) { }

    public bool Use(int index = -1)
    {
        //Amount--;

        return true;
    }
}