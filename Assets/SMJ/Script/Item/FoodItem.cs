using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class FoodItem : CountableItem, IUsableItem
    {
        public FoodItem(FoodItemData data, int amount = 1) : base(data, amount) { }

        private FoodItemData data =>CountableData as FoodItemData;

        public bool Use()
        {
            Amount--;
            PlayerState.Instance.IncreaseSatiety(data.Value);

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new FoodItem(CountableData as FoodItemData, amount);
        }
    }
}