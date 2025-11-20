using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private Transform itemSlotContent;
    [SerializeField] private Transform ingredientSlotParent;
    [SerializeField] private Button craftButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private GameObject ingredientSlotPrefab;

    [Header("Settings")]
    [SerializeField] private float slotSize = 80f;
    [SerializeField] private float slotSpacing = 20f;
    [SerializeField] private int columnCount = 6;

    [Header("Data")]
    [SerializeField] private Inventory inventory;

    private List<RecipeData> allRecipes;
    private RecipeData currentRecipe;

    private List<CraftingItemSlotUI> createdSlots new List<CraftingItemSlotUI>();

    private void Start()
    {
        allRecipes = DataManager.Instance.RecipeList;
        CloseUI();
        craftButton.onClick.AddListener(tryCraftItem);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (backgroundPanel.activeSelf)
            {
                CloseUI();
            }
        }
    }

    public void OpenUI()
    {
        backgroundPanel.SetActive(true);
        ShowRecipeList("All");
        clearIngredients();
        currentRecipe = null;
        craftButton.interactable = false;
    }

    public void CloseUI()
    {
        backgroundPanel.SetActive(false);
    }

    public void ShowRecipeList(string categoty)
    {
        foreach (Transform child in itemSlotContent) Destroy(child.gameObject);
        createdSlots.Clear();

        // 현재는 모든 레시피를 보여주지만, 카테고리에 따라 필터링할 수 있도록 수정 필요
        List<RecipeData> fiteredRecipes = new List<RecipeData>();
        foreach (var recipe in allRecipes)
        {
            // 카테고리 필터링 로직 추가 필요
            fiteredRecipes.Add(recipe);
        }

        int totalRows = Mathf.CeilToInt((float)fiteredRecipes.Count / columnCount);
        float contentHeight = (totalRows * slotSize) + ((totalRows + 1) * slotSpacing);
        RectTransform contentRect = itemSlotContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentHeight);

        for (int i = 0; i < fiteredRecipes.Count; i++)
        {
            RecipeData recipe = fiteredRecipes[i];
            GameObject go = Instantiate(itemSlotPrefab, itemSlotContent);

            int row = i / columnCount;
            int col = i % columnCount;

            float posX = slotSpacing + (col * (slotSize + slotSpacing));
            float posY = -(slotSpacing + (row * (slotSize + slotSpacing)));

            RectTransform slotRect = go.GetComponent<RectTransform>();
            slotRect.pivot = new Vector2(0, 1);
            slotRect.anchorMin = new Vector2(0, 1);
            slotRect.anchorMax = new Vector2(0, 1);
            slotRect.anchoredPosition = new Vector2(posX, posY);
            slotRect.sizeDelta = new Vector2(slotSize, slotSize);

            CraftingItemSlotUI slotUI = go.GetComponent<CraftingItemSlotUI>();
            ItemData resultItem = DataManager.Instance.GetItemDataByID(recipe.resultItemID);
            bool isCraftable = checkIfCraftable(recipe);

            slotUI.Setup(this, recipe, resultItem, isCraftable);
            createdSlots.Add(slotUI);
        }
    }

    public void OnRecipeSelected(RecipeData recipe)
    {
        currentRecipe = recipe;

        foreach (var ingredient in recipe.ingredients)
        {
            GameObject go = Instantiate(ingredientSlotPrefab, ingredientSlotParent);
            CraftingIngredientSlotUI ingredientSlot = go.GetComponent<CraftingIngredientSlotUI>();

            ItemData itemData = DataManager.Instance.GetItemDataByID(ingredient.itemID);
            int hasAmount = inventory.GetTotalAmountOfItem(ingredient.itemID);

            ingredientSlot.Setup(itemData, ingredient.amount, hasAmount);
        }
    }

    private void clearIngredients()
    {
        foreach (Transform child in ingredientSlotParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void updateCraftButtonState()
    {
        bool canCraft = checkIfCraftable(currentRecipe);
        craftButton.interactable = canCraft;
    }    

    private bool checkIfCraftable(RecipeData recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int hasAmount = inventory.GetTotalAmountOfItem(ingredient.itemID);
            if (hasAmount < ingredient.amount)
            {
                return false;
            }
        }
        return true;
    }

    private void tryCraftItem()
    {
        if (currentRecipe == null || !checkIfCraftable(currentRecipe)) return;

        foreach (var ingredient in currentRecipe.ingredients)
        {
            inventory.ConsumeItem(ingredient.itemID, ingredient.amount);
        }

        ItemData resultItem = DataManager.Instance.GetItemDataByID(currentRecipe.resultItemID);
        inventory.AddItem(resultItem, currentRecipe.resultAmount);

        ShowRecipeList("All");
        OnRecipeSelected(currentRecipe);
    }
}
