using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _craftingUIGo;

    private void OnEnable()
    {
        if (_craftingUIGo == null)
        {
            _craftingUIGo = GameObject.FindWithTag("CraftingUI");
        }
    }

    public void Interact(object context = null)
    {
        // 제작 UI 열기
        if (_craftingUIGo != null)
        {
            _craftingUIGo.GetComponentInChildren<UIAnimator>(true).Show();
        }
    }
}
