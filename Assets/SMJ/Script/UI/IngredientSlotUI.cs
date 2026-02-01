using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    public void Setup(ItemData itemData, int requiredAmount, int playerAmount)
    {
        gameObject.SetActive(true);
        iconImage.gameObject.SetActive(true);
        amountText.gameObject.SetActive(true);

        if (itemData == null) return;

        iconImage.sprite = Resources.Load<Sprite>($"Icon/{itemData.ItemIcon}");
        amountText.text = $"{playerAmount}/{requiredAmount}";
        amountText.color = (playerAmount >= requiredAmount) ? Color.white : Color.red;
    }

    public void SetEmpty()
    {
        iconImage.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
    }
}
