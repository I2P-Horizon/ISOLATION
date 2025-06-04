using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �÷��̾�� ��ȣ�ۿ��Ͽ� �κ��丮�� ����� �� �ִ� ������Ʈ�� ���� ��� Ŭ����.
/// </summary>
public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private bool _canBePickedUp = true;

    public ItemData ItemData => _itemData;

    /// <summary>
    /// ���� �������� �ֿ� �� �ִ� �������� Ȯ���ϴ� �Լ�.
    /// </summary>
    /// <returns>�ֿ� �� ������ true</returns>
    public bool CanBePickedUp()
    {
        // �⺻�� true������ �ڽ� ������Ʈ���� Ȯ���Ͽ� ��� ����
        return _canBePickedUp;
    }

    /// <summary>
    /// �������� �ֿ� �� �ִ��� �ƴ��� ����
    /// </summary>
    public void SetPickupState(bool state)
    {
        _canBePickedUp = state;
    }

    /// <summary>
    /// �÷��̾ �������� �ֿ��� �� ȣ��Ǵ� �Լ�.
    /// </summary>
    public void Interact(object context = null)
    {
        if (!_canBePickedUp) return;
        Destroy(gameObject);
    }
}