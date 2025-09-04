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
    [SerializeField] private int _itemID;

    public ItemData ItemData { get; private set; }

    private void Awake()
    {
        ItemData = FindItemDataByID(_itemID);
        if (ItemData == null)
        {
            Debug.LogError($"Item ID {_itemID}�� �ش��ϴ� ItemData�� ã�� �� �����ϴ�. {gameObject.name} ������Ʈ�� Ȯ���ϼ���.");
            _canBePickedUp = false;
            gameObject.SetActive(false);
        }
    }

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

    private ItemData FindItemDataByID(int id)
    {
        if (id > 10000 && id < 20000)
        {
            return DataManager.Instance.GetPortionDataById(id);
        }
        else if (id > 20000 && id < 30000)
        {
            return DataManager.Instance.GetArmorDataById(id);
        }
        else if (id > 30000 && id < 40000)
        {
            return DataManager.Instance.GetWeaponDataById(id);
        }
        else if (id > 40000 && id < 50000)
        {
            return DataManager.Instance.GetFoodDataById(id);
        }
        else if (id > 50000 && id < 60000)
        {
            return DataManager.Instance.GetMaterialDataById(id);
        }
        else if (id > 60000 && id < 70000)
        {
            return DataManager.Instance.GetPlaceableDataById(id);
        }

        return null;
    }
}