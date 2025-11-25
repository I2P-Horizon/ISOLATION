using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField] private CraftingUI _craftingUI;

    public void Interact(object context = null)
    {
        // 제작 UI 열기
        if (_craftingUI != null)
        {
            _craftingUI.OpenUI();
        }
    }
}
