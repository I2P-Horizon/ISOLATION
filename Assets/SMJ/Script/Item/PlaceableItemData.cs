using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    /// <summary>
    /// 설치 아이템 정보
    /// </summary>
    [CreateAssetMenu(fileName = "Item_Placeable_", menuName = "Inventory System/Item Data/Placeable", order = 5)]
    public class PlaceableItemData : CountableItemData
    {
        public override Item CreateItem()
        {
            return new PlaceableItem(this);
        }
    }
}