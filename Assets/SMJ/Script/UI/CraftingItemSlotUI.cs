using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button slotButton;
    [SerializeField] private GameObject selectionOutline;

    public RecipeData recipeData { get; private set; }
    private CraftingUI manager;

    public void Setup(CraftingUI uimanager, RecipeData recipe, ItemData itemData, bool isCraftable)
    {
        manager = uimanager;
        recipeData = recipe;

        iconImage.gameObject.SetActive(true);
        if (itemData != null)
            iconImage.sprite = Resources.Load<Sprite>($"Icon/{itemData.ItemIcon}");

        Debug.Log($"{itemData.ItemName} isCraftable: {isCraftable}");
        iconImage.color = isCraftable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.4f);

        slotButton.interactable = true;
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => manager.OnRecipeSelected(recipeData));

        SetSelected(false);
    }

    public void SetEmpty()
    {
        recipeData = null;
        iconImage.gameObject.SetActive(false);
        slotButton.interactable = false;
        slotButton.onClick.RemoveAllListeners();

        if (selectionOutline != null) selectionOutline.SetActive(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionOutline != null)
            selectionOutline.SetActive(isSelected);
    }

    public void UpdateCraftability(bool isCraftable)
    {
        if (recipeData == null) return;

        iconImage.color = isCraftable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.4f);
    }
}
