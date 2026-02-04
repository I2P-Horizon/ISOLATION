using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
                     PlayerItemSlotUI

                - 플레이어 아이템 슬롯 관리      
                - 슬롯에 아이템을 가져다놓으면 아이템을 등록
                    - 인벤토리에서 아이템을 사용하거나 제거하면 반영
 */

public class PlayerItemSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Text amount;

    [SerializeField] private Slider durabilitySlider;
    [SerializeField] private Image durabilityFillImage;
    [SerializeField] private Gradient durabilityGradient;

    [SerializeField] private Inventory inventory;

    [SerializeField] private UIRaycaster rc;
    [SerializeField] private InventoryPopupUI popup;
    [SerializeField] private RectTransform itemContent;

    public int Index { get; set; }

    private Item slotItem;             // 이 슬롯의 아이템

    private Transform originalParent;
    private Vector3 originalPosition;
    private Canvas parentCanvas;

    private void Awake()
    {
        if (durabilitySlider != null)
        {
            durabilitySlider.gameObject.SetActive(false);
        }

        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void ShowAmount() => amount.gameObject.SetActive(true);
    private void HideAmount() => amount.gameObject.SetActive(false);

    // 슬롯 업데이트
    public void UpdateSlot()
    {
        SetItem(slotItem);
    }

    // 슬롯에 아이템 등록(아이콘이미지, 수량텍스트)
    public void SetItem(Item item)
    {
        slotItem = item;

        if (item == null)
        {
            RemoveItem();
            return;
        }

        Sprite sprite = Resources.Load<Sprite>($"Icon/{item.Data.ItemIcon}");
        if (sprite != null)
        {
            icon.sprite = sprite;
            icon.color = Color.white;
            icon.gameObject.SetActive(true);
        }

        if (item is CountableItem ci)
        {
            amount.text = ci.Amount.ToString();
            amount.gameObject.SetActive(true);
        }
        else
        {
            amount.gameObject.SetActive(false);
        }

        updateDurabilityUI(item);
    }

    // 슬롯의 아이템 제거
    public void RemoveItem()
    {
        icon.sprite = null;
        icon.color = new Color(1f, 1f, 1f, 0f);
        slotItem = null;
        HideAmount();
        if (durabilitySlider != null)
        {
            durabilitySlider.gameObject.SetActive(false);
        }
    }

    // 해당 아이템이 등록되어있는지 여부
    public bool HasItem(Item item)
    {
        return slotItem == item;
    }

    private void updateDurabilityUI(Item item)
    {
        if (durabilitySlider == null) return;

        if (item is EquipmentItem ei && ei.EquipmentData.MaxDurability > 0)
        {
            durabilitySlider.gameObject.SetActive(true);

            float percent = ei.CurrentDurability / ei.EquipmentData.MaxDurability;
            durabilitySlider.value = percent;

            if (durabilityFillImage != null)
            {
                durabilityFillImage.color = durabilityGradient.Evaluate(percent);
            }
        }
        else
        {
            durabilitySlider.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 우클릭 시 아이템 사용
            if (slotItem != null)
            {
                inventory.Use(Index);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotItem == null) return;

        InventoryUI.IsDraggingItem = true;

        originalParent = itemContent.parent;
        originalPosition = itemContent.position;

        if (parentCanvas != null)
        {
            itemContent.SetParent(parentCanvas.transform, true);
        }

        itemContent.SetAsLastSibling();

        icon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotItem == null) return;

        itemContent.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slotItem == null) return;
        
        itemContent.SetParent(originalParent, true);
        itemContent.position = originalPosition;
        icon.raycastTarget = true;

        processDrop();

        InventoryUI.IsDraggingItem = false;
    }

    private void processDrop()
    {
        EquipmentSlotUI equipSlot = rc.RaycastAndgetFirstComponent<EquipmentSlotUI>();
        ItemSlotUI itemSlot = rc.RaycastAndgetFirstComponent<ItemSlotUI>();
        PlayerItemSlotUI playerItemSlotUI = rc.RaycastAndgetFirstComponent<PlayerItemSlotUI>();

        if (equipSlot != null)
        {
            if (slotItem is EquipmentItem equipItem)
            {
                bool result = equipSlot.TryEquip(equipItem);
                Debug.Log($"Equip Result: {result}");
                if (result)
                {
                    inventory.Remove(Index);
                }
            }
            return;
        }

        if (itemSlot != null && itemSlot.IsAccessible)
        {
            inventory.Swap(this.Index, itemSlot.Index);
            return;
        }

        if (playerItemSlotUI != null)
        {
            inventory.Swap(this.Index, playerItemSlotUI.Index);
            return;
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (popup != null)
            {
                int targetIndex = this.Index;
                popup.OpenConfirmationPopupUI(() => inventory.Remove(targetIndex), slotItem.Data.ItemName);
            }
        }
    }
}
