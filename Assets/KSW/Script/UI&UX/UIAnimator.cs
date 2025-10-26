using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    [Header("애니메이션 타입")]
    [SerializeField] private bool fade;

    [SerializeField] private bool up;
    [SerializeField] private bool down;
    [SerializeField] private bool left;
    [SerializeField] private bool right;

    [Header("값 설정")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float moveOffset = 200f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    public bool uiAnimation = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    /// <summary>
    /// UI 표시 + Animation 실행
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();

        canvasGroup.alpha = fade ? 0f : 1f;

        Vector2 startPos = originalPosition;
        if (up) startPos -= new Vector2(0, moveOffset);
        if (down) startPos += new Vector2(0, moveOffset);
        if (left) startPos += new Vector2(moveOffset, 0);
        if (right) startPos -= new Vector2(moveOffset, 0);
        rectTransform.anchoredPosition = startPos;

        StartCoroutine(In());
    }


    /// <summary>
    /// UI 닫기 + Animation 실행
    /// </summary>
    public void Close()
    {
        /* 실행 중인 코루틴 중지 후 비활성화 코루틴 실행 */
        StopAllCoroutines();
        StartCoroutine(Out());
    }

    /// <summary>
    /// UI 활성화 애니메이션 처리
    /// </summary>
    /// <returns></returns>
    private IEnumerator In()
    {
        /* 시작 위치 초기화 */
        Vector2 startPos = originalPosition;

        /* 각 옵션에 따라 시작 위치 변경 */
        if (up) startPos -= new Vector2(0, moveOffset);
        if (down) startPos += new Vector2(0, moveOffset);
        if (left) startPos += new Vector2(moveOffset, 0);
        if (right) startPos -= new Vector2(moveOffset, 0);

        /* UI의 RectTransform 위치를 시작 위치로 설정 */
        rectTransform.anchoredPosition = startPos;

        /* 애니메이션 진행 시간 초기화 */
        float time = 0f;

        /* while 루프를 통해 시간에 따라 점진적으로 이동/페이드 인 처리 */
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            if (fade) canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            yield return null;
        }

        /* 애니메이션 종료 후, 최종 위치 및 알파값 적용 */
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition;
    }

    /// <summary>
    /// UI 비활성화 애니메이션 처리
    /// </summary>
    /// <returns></returns>
    private IEnumerator Out()
    {
        Vector2 endPos = originalPosition;

        if (up) endPos -= new Vector2(0, moveOffset);
        if (down) endPos += new Vector2(0, moveOffset);
        if (left) endPos += new Vector2(moveOffset, 0);
        if (right) endPos -= new Vector2(moveOffset, 0);

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            if (fade) canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, endPos, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = originalPosition;

        gameObject.SetActive(false);
    }
}