using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*                  
                EquipmentSlotUI : ĳ���� ����â�� ��� ����
            
                - Highlight ȿ��
 */
public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;           // ������ ������ �̹���
    [SerializeField] private Image highlightImage;      // ���̶���Ʈ �̹���
    public int index;

    #region ** Fields **
    private EquipmentUI equipmentUI;
    private Image slotImage;

    private RectTransform slotRect;                     // ���� RT
    private RectTransform iconRect;                     // ������ RT

    private GameObject highlightGo;
    private GameObject iconGo;

    private RectTransform highlightRect;

    private float maxHighlightAlpha = 0.5f;             // ���̶���Ʈ �̹��� �ִ� ���İ�
    private float currentHighlightAlpha = 0f;           // ���� ���̶���Ʈ �̹��� ���İ�
    private float highlightFadeDuration = 0.2f;         // ���̶���Ʈ �ҿ�ð�
    #endregion

    #region ** Properties **
    public bool HasItem => iconImage.sprite != null;    // ���Կ� �������� �ִ��� ����
    public RectTransform SlotRect => slotRect;
    public RectTransform IconRect => iconRect;
    #endregion

    private void Awake()
    {
        Init();
    }

    #region ** Private Methods **
    private void Init()
    {
        equipmentUI = GetComponentInParent<EquipmentUI>();

        slotRect = (RectTransform)transform;
        iconRect = iconImage.rectTransform;

        iconGo = iconImage.gameObject;
        highlightGo = highlightImage.gameObject;

        highlightImage.raycastTarget = false;
        highlightRect = highlightImage.rectTransform;

        highlightGo.SetActive(false);
    }

    private void ShowIcon() => iconGo.SetActive(true);
    private void HideIcon() => iconGo.SetActive(false);

    #endregion

    #region ** Public Methods **
    // ���̶���Ʈ �̹����� ��/�ϴ����� ǥ��
    public void SetHighlightOnTop(bool value)
    {
        if (value)
            highlightRect.SetAsLastSibling();
        else
            highlightRect.SetAsFirstSibling();
    }

    // ���� ���̶���Ʈ ǥ�� �� ����
    public void Highlight(bool show)
    {
        if (show)
            StartCoroutine(nameof(HighlightFadeIn));
        else
            StartCoroutine(nameof(HighlightFadeOut));
    }

    // ������ ������ ���
    public void SetItemIcon(string itemSprite)
    {
        if(itemSprite != null)
        {
            // ������ ������ ��������
            ResourceManager.Instance.LoadIcon(itemSprite, sprite =>
            {
                // ����
                if (sprite != null)
                {
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

    // ���� ������ ����
    public void RemoveItemIcon()
    {
        iconImage.sprite = null;
        HideIcon();
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
        for (; currentHighlightAlpha <= maxHighlightAlpha; currentHighlightAlpha += timer * Time.deltaTime)
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
        for (; currentHighlightAlpha >= 0f; currentHighlightAlpha -= timer * Time.deltaTime)
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
