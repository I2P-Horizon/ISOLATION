using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와 상호작용하여 인벤토리에 저장될 수 있는 오브젝트의 공통 기반 클래스.
/// </summary>
public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData;

    /// <summary>
    /// 플레이어가 아이템을 주웠을 때 호출되는 함수.
    /// </summary>
    public void Interact(object context = null)
    {
        Destroy(gameObject);
    }
}
