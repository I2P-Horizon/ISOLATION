using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemGroupUI : MonoBehaviour
{
    [SerializeField] private List<PlayerItemSlotUI> slots = new();           // ���� ������
    [SerializeField] private Inventory inventory;

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            slots[0].UseItem(inventory);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            slots[1].UseItem(inventory);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            slots[2].UseItem(inventory);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            slots[3].UseItem(inventory);
        }
    }

    // ��� ���� ����
    public void UpdateSlots()
    {
        for(int i = 0;i<slots.Count;i++)
        {
            slots[i].UpdateSlot();
        }
    }

    // ������ �������� �����ϴ� ���� ����
    public void RemoveItem(CountableItem ci)
    {
        for(int i = 0;i<slots.Count;i++)
        {
            if(slots[i].HasItem(ci))
            {
                slots[i].RemoveItem();
            }
        }
    }

    // ���Կ� ������ ���
    public void SetItemIconAndAmountText(int index, CountableItem ci)
    {
        slots[index].SetItem(ci);
    }

}
