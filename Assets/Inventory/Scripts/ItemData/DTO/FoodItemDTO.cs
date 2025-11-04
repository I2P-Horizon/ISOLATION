using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodItemDTO
{
    public int id;
    public string itemName;
    public string itemToolTip;
    public string itemExplanation;
    public string itemIcon;
    public int itemPrice;
    public int maxAmount;
    public float value; // 포만감 회복량
}
