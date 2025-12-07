using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    [Header("애니메이션 타입")]
    [SerializeField] private bool _fade;
    [SerializeField] private bool _scale;
    [SerializeField] private bool _up;
    [SerializeField] private bool _down;
    [SerializeField] private bool _left;
    [SerializeField] private bool _right;

    [Header("값 설정")]
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _moveOffset = 200f;
    [SerializeField] private float _startScale = 0f;

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;

    private Vector2 _originalPosition;
    private Vector3 _originalScale;

    private bool _uiAnimation = false;

    /// <summary>
    /// UI 표시 + Animation 실행
    /// </summary>
    public void Show()
    {
        if (_uiAnimation) return;

        gameObject.SetActive(true);
        StopAllCoroutines();

        _canvasGroup.alpha = _fade ? 0f : 1f;

        Vector2 startPos = _originalPosition;
        if (_up) startPos -= new Vector2(0, _moveOffset);
        if (_down) startPos += new Vector2(0, _moveOffset);
        if (_left) startPos += new Vector2(_moveOffset, 0);
        if (_right) startPos -= new Vector2(_moveOffset, 0);
        if (_scale) transform.localScale = Vector3.one * _startScale;
        _rectTransform.anchoredPosition = startPos;

        StartCoroutine(openUI());
    }

    /// <summary>
    /// UI 닫기 + Animation 실행
    /// </summary>
    public void Close()
    {
        if (_uiAnimation) return;
        StartCoroutine(closeUI());
    }

    /// <summary>
    /// UI 활성화 애니메이션 처리
    /// </summary>
    /// <returns></returns>
    private IEnumerator openUI()
    {
        _uiAnimation = true;

        /* 시작 위치 초기화 */
        Vector2 startPos = _originalPosition;

        /* 각 옵션에 따라 시작 위치 변경 */
        if (_up) startPos -= new Vector2(0, _moveOffset);
        if (_down) startPos += new Vector2(0, _moveOffset);
        if (_left) startPos += new Vector2(_moveOffset, 0);
        if (_right) startPos -= new Vector2(_moveOffset, 0);

        /* UI의 RectTransform 위치를 시작 위치로 설정 */
        _rectTransform.anchoredPosition = startPos;

        /* 애니메이션 진행 시간 초기화 */
        float time = 0f;

        /* while 루프를 통해 시간에 따라 점진적으로 이동/페이드 인 처리 */
        while (time < _duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / _duration);

            if (_fade) _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            _rectTransform.anchoredPosition = Vector2.Lerp(startPos, _originalPosition, t);

            float scaleT = 1f - Mathf.Pow(1f - t, 3f);

            if (_scale) transform.localScale = Vector3.Lerp(Vector3.one * _startScale, _originalScale, scaleT);

            yield return null;
        }

        /* 애니메이션 종료 후, 최종 위치 및 알파값 적용 */
        _canvasGroup.alpha = 1f;
        _rectTransform.anchoredPosition = _originalPosition;
        if (_scale) transform.localScale = _originalScale;

        _uiAnimation = false;
    }

    /// <summary>
    /// UI 비활성화 애니메이션 처리
    /// </summary>
    /// <returns></returns>
    private IEnumerator closeUI()
    {
        _uiAnimation = true;

        Vector2 endPos = _originalPosition;

        if (_up) endPos -= new Vector2(0, _moveOffset);
        if (_down) endPos += new Vector2(0, _moveOffset);
        if (_left) endPos += new Vector2(_moveOffset, 0);
        if (_right) endPos -= new Vector2(_moveOffset, 0);

        float time = 0f;

        while (time < _duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / _duration);

            if (_fade) _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            _rectTransform.anchoredPosition = Vector2.Lerp(_originalPosition, endPos, t);

            float scaleT = 1f - Mathf.Pow(1f - t, 3f);

            if (_scale) transform.localScale = Vector3.Lerp(_originalScale, Vector3.one * _startScale, scaleT);

            yield return null;
        }

        _canvasGroup.alpha = 0f;
        _rectTransform.anchoredPosition = _originalPosition;
        if (_scale) transform.localScale = _originalScale;

        gameObject.SetActive(false);

        _uiAnimation = false;
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        _originalScale = transform.localScale;
    }
}