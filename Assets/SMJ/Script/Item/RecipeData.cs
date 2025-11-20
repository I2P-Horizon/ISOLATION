using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredient
{
    public int itemID;
    public int amount;
}

[System.Serializable]
public class RecipeData
{
    public int resultItemID;
    public int resultAmount;
    public List<RecipeIngredient> ingredients;
}
