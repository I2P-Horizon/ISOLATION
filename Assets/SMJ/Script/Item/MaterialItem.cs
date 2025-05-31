using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    /// <summary>
    /// 수량 아이템 - 재료 아이템
    /// </summary>
    public class MaterialItem : CountableItem, IUsableItem
    {
        public MaterialItem(MaterialItemData data, int amount = 1) : base(data, amount) { }

        public bool Use()
        {
            Amount--;

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new MaterialItem(CountableData as MaterialItemData, amount);
        }
    }
}