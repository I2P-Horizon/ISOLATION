using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 플레이어와 상호작용하여 인벤토리에 저장될 수 있는 오브젝트의 공통 기반 클래스.
/// </summary>
public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private bool _canBePickedUp = true;

    public ItemData ItemData => _itemData;

    /// <summary>
    /// 현재 아이템을 주울 수 있는 상태인지 확인하는 함수.
    /// </summary>
    /// <returns>주울 수 있으면 true</returns>
    public bool CanBePickedUp()
    {
        // 기본은 true이지만 자식 오브젝트에서 확장하여 사용 가능
        return _canBePickedUp;
    }

    /// <summary>
    /// 아이템을 주울 수 있는지 아닌지 설정
    /// </summary>
    public void SetPickupState(bool state)
    {
        _canBePickedUp = state;
    }

    /// <summary>
    /// 플레이어가 아이템을 주웠을 때 호출되는 함수.
    /// </summary>
    public void Interact(object context = null)
    {
        if (!_canBePickedUp) return;
        Destroy(gameObject);
    }
}