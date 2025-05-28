using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
                        StoreItemSlotUI

            - 상점에서 판매하는 아이템들의 정보를 가짐
                - 아이템 ID 및 ItemData
            - 아이템 ID에 따라 아이템종류를 분류하고, 판매아이템들을 세팅
            - 인벤토리와 연결하여 아이템을 구매하면 인벤토리에 아이템 생성

*/
public class StoreItemSlotUI : MonoBehaviour
{
    public int itemId;                                          // 아이템 ID
    [SerializeField] private Image  itemIcon;                   // 아이템 아이콘
    [SerializeField] private Text   itemNameText;               // 아이템 이름
    [SerializeField] private Text   itemExplanation;            // 아이템 설명
    [SerializeField] private Text   itemPrice;                  // 아이템 가격
    [SerializeField] private Button perchaseBtn;                // 구매 버튼
    [SerializeField] private Inventory inventory;               // 연결된 인벤토리

    private ArmorItemData armorItemData;
    private WeaponItemData weaponItemData;
    private PortionItemData portionItemData;
    private ItemData curItemData;                               // 현재 슬롯의 아이템 데이터

    private int price;                                          // 아이템 가격

    private void Start()
    {
        GetItemData();

        perchaseBtn.onClick.AddListener(PerchaseItem);
    }

    // 구매 버튼 이벤트
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
            Debug.Log("골드가 부족합니다.");
        }
    }

    // 아이템 타입별 데이터 불러오기
    private void GetItemData()
    {
        // 1. 포션아이템
        if(itemId > 10000 && itemId < 20000)
        {
            portionItemData = DataManager.Instance.GetPortionDataById(itemId);
            curItemData = portionItemData;
            SetSlotData(portionItemData);
        }
        // 2. 방어구아이템
        else if(itemId > 20000 && itemId < 30000)
        {
            armorItemData = DataManager.Instance.GetArmorDataById(itemId);
            curItemData = armorItemData;
            SetSlotData(armorItemData);
        }
        // 3. 무기아이템
        else if(itemId > 30000 && itemId < 40000)
        {
            weaponItemData = DataManager.Instance.GetWeaponDataById(itemId);
            curItemData = weaponItemData;
            SetSlotData(weaponItemData);
        }

    }

    // 아이템 슬롯 정보 채우기
    private void SetSlotData(ItemData data)
    {
        // 1. 아이콘
        ResourceManager.Instance.LoadIcon(data.ItemIcon, sprite =>
        {
            // 성공
            if (sprite != null)
            {
                itemIcon.sprite = sprite;
            }
            else
            {
                Debug.Log($"Failed to load icon for item : {data.ItemIcon}");
            }
        });

        // 2. 아이템 이름
        itemNameText.text = data.ItemName;

        // 3. 아이템 설명
        itemExplanation.text = data.ItemExplanation;

        // 4. 아이템 가격
        itemPrice.text = data.ItemPrice.ToString() + "G";
        price = data.ItemPrice;
    }
}
