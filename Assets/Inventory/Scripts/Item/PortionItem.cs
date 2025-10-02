using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                PortionItem : 포션 아이템
                
                Use() : 포션 사용
                    - 갯수 하나 차감
                    - 플레이어 체력 및 마나 회복
 */

public class PortionItem : CountableItem, IUsableItem
{
    public PortionItemData PortionData { get; private set; }
    public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) 
    {
        PortionData = data;
    }

    // 포션 사용
    public bool Use()
    {
        Amount--;

        DataManager.Instance.GetPlayerData().UsePortion(PortionData.Value, PortionData.PortionType);

        return true;
    }
}
