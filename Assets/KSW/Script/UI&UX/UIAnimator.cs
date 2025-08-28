using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Transform 위치를 애니메이션하는 클래스
/// </summary>
public class MoveAnimator
{
    private readonly Transform target;

    public MoveAnimator(Transform target)
    {
        this.target = target;
    }

    public IEnumerator AnimateTo(Vector3 targetPosition, float duration, float delay = 0f, Action onComplete = null)
    {
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        Vector3 startPos = target.localPosition;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            target.localPosition = Vector3.Lerp(startPos, targetPosition, SmoothStep(t));
            yield return null;
        }

        target.localPosition = targetPosition;
        onComplete?.Invoke();
    }

    private float SmoothStep(float t) => t * t * (3f - 2f * t);
}

/// <summary>
/// Transform 스케일을 애니메이션하는 클래스
/// </summary>
public class ScaleAnimator
{
    private readonly Transform target;

    public ScaleAnimator(Transform target)
    {
        this.target = target;
    }

    public IEnumerator AnimateTo(Vector3 targetScale, float duration, float delay = 0f, Action onComplete = null)
    {
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        Vector3 startScale = target.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            target.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        target.localScale = targetScale;
        onComplete?.Invoke();
    }
}

/// <summary>
/// UI 확장/축소 애니메이션을 관리하는 컴포넌트
/// </summary>
[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float duration = 0.1f;
    public float delay = 0f;

    [Header("Scale Settings")]
    public bool useScale = true;
    public Vector3 scaleShown = Vector3.one;
    public Vector3 scaleHidden = Vector3.zero;

    [Header("Move Settings")]
    public bool useMove = false;
    public Vector3 positionShown = Vector3.zero;
    public Vector3 positionHidden = new Vector3(0f, 200f, 0f);

    [Header("Fade Settings")]
    public bool useFade = true;

    private CanvasGroup canvasGroup;
    private ScaleAnimator scaleAnimator;
    private MoveAnimator moveAnimator;

    private Coroutine currentScaleRoutine;
    private Coroutine currentMoveRoutine;
    private Coroutine currentFadeRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        scaleAnimator = new ScaleAnimator(transform);
        moveAnimator = new MoveAnimator(transform);
    }

    private void OnEnable()
    {
        if (useScale) transform.localScale = scaleHidden;
        if (useMove) transform.localPosition = positionHidden;
        if (useFade) canvasGroup.alpha = 0f;

        PlayAnimation();
    }

    private void OnDisable()
    {
        if (currentScaleRoutine != null) StopCoroutine(currentScaleRoutine);
        if (currentMoveRoutine != null) StopCoroutine(currentMoveRoutine);
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (useScale) transform.localScale = scaleHidden;
        if (useMove) transform.localPosition = positionHidden;
        if (useFade) canvasGroup.alpha = 0f;

        PlayAnimation();
    }

    public void Close()
    {
        PlayReverseAnimation(() => gameObject.SetActive(false));
    }

    private void PlayAnimation()
    {
        if (useScale)
        {
            if (currentScaleRoutine != null) StopCoroutine(currentScaleRoutine);
            currentScaleRoutine = StartCoroutine(scaleAnimator.AnimateTo(scaleShown, duration, delay));
        }

        if (useMove)
        {
            if (currentMoveRoutine != null) StopCoroutine(currentMoveRoutine);
            currentMoveRoutine = StartCoroutine(moveAnimator.AnimateTo(positionShown, duration, delay));
        }

        if (useFade)
        {
            if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
            currentFadeRoutine = StartCoroutine(FadeTo(1f, duration, delay));
        }
    }

    private void PlayReverseAnimation(Action onComplete = null)
    {
        int completed = 0;
        int required = (useScale ? 1 : 0) + (useMove ? 1 : 0) + (useFade ? 1 : 0);

        Action callback = () =>
        {
            completed++;
            if (completed >= required)
                onComplete?.Invoke();
        };

        if (useScale)
        {
            if (currentScaleRoutine != null) StopCoroutine(currentScaleRoutine);
            currentScaleRoutine = StartCoroutine(scaleAnimator.AnimateTo(scaleHidden, duration, delay, callback));
        }

        if (useMove)
        {
            if (currentMoveRoutine != null) StopCoroutine(currentMoveRoutine);
            currentMoveRoutine = StartCoroutine(moveAnimator.AnimateTo(positionHidden, duration, delay, callback));
        }

        if (useFade)
        {
            if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
            currentFadeRoutine = StartCoroutine(FadeTo(0f, duration, delay, callback));
        }
    }

    private IEnumerator FadeTo(float targetAlpha, float duration, float delay = 0f, Action onComplete = null)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);

        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }
}