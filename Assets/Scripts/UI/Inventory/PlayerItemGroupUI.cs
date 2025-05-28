using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemGroupUI : MonoBehaviour
{
    [SerializeField] private List<PlayerItemSlotUI> slots = new();           // 슬롯 데이터
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

    // 모든 슬롯 갱신
    public void UpdateSlots()
    {
        for(int i = 0;i<slots.Count;i++)
        {
            slots[i].UpdateSlot();
        }
    }

    // 삭제할 아이템을 참조하는 슬롯 제거
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

    // 슬롯에 아이템 등록
    public void SetItemIconAndAmountText(int index, CountableItem ci)
    {
        slots[index].SetItem(ci);
    }

}
