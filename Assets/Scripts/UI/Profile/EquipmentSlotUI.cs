using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*                  
                EquipmentSlotUI : 캐릭터 정보창의 장비 슬롯
            
                - Highlight 효과
 */
public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;           // 아이템 아이콘 이미지
    [SerializeField] private Image highlightImage;      // 하이라이트 이미지
    public int index;

    #region ** Fields **
    private EquipmentUI equipmentUI;
    private Image slotImage;

    private RectTransform slotRect;                     // 슬롯 RT
    private RectTransform iconRect;                     // 아이콘 RT

    private GameObject highlightGo;
    private GameObject iconGo;

    private RectTransform highlightRect;

    private float maxHighlightAlpha = 0.5f;             // 하이라이트 이미지 최대 알파값
    private float currentHighlightAlpha = 0f;           // 현재 하이라이트 이미지 알파값
    private float highlightFadeDuration = 0.2f;         // 하이라이트 소요시간
    #endregion

    #region ** Properties **
    public bool HasItem => iconImage.sprite != null;    // 슬롯에 아이템이 있는지 여부
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
    // 하이라이트 이미지를 상/하단으로 표시
    public void SetHighlightOnTop(bool value)
    {
        if (value)
            highlightRect.SetAsLastSibling();
        else
            highlightRect.SetAsFirstSibling();
    }

    // 슬롯 하이라이트 표시 및 해제
    public void Highlight(bool show)
    {
        if (show)
            StartCoroutine(nameof(HighlightFadeIn));
        else
            StartCoroutine(nameof(HighlightFadeOut));
    }

    // 아이템 아이콘 등록
    public void SetItemIcon(string itemSprite)
    {
        if(itemSprite != null)
        {
            // 아이콘 데이터 가져오기
            ResourceManager.Instance.LoadIcon(itemSprite, sprite =>
            {
                // 성공
                if (sprite != null)
                {
                    // 아이콘 설정
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

    // 슬롯 아이템 제거
    public void RemoveItemIcon()
    {
        iconImage.sprite = null;
        HideIcon();
    }
    #endregion

    #region ** Coroutines **
    // 하이라이트 Fade-in
    private IEnumerator HighlightFadeIn()
    {
        // 실행중인 fade-out 멈추기
        StopCoroutine(nameof(HighlightFadeOut));

        // 하이라이트 이미지 활성화
        highlightGo.SetActive(true);

        float timer = maxHighlightAlpha / highlightFadeDuration;

        // 하이라이트 이미지 알파값을 서서히 증가시키기
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

    // 하이라이트 Fade-out
    private IEnumerator HighlightFadeOut()
    {
        StopCoroutine(nameof(HighlightFadeIn));

        float timer = maxHighlightAlpha / highlightFadeDuration;

        // 하이라이트 이미지 알파값을 서서히 감소시키기
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
