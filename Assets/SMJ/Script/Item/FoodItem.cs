using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : CountableItem, IUsableItem
{
    public FoodItem(FoodItemData data, int amount = 1) : base(data, amount) { }

    private FoodItemData data => CountableData as FoodItemData;

    public bool Use()
    {
        bool success = DataManager.Instance.GetPlayerData().IncreaseSatiety(data.Value);
        
        if (!success)
            return false;

        Amount--;

        return true;
    }
}