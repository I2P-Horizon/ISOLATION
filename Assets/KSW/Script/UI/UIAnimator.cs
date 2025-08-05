using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Transform ��ġ�� �ִϸ��̼��ϴ� Ŭ����
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
/// Transform �������� �ִϸ��̼��ϴ� Ŭ����
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
/// UI Ȯ��/��� �ִϸ��̼��� �����ϴ� ������Ʈ
/// </summary>
[RequireComponent(typeof(RectTransform))]
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

    private Coroutine currentScaleRoutine;
    private Coroutine currentMoveRoutine;

    private ScaleAnimator scaleAnimator;
    private MoveAnimator moveAnimator;

    private void Awake()
    {
        scaleAnimator = new ScaleAnimator(transform);
        moveAnimator = new MoveAnimator(transform);
    }

    private void OnEnable()
    {
        if (useScale) transform.localScale = scaleHidden;
        if (useMove) transform.localPosition = positionHidden;

        PlayAnimation();
    }

    private void OnDisable()
    {
        if (currentScaleRoutine != null) StopCoroutine(currentScaleRoutine);
        if (currentMoveRoutine != null) StopCoroutine(currentMoveRoutine);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (useScale) transform.localScale = scaleHidden;
        if (useMove) transform.localPosition = positionHidden;

        PlayAnimation();
    }

    public void Close()
    {
        PlayReverseAnimation(() =>
        {
            gameObject.SetActive(false);
        });
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
    }

    private void PlayReverseAnimation(Action onComplete = null)
    {
        int completed = 0;
        int required = (useScale ? 1 : 0) + (useMove ? 1 : 0);

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
    }
}