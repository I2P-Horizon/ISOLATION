using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GatherableObject : InteractiveObject
{
    /// <summary>
    /// 내구도 표시 UI
    /// </summary>
    [SerializeField] private GameObject durabilityUI;

    private Slider durabilitySlider;
    private Canvas durabilityCanvas;

    /// <summary>
    /// 플레이어와 상호작용하고 몇초가 지난 후 UI를 숨길건지
    /// </summary>
    [SerializeField] private float UIHideTime = 3.0f;
    private Coroutine hideUICoroutine;

    private void Awake()
    {
        if (durabilityUI != null)
        {
            durabilityCanvas = durabilityUI.GetComponentInChildren<Canvas>();
            durabilitySlider = durabilityUI.GetComponentInChildren<Slider>();

            durabilitySlider.maxValue = hp;
            durabilitySlider.value = hp;

            durabilitySlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (durabilityCanvas != null)
        {
            // UI가 카메라를 바라보도록 방향 전환
            durabilityCanvas.transform.rotation = Quaternion.LookRotation(durabilityCanvas.transform.position - Camera.main.transform.position);
        }
    }

    /// <summary>
    /// 채집 상호작용 시 내구도를 깎는 함수 (플레이어가 상호작용 시 호출)
    /// </summary>
    /// <param name="amount">줄일 내구도 수치</param>
    public override void Interact(float amount)
    {
        hp -= amount;

        if (hp > 0)
        {
            if (durabilityUI != null)
            {
                durabilitySlider.value = hp;

                // 상호작용 하면 UI 활성화
                durabilitySlider.gameObject.SetActive(true);

                if (hideUICoroutine != null)
                {
                    StopCoroutine(hideUICoroutine);
                }

                hideUICoroutine = StartCoroutine(HideUIAfterDelay(UIHideTime));
            }
        }
        else
        {
            DestroyObject();
        }
    }

    private IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (durabilityCanvas != null)
        {
            durabilitySlider.gameObject.SetActive(false);
        }
    }
}