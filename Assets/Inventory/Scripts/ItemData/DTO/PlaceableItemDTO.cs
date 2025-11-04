using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaceableItemDTO : MonoBehaviour
{
    public int id;
    public string itemName;
    public string itemToolTip;
    public string itemExplanation;
    public string itemIcon;
    public int itemPrice;
    public int maxAmount;
    public string prefabName; // 설치될 프리팹 이름
}
