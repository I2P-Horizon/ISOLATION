using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] private ItemTooltipUI itemTooltipUI;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject targetUI;

    // 장비 타입별 idx(0: Weapon, 1: Shoes, 2: Gloves, 3: Top)
    private enum Type { Weapon, Shoes, Gloves, Top}

    #region ** Fields **
    [Tooltip("캐릭터 장비 슬롯")]
    public List<EquipmentSlotUI> slotUIList = new List<EquipmentSlotUI>();
    public Item[] items;

    private GraphicRaycaster gr;
    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private int leftClick = 0;                              // 좌클릭 = 0
    private int rightClick = 1;                             // 우클릭 = 0;
    private int slotCounts = 4;                             // 장비슬롯 수

    private EquipmentSlotUI pointerOverSlot;                // 현재 마우스 포인터가 위치한 곳의 슬롯
    private EquipmentSlotUI beginDragSlot;                  // 마우스 드래그를 시작한 슬롯
    private Transform beginDragIconTransform;               // 마우스 드래그를 시작한 슬롯의 위치

    private Vector3 beginDragIconPoint;                     // 마우스 드래그를 시작한 아이콘 위치
    private Vector3 beginDragCursorPoint;                   // 마우스 드래그를 시작한 커서 위치
    private int beginDragSlotSiblingIndex;                  // 마우스 드래그를 시작한 슬롯의 SiblingIdx
    #endregion  

    #region ** 유니티 이벤트 함수 **
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        HideUI();
    }
    private void Update()
    {
        ped.position = Input.mousePosition;

        OnPointerEnterAndExit();
        ShowOrHideTooltipUI();
        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
        EndDrag();
    }
    #endregion

    #region ** Private Methods **
    // 초기화
    private void Init()
    {
        items = new Item[slotCounts];

        TryGetComponent(out gr);
        if (gr == null)
            gr = gameObject.AddComponent<GraphicRaycaster>();

        ped = new PointerEventData(EventSystem.current);
        rrList = new List<RaycastResult>(10);
    }

    private void HideUI() => targetUI.SetActive(false);

    // 툴팁 UI 갱신
    private void UpdateTooltipUI(EquipmentSlotUI slot)
    {
        if (!slot.HasItem)
            return;

        itemTooltipUI.SetItemInfo(items[slot.index].Data);
        itemTooltipUI.SetUIPosition(slot.SlotRect);
    }
    #endregion

    #region ** 마우스 이벤트 함수들 **
    // 마우스 커서가 UI 위에 있는지 여부
    private bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    // 레이캐스팅한 첫 UI요소의 컴포넌트를 가져오기
    private T RaycastAndgetFirstComponent<T>() where T : Component
    {
        // RaycastResult 초기화
        rrList.Clear();

        // 현재 마우스 위치에서 감지된 UI요소 저장
        gr.Raycast(ped, rrList);

        // 없으면
        if (rrList.Count == 0)
            return null;

        // 첫번째 UI의 컴포넌트 반환
        return rrList[0].gameObject.GetComponent<T>();
    }

    // 마우스 올라갈때 나갈때 처리
    private void OnPointerEnterAndExit()
    {
        // 이전 프레임 슬롯
        var prevSlot = pointerOverSlot;

        // 현재 프레임 슬롯
        var curSlot = pointerOverSlot = RaycastAndgetFirstComponent<EquipmentSlotUI>();

        // 마우스 올라갈 때
        if(prevSlot == null)
        {
            if(curSlot != null)
            {
                OnCurrentEnter();
            }
        }
        // 마우스 나갈 때
        else
        {
            if (curSlot == null)
            {
                OnPrevExit();
            }
            // 다른 슬롯으로 커서 옮길때
            else if (prevSlot != curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        void OnCurrentEnter()
        {
            curSlot.Highlight(true);
        }

        void OnPrevExit()
        {
            prevSlot.Highlight(false);
        }
    }

    // 마우스 눌렀을 때 처리
    private void OnPointerDown()
    {
        // 마우스 좌클릭(Holding)
        if (Input.GetMouseButtonDown(leftClick))
        {
            // 시작 슬롯
            beginDragSlot = RaycastAndgetFirstComponent<EquipmentSlotUI>();

            // 슬롯에 아이템이 있을 때
            if(beginDragSlot != null && beginDragSlot.HasItem)
            {
                // 드래그 위치, 참조
                beginDragIconTransform = beginDragSlot.IconRect.transform;
                beginDragIconPoint = beginDragIconTransform.position;
                beginDragCursorPoint = Input.mousePosition;

                beginDragSlotSiblingIndex = beginDragSlot.transform.GetSiblingIndex();
                beginDragSlot.transform.SetAsLastSibling();     // 가장 위에 표시

                beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                beginDragSlot = null;
            }
        }
        // 마우스 우클릭
        else if (Input.GetMouseButtonDown(rightClick))
        {
            // 우클릭 위치의 슬롯
            EquipmentSlotUI slotUI = RaycastAndgetFirstComponent<EquipmentSlotUI>();
            
            // 장비 장착 해제
            if(slotUI != null && slotUI.HasItem)
            {
                EquipmentItem item = (EquipmentItem)items[slotUI.index];
                inventory.AddItem(item.Data);
                item.Unequip();
                slotUIList[slotUI.index].RemoveItemIcon();
            }
        }
    }

    // 마우스 드래그중일 때 처리
    private void OnPointerDrag()
    {
        // 드래그중이 아닐때
        if (beginDragSlot == null) return;

        if(Input.GetMouseButton(leftClick))
        {
            // 슬롯 아이콘 위치 업데이트
            beginDragIconTransform.position = beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
        }
    }

    // 마우스 뗐을 때 처리
    private void OnPointerUp()
    {
        if(Input.GetMouseButtonUp(leftClick))
        {
            // 복원
            if(beginDragSlot != null)
            {
                // 위치 복원
                beginDragIconTransform.position = beginDragIconPoint;

                // UI 순서 복원
                beginDragSlot.transform.SetSiblingIndex(beginDragSlotSiblingIndex);

                // 드래그 완료
                EndDrag();

                // 하이라이트 이미지를 아이콘보다 앞에
                beginDragSlot.SetHighlightOnTop(true);

                // 참조제거
                beginDragSlot = null;
                beginDragIconTransform = null;
            }
        }
    }

    // 마우스 드래그 종료 처리
    private void EndDrag()
    {
        
    }

    // 아이템 툴팁 UI 활성/비활성화
    private void ShowOrHideTooltipUI()
    {
        // 마우스가 아이템 아이콘 위에 올라가있을 때 툴팁표시
        bool isValid = pointerOverSlot != null && pointerOverSlot.HasItem && (pointerOverSlot != beginDragSlot);

        if (isValid)
        {
            UpdateTooltipUI(pointerOverSlot);
            itemTooltipUI.ShowTooltipUI();
        }
        else
            itemTooltipUI.HideTooltipUI();
    }
    #endregion

    // 아이템 아이콘
    public void SetItemIcon(Item item, string type, string icon)
    {
        // 아이템 타입에 따른 index
        if (Enum.TryParse(type, out Type result))
        {
            int index = (int)result;

            // 슬롯 아이템 저장
            items[index] = item;

            // 아이콘 등록
            slotUIList[index].SetItemIcon(icon);
        }
        else
        {
            Debug.LogError($"'{type}'은(는) 유효한 타입이 아닙니다.");
        }
    }


}
