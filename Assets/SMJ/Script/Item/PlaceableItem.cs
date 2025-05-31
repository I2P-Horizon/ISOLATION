using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    /// <summary>
    /// 수량 아이템 - 설치 아이템
    /// </summary>
    public class PlaceableItem : CountableItem, IUsableItem
    {
        public PlaceableItem(PlaceableItemData data, int amount = 1) : base(data, amount) { }

        public bool Use()
        {
            Amount--;

            // 추후 설치 시스템 구현

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PlaceableItem(CountableData as PlaceableItemData, amount);
        }
    }
}