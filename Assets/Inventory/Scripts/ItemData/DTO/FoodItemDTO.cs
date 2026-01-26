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
    public float hydrationValue; // 수분 회복량
    public List<ItemEffect> effects; // 상태이상 효과 리스트
}
