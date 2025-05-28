using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
                        InventoryUI

            - �κ��丮 �� ���Ե��� ��� ����
            - �κ��丮 ������ ���� ����
            - ���콺 �̺�Ʈ ó��
            - Inventory <-> ItemSlotUI �߰��ٸ�����
*/
public class InventoryUI : MonoBehaviour
{
    #region ** Serialized Fields **
    [SerializeField] private RectTransform contentAreaRT;   // ������ ����
    [SerializeField] private GameObject itemSlotPrefab;     // ������ ���� ���� ������
    [SerializeField] private InventoryPopupUI popup;        // �˾� UI
    [SerializeField] private ItemTooltipUI itemTooltipUI;
    [SerializeField] private Text goldText;                 // ���� ��� �ؽ�Ʈ
    [SerializeField] private UIRaycaster rc;                // ����ĳ����
    #endregion

    #region ** �κ��丮 �ɼ� **
    private int horizontalSlotCount = 6;                    // ���� ���ΰ���
    private int verticalSlotCount = 6;                      // ���� ���ΰ���
    private float slotMargin = 9f;                          // ���� ����
    private float contentAreaPadding = 8f;                  // �κ��丮 ���� ���� ����
    private float slotSize = 81f;                           // ���� ������
    private bool showHighlight = true;                      // ���̶���Ʈ ���̱�
    #endregion

    #region ** Fields **
    private Inventory inventory;                            // ����� �κ��丮
    private List<ItemSlotUI> slotUIList = new List<ItemSlotUI>();

    private ItemSlotUI pointerOverSlot;                     // ���� ���콺 �����Ͱ� ��ġ�� ���� ����
    private ItemSlotUI beginDragSlot;                       // ���콺 �巡�׸� ������ ����
    private Transform beginDragIconTransform;               // ���콺 �巡�׸� ������ ������ ��ġ

    private int leftClick = 0;                              // ��Ŭ�� = 0
    private int rightClick = 1;                             // ��Ŭ�� = 0;

    private Vector3 beginDragIconPoint;                     // ���콺 �巡�׸� ������ ������ ��ġ
    private Vector3 beginDragCursorPoint;                   // ���콺 �巡�׸� ������ Ŀ�� ��ġ
    private int beginDragSlotSiblingIndex;                  // ���콺 �巡�׸� ������ ������ SiblingIdx
    #endregion

    #region ** ����Ƽ �̺�Ʈ �Լ� **
    private void Awake()
    {
        Init();
        InitSlots();
    }

    private void Start()
    {
        HideUI();
    }

    private void Update()
    {
        OnPointerEnterAndExit();
        ShowOrHideTooltipUI();
        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
        ShowPlayerGold();
    }

    #endregion

    #region ** Private Methods **
    // �ʱ�ȭ
    private void Init()
    {
        if(itemTooltipUI == null)
        {
            itemTooltipUI = GetComponentInChildren<ItemTooltipUI>();
        }
    }

    // ���� ���� ���� ���� ����
    private void InitSlots()
    {
        // ���� ������ ����
        itemSlotPrefab.TryGetComponent(out RectTransform slotRect);

        // ���� ������ ����
        slotRect.sizeDelta = new Vector2(slotSize, slotSize);

        // ���Կ� ItemSlotUI ��ũ��Ʈ ���̱�
        itemSlotPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
            itemSlotPrefab.AddComponent<ItemSlotUI>();

        // ���� ���� ��Ȱ��ȭ
        itemSlotPrefab.SetActive(false);

        // ������ ä�� ������ġ, ������ġ
        Vector2 beginPos = new Vector2(contentAreaPadding, -contentAreaPadding);
        Vector2 curPos = beginPos;

        // ������ ���Ե��� ���� ����Ʈ
        slotUIList = new List<ItemSlotUI>(verticalSlotCount * horizontalSlotCount);

        // ���� �����ϱ�
        for (int j = 0; j < verticalSlotCount; j++)
        {
            for (int i = 0; i < horizontalSlotCount; i++)
            {
                int slotIndex = (horizontalSlotCount * j) + i;       // ���� �ε���

                // �����ϳ� ����
                var slotRT = CloneSlot();
                // ������ �������� ����
                slotRT.pivot = new Vector2(0f, 1f);                  // Left Top ����
                slotRT.anchoredPosition = curPos;
                slotRT.gameObject.SetActive(true);
                slotRT.gameObject.name = $"Item Slot [{slotIndex}]"; // ���̾��Ű�� �����̸�("Item Slot 0~35")

                ItemSlotUI slotUI = slotRT.GetComponent<ItemSlotUI>();
                slotUI.SetSlotIndex(slotIndex);                      // ���Կ� �ε������̱�
                slotUIList.Add(slotUI);                              // ����Ʈ�� ������ �������� �߰�

                // ����ĭ(����)
                curPos.x += (slotMargin + slotSize);
            }

            // ������(����)
            curPos.x = beginPos.x;
            curPos.y -= (slotMargin + slotSize);
        }

        // ���� �����ı�
        if (itemSlotPrefab.scene.rootCount != 0)
            Destroy(itemSlotPrefab);

        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(itemSlotPrefab);
            RectTransform rt = slotGo.GetComponent<RectTransform>();
            rt.SetParent(contentAreaRT);

            return rt;
        }
    }

    // ���� ��� ǥ��
    private void ShowPlayerGold() => goldText.text = DataManager.Instance.GetPlayerData().Gold.ToString();
    // �κ��丮 UI ��Ȱ��ȭ
    private void HideUI()
    {
        this.gameObject.SetActive(false);
    }

    // �� ���� ������ ��ȯ
    private void TrySwapItems(ItemSlotUI begin, ItemSlotUI end)
    {
        // �ڱ��ڽ� ó��
        if (begin == end) return;

        begin.SwapOrMoveIcon(end);
        inventory.Swap(begin.Index, end.Index);
    }

    // ������ ������ ��û
    private void TryRemoveItem(int index)
    {
        inventory.Remove(index);
    }

    // ������ ��� �� ����
    private void TryUseItem(int index)
    {
        inventory.Use(index);
    }

    // �÷��̾� ������â�� ������ ���
    private void TryRegisterItem(int index, PlayerItemSlotUI end)
    {
        inventory.AddItemAtPlayerItemSlot(index, end);
    }

    // ���� UI ���� ������ ����
    private void UpdateTooltipUI(ItemSlotUI slot)
    {
        if (!slot.IsAccessible || !slot.HasItem)
            return;

        // ���� ���� ����
        itemTooltipUI.SetItemInfo(inventory.GetItemData(slot.Index));

        // ���� ��ġ ����
        itemTooltipUI.SetUIPosition(slot.SlotRect);
    }

    #endregion

    #region ** ���콺 �̺�Ʈ �Լ��� **

    // ���콺 Ŀ���� UI ���� �ִ��� ����
    private bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    // ������ ���� UI Ȱ��/��Ȱ��ȭ
    private void ShowOrHideTooltipUI()
    {
        // ���콺�� ������ ������ ���� �ö����� �� ����ǥ��
        bool isValid =
            pointerOverSlot != null && pointerOverSlot.HasItem && pointerOverSlot.IsAccessible
            && (pointerOverSlot != beginDragSlot);

        if (isValid)
        {
            UpdateTooltipUI(pointerOverSlot);
            itemTooltipUI.ShowTooltipUI();
        }
        else
            itemTooltipUI.HideTooltipUI();

    }

    
    // ���콺 �ö󰥶� ������ ó��
    private void OnPointerEnterAndExit()
    {
        // ���� ������ ����
        var prevSlot = pointerOverSlot;

        // ���� ������ ����
        var curSlot = pointerOverSlot = rc.RaycastAndgetFirstComponent<ItemSlotUI>();

        // ���콺 �ö� ��
        if (prevSlot == null)
        {
            if (curSlot != null)
            {
                OnCurrentEnter();
            }
        }
        // ���콺 ���� ��
        else
        {
            if (curSlot == null)
            {
                OnPrevExit();
            }
            // �ٸ� �������� Ŀ�� �ű涧
            else if (prevSlot != curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        void OnCurrentEnter()
        {
            if (showHighlight)
                curSlot.Highlight(true);
        }

        void OnPrevExit()
        {
            prevSlot.Highlight(false);
        }
    }
    
    // ���콺 ������ �� ó��
    private void OnPointerDown()
    {
        // ���콺 ��Ŭ��(Holding)
        if (Input.GetMouseButtonDown(leftClick))
        {
            // ���� ����
            beginDragSlot = rc.RaycastAndgetFirstComponent<ItemSlotUI>();
            // ���Կ� �������� ���� ��
            if (beginDragSlot != null && beginDragSlot.HasItem && beginDragSlot.IsAccessible)
            {
                // �巡�� ��ġ,���� 
                beginDragIconTransform = beginDragSlot.IconRect.transform;
                beginDragIconPoint = beginDragIconTransform.position;
                beginDragCursorPoint = Input.mousePosition;

                beginDragSlotSiblingIndex = beginDragSlot.transform.GetSiblingIndex();
                beginDragSlot.transform.SetAsLastSibling();     // ���� ���� ǥ��

                beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                beginDragSlot = null;
            }
        }
        // ���콺 ��Ŭ��(������ ��� �� ����)
        else if (Input.GetMouseButtonDown(rightClick))
        {
            ItemSlotUI slotUI = rc.RaycastAndgetFirstComponent<ItemSlotUI>();
            
            if(slotUI != null && slotUI.HasItem && slotUI.IsAccessible)
            {
                TryUseItem(slotUI.Index);
            }
        }

    }

    // ���콺 �巡������ �� ó��
    private void OnPointerDrag()
    {
        // �巡������ �ƴҶ�
        if (beginDragSlot == null) return;

        if (Input.GetMouseButton(leftClick))
        {
            // ���� ������ ��ġ ������Ʈ
            beginDragIconTransform.position = beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
        }
    }

    // ���콺 ���� �� ó��
    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(leftClick))
        {
            // �������� ����
            if (beginDragSlot != null)
            {
                // ��ġ ����
                beginDragIconTransform.position = beginDragIconPoint;

                // UI ���� ����
                beginDragSlot.transform.SetSiblingIndex(beginDragSlotSiblingIndex);

                // �巡�� �Ϸ�
                EndDrag();

                // ���̶���Ʈ �̹����� �����ܺ��� �տ�
                beginDragSlot.SetHighlightOnTop(true);

                // ���� ����
                beginDragSlot = null;
                beginDragIconTransform = null;
            }
        }
    }

    // ���콺 �巡�� ���� ó��(������ ��ȯ, �̵�, ������ ��)
    private void EndDrag()
    {
        ItemSlotUI endDragSlot = rc.RaycastAndgetFirstComponent<ItemSlotUI>();
        PlayerItemSlotUI playerSlot = rc.RaycastAndgetFirstComponent<PlayerItemSlotUI>();

        // �÷��̾� ������â�� ������ ���
        if (playerSlot != null)
        {
            TryRegisterItem(beginDragSlot.Index, playerSlot);
        }

        // ������ �̵� �� ��ȯ
        if (endDragSlot != null && endDragSlot.IsAccessible)
        {
            TrySwapItems(beginDragSlot, endDragSlot);
        }
        
        // ������ ������
        if (!IsOverUI())
        {
            int index = beginDragSlot.Index;
            string itemName = inventory.GetItemName(index);

            popup.OpenConfirmationPopupUI(() => TryRemoveItem(index), itemName);
        }
    }
    #endregion

    #region ** Public Methods **
    // �κ��丮 �������
    public void SetInventoryRef(Inventory inv)
    {
        inventory = inv;
    }

    // ���� ������ ���� ���� ����(Ȱ��ȭ�� ����)
    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        // �� 36ĭ ��
        for (int i = 0; i < slotUIList.Count; i++)
        {
            // accessibleCount ���� ��ŭ�� ���� Ȱ��ȭ
            slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

    // �ش� �ε��� ������ ������ ������ ��� �� ���� ǥ��
    public void SetItemIconAndAmountText(int index, string icon, int amount = 1)
    {
        slotUIList[index].SetItemIconAndAmount(icon, amount);
    }

    // �ش� �ε��� ������ ������ ���� �ؽ�Ʈ ����
    public void HideItemAmountText(int index)
    {
        slotUIList[index].SetItemAmount(1);
    }

    // �ش� �ε��� ������ ������ ����(������ �� �ؽ�Ʈ ����)
    public void RemoveItem(int index)
    {
        slotUIList[index].RemoveItemIcon();
    }

    #endregion
}
