using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;

    private RecipeData recipeData;

    public void Setup(RecipeData recipe, ItemData resultItemData)
    {
        recipeData = recipe;
        
        if (resultItemData != null)
        {
            itemIcon.sprite = Resources.Load<Sprite>($"Icon/{resultItemData.ItemIcon}");
            itemName.text = resultItemData.ItemName;
        }
        else
        {
            itemIcon.sprite = null;
            itemName.text = "Unknown Item";
        }
    }
}
