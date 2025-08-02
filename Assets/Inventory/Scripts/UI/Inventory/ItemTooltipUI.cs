using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  
                        ItemTooltipUI 

            - SetUIPosition: 아이템 툴팁 UI 위치 선정
            - SetItemInfo : ItemData를 받아와 Text 설정
 */
public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField] private Text itemNameText;         // 아이템 이름 텍스트
    [SerializeField] private Text itemTooltipText;      // 아이템 툴팁 텍스트

    private RectTransform myRect;

    private void Awake()
    {
        HideTooltipUI();
        Init();
    }

    private void Init()
    {
        TryGetComponent(out myRect);
        // 피벗 : LeftTop
        myRect.pivot = new Vector2(0f, 1f); ;   
    }

    public void ShowTooltipUI()
    {
        myRect.SetAsLastSibling();
        gameObject.SetActive(true);
    }
    public void HideTooltipUI() => gameObject.SetActive(false);

    // 해당 슬롯 아이템의 정보로 Text 설정
    public void SetItemInfo(ItemData data)
    {
        itemNameText.text = data.ItemName;
        itemTooltipText.text = data.ItemToolTip;
    }

    // 툴팁 UI 위치 설정(슬롯 우측하단)
    public void SetUIPosition(RectTransform slotRect)
    {
        float slotWidth = slotRect.rect.width;
        float slotHeight = slotRect.rect.height;

        // 툴팁 최초 위치 설정(슬롯 우측 하단)
        myRect.position = slotRect.position + new Vector3(slotWidth, -slotHeight);
        Vector2 pos = myRect.position;

        // 툴팁 크기
        float width = myRect.rect.width;
        float height = myRect.rect.height;

        // 우측 및 하단이 잘렸는 지 여부
        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        // 오른쪽만 잘림
        if(rightTruncated && !bottomTruncated)
        {
            myRect.position = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        // 아래쪽만 잘림
        else if(!rightTruncated && bottomTruncated)
        {
            myRect.position = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        // 둘 다 잘림
        else if(rightTruncated && bottomTruncated)
        {
            myRect.position = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }
    }

}
