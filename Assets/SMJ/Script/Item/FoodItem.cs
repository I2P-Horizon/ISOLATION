using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : CountableItem, IUsableItem
{
    public FoodItem(FoodItemData data, int amount = 1) : base(data, amount) { }

    private FoodItemData data => CountableData as FoodItemData;

    public bool Use()
    {
        Player player = Player.Instance;

        if (player == null || player.State.Die)
            return false;

        bool satietyRecovered = player.State.IncreaseSatiety(data.SatietyValue);
        bool hydrationRecovered = player.State.IncreaseHydration(data.HydrationValue);

        // 이미 포만감과 수분량이 최대치라면 사용 실패 처리
        if (!satietyRecovered && !hydrationRecovered && data.Effects.Count == 0)
            return false;

        applyEffects(player);

        Amount--;
        return true;
    }

    private void applyEffects(Player player)
    {
        foreach (var effect in data.Effects)
        {
            float randomValue = Random.value;

            if (randomValue <= effect.chance)
            {
                if (effect.isRemoval)
                {
                    player.Condition.RemoveCondition(effect.type);
                    Debug.Log($"[FoodItem] Removed condition: {effect.type}");
                }
                else
                {
                    player.Condition.AddCondition(effect.type, effect.duration);
                    Debug.Log($"[FoodItem] Applied condition: {effect.type} for {effect.duration} seconds");
                }
            }
        }
    }
}