using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIAnimator : MonoBehaviour
{
    public float duration = 0.1f;
    public float delay = 0f;
    public Vector3 scaleShown = Vector3.one;
    public Vector3 scaleHidden = Vector3.zero;

    private Coroutine currentRoutine;
    private bool isAnimating = false;

    private void OnEnable()
    {
        transform.localScale = scaleHidden;
        AnimateScale(scaleShown);
    }

    private void OnDisable()
    {
        if (isAnimating)
            StopAllCoroutines();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.localScale = scaleHidden;
        AnimateScale(scaleShown);
    }

    public void Close()
    {
        AnimateScale(scaleHidden, () =>
        {
            gameObject.SetActive(false);
        });
    }

    private void AnimateScale(Vector3 targetScale, System.Action onComplete = null)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ScaleRoutine(targetScale, onComplete));
    }

    private IEnumerator ScaleRoutine(Vector3 targetScale, System.Action onComplete)
    {
        isAnimating = true;

        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        isAnimating = false;
        onComplete?.Invoke();
    }
}