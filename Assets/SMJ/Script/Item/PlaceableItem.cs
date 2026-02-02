using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 수량 아이템 - 설치 아이템
/// </summary>
public class PlaceableItem : CountableItem, IUsableItem
{
    public PlaceableItem(PlaceableItemData data, int amount = 1) : base(data, amount) { }

    public bool Use()
    {
        if (PlacementManager.Instance != null)
        {
            PlacementManager.Instance.BeginPlacement(this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 배치 확정 시 외부에서 호출되는 함수
    /// </summary>
    public void OnPlaced()
    {
        GameObject inventory = GameObject.FindWithTag("Inventory");
        if (inventory != null)
        {
            Inventory inv = inventory.GetComponent<Inventory>();
            int index = inv.GetItemIndexByID(Data.ID);
            if (index >= 0)
            {
                inv.ConsumeItem(Data.ID, 1);
            }
        }
    }
}