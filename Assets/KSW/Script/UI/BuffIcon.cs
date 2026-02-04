using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    public ConditionType Type { get; private set; }

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 버프 타입, 아이콘 초기화
    /// </summary>
    public void Init(ConditionType type, Sprite sprite)
    {
        Type = type;
        GetComponent<Image>().sprite = sprite;
    }

    /// <summary>
    /// 지정된 위치로 부드럽게 이동
    /// </summary>
    public void MoveTo(Vector2 target, float time = 0.2f)
    {
        StopAllCoroutines();
        StartCoroutine(moveRoutine(target, time));
    }

    /// <summary>
    /// 실제 위치 이동 애니메이션 코루틴
    /// </summary>
    private IEnumerator moveRoutine(Vector2 target, float time)
    {
        Vector2 start = _rect.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            _rect.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        _rect.anchoredPosition = target;
    }

    /// <summary>
    /// 애니메이션 없이 즉시 위치 설정
    /// </summary>
    /// <param name="pos"></param>
    public void SetImmediatePosition(Vector2 pos)
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
    }
}