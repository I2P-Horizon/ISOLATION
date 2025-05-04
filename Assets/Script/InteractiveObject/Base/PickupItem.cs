using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾�� ��ȣ�ۿ��Ͽ� �κ��丮�� ����� �� �ִ� ������Ʈ�� ���� ��� Ŭ����.
/// </summary>
public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData;

    /// <summary>
    /// �÷��̾ �������� �ֿ��� �� ȣ��Ǵ� �Լ�.
    /// </summary>
    public void Interact(object context = null)
    {
        Destroy(gameObject);
    }
}
