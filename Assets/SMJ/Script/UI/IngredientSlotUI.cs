using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text infoText;

    public void Setup(ItemData ingredientData, int requiredAmount, int ownedAmount)
    {
        if (ingredientData != null)
        {
            itemIcon.sprite = Resources.Load<Sprite>($"Icon/{ingredientData.ItemIcon}");
            infoText.text = $"{ingredientData.ItemName}: {ownedAmount} / {requiredAmount}";

            if (ownedAmount < requiredAmount)
            {
                infoText.color = Color.red;
            }
            else
            {
                infoText.color = Color.green;
            }
        }
    }
}
