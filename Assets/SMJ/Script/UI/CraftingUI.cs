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
    [SerializeField] private int rowCount = 3;
    [SerializeField] private int maxIngredientSlots = 5;

    [Header("Data")]
    [SerializeField] private Inventory inventory;

    private RecipeData currentRecipe;

    private List<CraftingItemSlotUI> createdSlots = new List<CraftingItemSlotUI>();
    private List<IngredientSlotUI> ingredientSlots = new List<IngredientSlotUI>();

    private void Start()
    {
        initRecipeSlots();
        initIngredientSlots();

        CloseUI();
        craftButton.onClick.AddListener(tryCraftItem);
    }

    private void OnEnable()
    {
        ShowRecipeList("All");
        clearIngredients();
        currentRecipe = null;
        craftButton.interactable = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (backgroundPanel.activeSelf)
            {
                this.gameObject.GetComponent<UIAnimator>().Close();
            }
        }
    }

    private void initRecipeSlots()
    {
        int totalSlots = columnCount * rowCount;

        float contentHeight = (rowCount * slotSize) + ((rowCount + 1) * slotSpacing);
        RectTransform contentRect = itemSlotContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentHeight);

        for (int i = 0; i < totalSlots; i++)
        {
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
            slotUI.SetEmpty();
            createdSlots.Add(slotUI);
        }
    }

    private void initIngredientSlots()
    {
        for (int i = 0; i < maxIngredientSlots; i++)
        {
            GameObject go = Instantiate(ingredientSlotPrefab, ingredientSlotParent);
            IngredientSlotUI ingredientSlot = go.GetComponent<IngredientSlotUI>();
            ingredientSlot.SetEmpty();
            ingredientSlots.Add(ingredientSlot);
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

    public void ShowRecipeList(string category)
    {
        List<RecipeData> recipes = DataManager.Instance.RecipeList;

        //for (int i = 0; i < createdSlots.Count; i++)
        //{
        //    if (i < recipes.Count)
        //    {
        //        RecipeData recipe = recipes[i];
        //        ItemData resultItem = DataManager.Instance.GetItemDataByID(recipe.resultItemID);
        //        bool isCraftable = checkIfCraftable(recipe);

        //        createdSlots[i].Setup(this, recipe, resultItem, isCraftable);
        //    }    
        //    else
        //    {
        //        createdSlots[i].SetEmpty();
        //    }
        //}    

        for (int i = 0; i < createdSlots.Count; i++)
        {
            if (i < recipes.Count)
            {
                RecipeData recipe = recipes[i];
                ItemData resultData = DataManager.Instance.GetItemDataByID(recipe.resultItemID);

                if (resultData == null)
                {
                    Debug.LogError($"[오류] 레시피의 결과물 ID({recipe.resultItemID})에 해당하는 아이템 데이터가 없습니다! JSON ID 확인 필요.");
                }
                else
                {
                    Sprite iconSprite = Resources.Load<Sprite>($"Icon/{resultData.ItemIcon}");
                    if (iconSprite == null)
                    {
                        Debug.LogError($"[오류] 아이템({resultData.ItemName})의 아이콘({resultData.ItemIcon})을 Resources/Icons 폴더에서 찾을 수 없습니다!");
                    }
                    else
                    {
                        Debug.Log($"[성공] 아이템({resultData.ItemName}) 로드 성공. 아이콘: {resultData.ItemIcon}");
                    }
                }

                bool isCraftable = checkIfCraftable(recipe);
                createdSlots[i].Setup(this, recipe, resultData, isCraftable);
            }
            else
            {
                createdSlots[i].SetEmpty();
            }
        }
    }

    public void OnRecipeSelected(RecipeData recipe)
    {
        currentRecipe = recipe;

        foreach (var slot in createdSlots)
        {
            slot.SetSelected(slot.recipeData == recipe);
        }

        updateIngredientUI(recipe);
        updateCraftButtonState(recipe);
    }

    private void updateIngredientUI(RecipeData recipe)
    {
        for (int i = 0; i < ingredientSlots.Count; i++)
        {
            if (i < recipe.ingredients.Count)
            {
                var ing = recipe.ingredients[i];
                ItemData data = DataManager.Instance.GetItemDataByID(ing.itemID);
                int hasAmount = inventory.GetTotalAmountOfItem(ing.itemID);

                ingredientSlots[i].Setup(data, ing.amount, hasAmount);
            }
            else
            {
                ingredientSlots[i].SetEmpty();
            }
        }
    }

    private void clearIngredients()
    {
        foreach (var slot in ingredientSlots) slot.SetEmpty();
    }

    private void updateCraftButtonState(RecipeData recipe)
    {
        bool canCraft = checkIfCraftable(recipe);
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

    private void refreshAllSlotsState()
    {
        foreach (var slot in createdSlots)
        {
            if (slot.recipeData != null)
            {
                bool isCraftable = checkIfCraftable(slot.recipeData);
                slot.UpdateCraftability(isCraftable);
            }
        }
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
        refreshAllSlotsState();
    }
}
