using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
                     PlayerItemSlotUI

                - �÷��̾� ������ ���� ����      
                - ���Կ� �������� �����ٳ����� �������� ���
                    - �κ��丮���� �������� ����ϰų� �����ϸ� �ݿ�
 */

public class PlayerItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text amount;
    public int index;

    private CountableItem slotItem;             // �� ������ ������


    private void ShowAmount() => amount.gameObject.SetActive(true);
    private void HideAmount() => amount.gameObject.SetActive(false);

    // ���� ������Ʈ
    public void UpdateSlot()
    {
        SetItem(slotItem);
    }

    // ���Կ� ������ ���(�������̹���, �����ؽ�Ʈ)
    public void SetItem(CountableItem item)
    {
        if (item == null)
        {
            return;
        }

        slotItem = item;

        ResourceManager.Instance.LoadIcon(item.Data.ItemIcon, sprite =>
        {
            if (sprite != null)
            {
                icon.sprite = sprite;
                icon.color = new Color(1f, 1f, 1f, 1f);

                if(slotItem.Amount > 1)
                {
                    ShowAmount();
                }
                else
                {
                    HideAmount();
                }
                
                amount.text = item.Amount.ToString();
            }
            else
            {
                Debug.Log($"Failed to load icon for item : {item.Data.ItemIcon}");
            }
        });
    }

    // ������ ������ ����
    public void RemoveItem()
    {
        icon.sprite = null;
        icon.color = new Color(1f, 1f, 1f, 0f);
        slotItem = null;
        HideAmount();
    }

    // �ش� �������� ��ϵǾ��ִ��� ����
    public bool HasItem(CountableItem ci)
    {
        return slotItem == ci;
    }

    // ������ ������ ���
    public void UseItem(Inventory iv)
    {
        iv.Use(slotItem);
    }
}
