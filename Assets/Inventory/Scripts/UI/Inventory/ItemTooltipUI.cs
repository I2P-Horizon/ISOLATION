using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  
                        ItemTooltipUI 

            - SetUIPosition: ������ ���� UI ��ġ ����
            - SetItemInfo : ItemData�� �޾ƿ� Text ����
 */
public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField] private Text itemNameText;         // ������ �̸� �ؽ�Ʈ
    [SerializeField] private Text itemTooltipText;      // ������ ���� �ؽ�Ʈ

    private RectTransform myRect;

    private void Awake()
    {
        HideTooltipUI();
        Init();
    }

    private void Init()
    {
        TryGetComponent(out myRect);
        // �ǹ� : LeftTop
        myRect.pivot = new Vector2(0f, 1f); ;   
    }

    public void ShowTooltipUI()
    {
        myRect.SetAsLastSibling();
        gameObject.SetActive(true);
    }
    public void HideTooltipUI() => gameObject.SetActive(false);

    // �ش� ���� �������� ������ Text ����
    public void SetItemInfo(ItemData data)
    {
        itemNameText.text = data.ItemName;
        itemTooltipText.text = data.ItemToolTip;
    }

    // ���� UI ��ġ ����(���� �����ϴ�)
    public void SetUIPosition(RectTransform slotRect)
    {
        float slotWidth = slotRect.rect.width;
        float slotHeight = slotRect.rect.height;

        // ���� ���� ��ġ ����(���� ���� �ϴ�)
        myRect.position = slotRect.position + new Vector3(slotWidth, -slotHeight);
        Vector2 pos = myRect.position;

        // ���� ũ��
        float width = myRect.rect.width;
        float height = myRect.rect.height;

        // ���� �� �ϴ��� �߷ȴ� �� ����
        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        // �����ʸ� �߸�
        if(rightTruncated && !bottomTruncated)
        {
            myRect.position = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        // �Ʒ��ʸ� �߸�
        else if(!rightTruncated && bottomTruncated)
        {
            myRect.position = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        // �� �� �߸�
        else if(rightTruncated && bottomTruncated)
        {
            myRect.position = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }
    }

}
