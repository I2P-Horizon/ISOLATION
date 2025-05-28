using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
                        ItemSlotUI

            - �κ��丮 �� ���� ������ ����
            - ������ �����ܰ� ������ ���� �ؽ�Ʈ ��� �� ����
            - ���� ����ȿ��
*/
public class ItemSlotUI : MonoBehaviour
{
    #region ** Serialized Fields **
    [SerializeField] private Image iconImage;           // ������ ������ �̹���
    [SerializeField] private Text amountText;           // ������ ����
    [SerializeField] private Image highlightImage;      // ���̶���Ʈ �̹���
    #endregion

    #region ** Fields **
    private Image slotImage;
    private InventoryUI inventoryUI;

    private RectTransform slotRect;                     // ������ RT
    private RectTransform iconRect;                     // ������ ������ ������ RT
    private RectTransform highlightRect;                // ������ ���̶���Ʈ RT

    private GameObject iconGo;                          
    private GameObject textGo;
    private GameObject highlightGo;


    private float padding = 4f;                         // ���� �� �����ܰ� ���� ���� ����
    private float maxHighlightAlpha = 0.5f;             // ���̶���Ʈ �̹��� �ִ� ���İ�
    private float currentHighlightAlpha = 0f;           // ���� ���̶���Ʈ �̹��� ���İ�
    private float highlightFadeDuration = 0.2f;         // ���̶���Ʈ �ҿ� �ð�
    private string iconName = "";                       // ���� ������ ������ �̸�

    private bool isAccessibleSlot = true;               // ���� ���ٰ��� ����
    private bool isAccessibleItem = true;               // ������ ���ٰ��� ����

    // ��Ȱ��ȭ�� ���� ����
    private Color InAccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    // ��Ȱ��ȭ�� ������ ����
    private Color InAccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    #endregion

    #region ** Properties **
    public int Index { get; private set; }              // ���� �ε���
    public bool HasItem => iconImage.sprite != null;    // ���Կ� �������� �ִ��� ����(sprite ���η� Ȯ��)

    public bool IsAccessible => isAccessibleItem && isAccessibleSlot;

    public RectTransform SlotRect => slotRect;

    public RectTransform IconRect => iconRect;
    #endregion

    #region ** Unity Events **
    private void Awake()
    {
        InitComponents();
        InitValues();
    }
    #endregion

    #region ** Private Methods **
    // �ʱ�ȭ
    private void InitComponents()
    {
        inventoryUI = GetComponentInParent<InventoryUI>();

        slotRect = GetComponent<RectTransform>();
        iconRect = iconImage.rectTransform;
        highlightRect = highlightImage.rectTransform;

        iconGo = iconRect.gameObject;
        textGo = amountText.gameObject;
        highlightGo = highlightImage.gameObject;

        slotImage = GetComponent<Image>();
    }

    // �ʱ�ȭ(������ ��ġ, ���̶���Ʈ �̹��� ��ġ)
    private void InitValues()
    {
        // ������ RT ����(Pivot : �߾�, Anchor : Top Left)
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;

        // ������ �е� ����
        iconRect.offsetMin = Vector2.one * (padding);
        iconRect.offsetMax = Vector2.one * (-padding);

        // �����ܰ� ���̶���Ʈ RT�� �����ϰ� ����
        highlightRect.pivot = iconRect.pivot;
        highlightRect.anchorMin = iconRect.anchorMin;
        highlightRect.anchorMax = iconRect.anchorMax;
        highlightRect.offsetMin = iconRect.offsetMin;
        highlightRect.offsetMax = iconRect.offsetMax;

        // ������ �� ���̶���Ʈ �̹����� Ŭ��X
        iconImage.raycastTarget = false;
        highlightImage.raycastTarget = false;

        HideIcon();
        // ���̶���Ʈ ȿ�� ������
        highlightGo.SetActive(false);
    }

    // ������ ������ Ȱ��ȭ
    private void ShowIcon() => iconGo.SetActive(true);
    
    // ������ ������ ��Ȱ��ȭ
    private void HideIcon() => iconGo.SetActive(false);
   
    // ���� �ؽ�Ʈ Ȱ��ȭ
    private void ShowText() => textGo.SetActive(true);
    
    // ���� �ؽ�Ʈ ��Ȱ��ȭ
    private void HideText() => textGo.SetActive(false);

    #endregion

    #region ** Public Methods **
    // ���� �ε��� ����
    public void SetSlotIndex(int index) => Index = index;
    
    // ���� Ȱ��ȭ/��Ȱ��ȭ ���� ����
    public void SetSlotAccessibleState(bool value)
    {
        // ���� ���Ի��°� �����ϰ����ϴ� value�� ������ ����
        if (isAccessibleSlot == value) return;

        // Ȱ��ȭ�� ����
        if(value)
        {
            slotImage.color = Color.black;
        }
        // ��Ȱ��ȭ�� ����
        else
        {
            slotImage.color = InAccessibleSlotColor;
            HideIcon();
            HideText();
        }

        isAccessibleSlot = value;
    }

    // ������ Ȱ��ȭ/��Ȱ��ȭ ���� ����
    public void SetItemAccessibleState(bool value)
    {
        // ���� �����ۻ��°� �����ϰ����ϴ� value�� ������ ����
        if (isAccessibleItem == value) return;

        // Ȱ��ȭ�� ������ ����
        if(value)
        {
            iconImage.color = Color.white;
            amountText.color = Color.white;
        }
        // ��Ȱ��ȭ�� ������ ����
        else
        {
            iconImage.color = InAccessibleIconColor;
            amountText.color = InAccessibleIconColor;
        }

        isAccessibleItem = value;
    }

    // ������ ������ ���
    public void SetItemIcon(string itemSprite)
    {
        if (itemSprite != null)
        {
            // ������ ������ ��������
            ResourceManager.Instance.LoadIcon(itemSprite, sprite =>
            {
                // ����
                if (sprite != null)
                {
                    // ������ �̸� ����
                    iconName = itemSprite;
                    // ������ ����
                    iconImage.sprite = sprite;
                    ShowIcon();
                }
                else
                {
                    Debug.Log($"Failed to load icon for item : {itemSprite}");
                }
            });
        }
        else
        {
            RemoveItemIcon();
        }
    }

    // ������ ������ ��� �� ���� ǥ��
    public void SetItemIconAndAmount(string itemSprite, int amount)
    {
        if (itemSprite != null)
        {
            // ������ ������ ��������
            ResourceManager.Instance.LoadIcon(itemSprite, sprite =>
            {
                // ����
                if (sprite != null)
                {
                    // ������ �̸� ����
                    iconName = itemSprite;
                    // ������ ����
                    iconImage.sprite = sprite;

                    if(amount > 1)
                    {
                        ShowText();
                    }
                    else
                    {
                        HideText();
                    }

                    ShowIcon();
                    amountText.text = amount.ToString();
                }
                else
                {
                    Debug.Log($"Failed to load icon for item : {itemSprite}");
                }
            });
        }
        else
        {
            RemoveItemIcon();
        }
    }

    // ���̶���Ʈ �̹����� ��/�ϴ����� ǥ��
    public void SetHighlightOnTop(bool value)
    {
        if (value)
            highlightRect.SetAsLastSibling();
        else
            highlightRect.SetAsFirstSibling();
    }

    // ���Կ��� ������ ����(������, Text)
    public void RemoveItemIcon()
    {
        iconImage.sprite = null;
        HideIcon();
        HideText();
    }

    // ���Կ� ������ ���� �ؽ�Ʈ ����
    public void SetItemAmount(int amount)
    {
        // ������ 2���̻��϶��� ǥ��
        if (HasItem && amount > 1)
        {
            ShowText();
        }
        else
        {
            HideText();
        }
        amountText.text = amount.ToString();
    }

    // �ٸ� �������� ������ �̵�
    public void SwapOrMoveIcon(ItemSlotUI otherSlot)
    {
        // 1. �ٸ� ������ �������� �ʾ��� ��
        if (otherSlot == null) return;
        // 2. �ڱ� �ڽ��� ��
        if (otherSlot == this) return;
        // 3. ��Ȱ��ȭ ������ ��
        if (!this.IsAccessible || !otherSlot.IsAccessible) return;

        // 1. �ٸ� ���Կ� �������� ���� �� -> �� �������� �ٸ� ������ ���������� ����
        if (otherSlot.HasItem) SetItemIcon(otherSlot.iconName);
        // 2. �ٸ� ���Կ� �������� ���� �� -> �� �����ܸ� �����
        else RemoveItemIcon();

        otherSlot.SetItemIcon(iconName);
    }

    // ���� ���̶���Ʈ ǥ�� �� ����
    public void Highlight(bool show)
    {
        // ��Ȱ��ȭ ������ ����
        if (!this.IsAccessible) return;

        if (show)
            StartCoroutine(nameof(HighlightFadeIn));
        else
            StartCoroutine(nameof(HighlightFadeOut));
    }

    #endregion

    #region ** Coroutines **
    // ���̶���Ʈ Fade-in
    private IEnumerator HighlightFadeIn()
    {
        // �������� fade-out ���߱�
        StopCoroutine(nameof(HighlightFadeOut));

        // ���̶���Ʈ �̹��� Ȱ��ȭ
        highlightGo.SetActive(true);

        float timer = maxHighlightAlpha / highlightFadeDuration;

        // ���̶���Ʈ �̹��� ���İ��� ������ ������Ű��
        for(; currentHighlightAlpha <= maxHighlightAlpha; currentHighlightAlpha += timer * Time.deltaTime)
        {
            highlightImage.color = new Color(
                     highlightImage.color.r,
                     highlightImage.color.g,
                     highlightImage.color.b,
                     currentHighlightAlpha
                );

            yield return null;
        }
    }

    // ���̶���Ʈ Fade-out
    private IEnumerator HighlightFadeOut()
    {
        StopCoroutine(nameof(HighlightFadeIn));

        float timer = maxHighlightAlpha / highlightFadeDuration;

        // ���̶���Ʈ �̹��� ���İ��� ������ ���ҽ�Ű��
        for(; currentHighlightAlpha >= 0f; currentHighlightAlpha -= timer * Time.deltaTime)
        {
            highlightImage.color = new Color(
                     highlightImage.color.r,
                     highlightImage.color.g,
                     highlightImage.color.b,
                     currentHighlightAlpha
                );

            yield return null;
        }

        highlightGo.SetActive(false);
    }
    #endregion

}
