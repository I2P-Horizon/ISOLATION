using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemGroupUI : MonoBehaviour
{
    [SerializeField] private List<PlayerItemSlotUI> slots = new();           // 슬롯 데이터
    [SerializeField] private Inventory inventory;

    private const int QUICK_SLOT_COUNT = 6;

    private void Start()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Index = i;
        }
    }

    public void UpdateSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Item item = inventory.GetItem(i);

            if (item != null)
            {
                slots[i].SetItem(item);
            }
            else
            {
                slots[i].RemoveItem();
            }
        }
    }

    // 삭제할 아이템을 참조하는 슬롯 제거
    public void RemoveItem(Item item)
    {
        for(int i = 0;i<slots.Count;i++)
        {
            if(slots[i].HasItem(item))
            {
                slots[i].RemoveItem();
            }
        }
    }

    public void RemoveItemAt(int index)
    {
        slots[index].RemoveItem();
    }

    // 슬롯에 아이템 등록
    public void SetItemIconAndAmountText(int index, Item item)
    {
        slots[index].SetItem(item);
    }

}
