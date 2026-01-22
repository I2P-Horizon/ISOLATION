using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField] private CraftingUI _craftingUI;

    private void OnEnable()
    {
        if (_craftingUI == null)
        {
            GameObject craftingGo = GameObject.FindWithTag("CraftingUI");
            if (craftingGo != null)
            {
                _craftingUI = craftingGo.GetComponentInChildren<CraftingUI>(true);
            }
        }
    }

    public void Interact(object context = null)
    {
        // 제작 UI 열기
        if (_craftingUI != null)
        {
            _craftingUI.OpenUI();
        }
    }
}
