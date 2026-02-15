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
    public bool Use(int index = -1)
    {
        Debug.Log($"[Portion] Use portion item: {PortionData.ItemName}");
        Player player = Player.Instance;

        if (player == null || player.State.Die) return false;

        player.State.IncreaseHp(PortionData.HpValue);

        applyEffects(player);

        Amount--;
        return true;
    }

    private void applyEffects(Player player)
    {
        foreach (var effect in PortionData.Effects)
        {
            float randomValue = Random.value;

            if (randomValue <= effect.chance)
            {
                if (effect.isRemoval)
                {
                    player.Condition.RemoveCondition(effect.type);
                    Debug.Log($"[PortionItem] Remove condition: {effect.type}");
                }
                else
                {
                    player.Condition.AddCondition(effect.type, effect.duration);
                    Debug.Log($"[PortionItem] Applied condition: {effect.type}");
                }
            }
        }
    }
}
