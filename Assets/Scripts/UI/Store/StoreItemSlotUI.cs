using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
                        StoreItemSlotUI

            - �������� �Ǹ��ϴ� �����۵��� ������ ����
                - ������ ID �� ItemData
            - ������ ID�� ���� ������������ �з��ϰ�, �Ǹž����۵��� ����
            - �κ��丮�� �����Ͽ� �������� �����ϸ� �κ��丮�� ������ ����

*/
public class StoreItemSlotUI : MonoBehaviour
{
    public int itemId;                                          // ������ ID
    [SerializeField] private Image  itemIcon;                   // ������ ������
    [SerializeField] private Text   itemNameText;               // ������ �̸�
    [SerializeField] private Text   itemExplanation;            // ������ ����
    [SerializeField] private Text   itemPrice;                  // ������ ����
    [SerializeField] private Button perchaseBtn;                // ���� ��ư
    [SerializeField] private Inventory inventory;               // ����� �κ��丮

    private ArmorItemData armorItemData;
    private WeaponItemData weaponItemData;
    private PortionItemData portionItemData;
    private ItemData curItemData;                               // ���� ������ ������ ������

    private int price;                                          // ������ ����

    private void Start()
    {
        GetItemData();

        perchaseBtn.onClick.AddListener(PerchaseItem);
    }

    // ���� ��ư �̺�Ʈ
    private void PerchaseItem()
    {
        if(DataManager.Instance.GetPlayerData().Gold >= price)
        {
            AudioManager.Instance.PlaySFX("Gold");
            inventory.AddItem(curItemData);
            DataManager.Instance.GetPlayerData().UseGold(price);
        }
        else
        {
            Debug.Log("��尡 �����մϴ�.");
        }
    }

    // ������ Ÿ�Ժ� ������ �ҷ�����
    private void GetItemData()
    {
        // 1. ���Ǿ�����
        if(itemId > 10000 && itemId < 20000)
        {
            portionItemData = DataManager.Instance.GetPortionDataById(itemId);
            curItemData = portionItemData;
            SetSlotData(portionItemData);
        }
        // 2. ��������
        else if(itemId > 20000 && itemId < 30000)
        {
            armorItemData = DataManager.Instance.GetArmorDataById(itemId);
            curItemData = armorItemData;
            SetSlotData(armorItemData);
        }
        // 3. ���������
        else if(itemId > 30000 && itemId < 40000)
        {
            weaponItemData = DataManager.Instance.GetWeaponDataById(itemId);
            curItemData = weaponItemData;
            SetSlotData(weaponItemData);
        }

    }

    // ������ ���� ���� ä���
    private void SetSlotData(ItemData data)
    {
        // 1. ������
        ResourceManager.Instance.LoadIcon(data.ItemIcon, sprite =>
        {
            // ����
            if (sprite != null)
            {
                itemIcon.sprite = sprite;
            }
            else
            {
                Debug.Log($"Failed to load icon for item : {data.ItemIcon}");
            }
        });

        // 2. ������ �̸�
        itemNameText.text = data.ItemName;

        // 3. ������ ����
        itemExplanation.text = data.ItemExplanation;

        // 4. ������ ����
        itemPrice.text = data.ItemPrice.ToString() + "G";
        price = data.ItemPrice;
    }
}
