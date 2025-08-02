using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemGroupUI : MonoBehaviour
{
    [SerializeField] private List<PlayerItemSlotUI> slots = new();           // ���� ������

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
