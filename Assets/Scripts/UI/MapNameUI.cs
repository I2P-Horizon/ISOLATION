using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNameUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public Text mapNameText;
    public string mapName;

    private float duration = 1f;
    private float displayDuration = 2f;

    private void OnEnable()
    {
        canvasGroup.alpha = 0;
        mapNameText.text = mapName;
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return Fade(0, 1);
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1, 0);
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while(elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
