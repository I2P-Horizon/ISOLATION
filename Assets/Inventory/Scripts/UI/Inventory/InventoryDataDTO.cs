using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDataDTO
{
    public int slotIndex;                   // 슬롯 인덱스
    public int itemId;                      // 아이템 id
    public int amount;                      // 아이템갯수
    public bool isAccessibleSlot;           // 슬롯 활성화여부
}
