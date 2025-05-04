using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GatherableObject : InteractiveObject
{
    /// <summary>
    /// ������ ǥ�� UI
    /// </summary>
    [SerializeField] private GameObject durabilityUI;

    private Slider durabilitySlider;
    private Canvas durabilityCanvas;

    /// <summary>
    /// �÷��̾�� ��ȣ�ۿ��ϰ� ���ʰ� ���� �� UI�� �������
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
            // UI�� ī�޶� �ٶ󺸵��� ���� ��ȯ
            durabilityCanvas.transform.rotation = Quaternion.LookRotation(durabilityCanvas.transform.position - Camera.main.transform.position);
        }
    }

    /// <summary>
    /// ä�� ��ȣ�ۿ� �� �������� ��� �Լ� (�÷��̾ ��ȣ�ۿ� �� ȣ��)
    /// </summary>
    /// <param name="amount">���� ������ ��ġ</param>
    public override void Interact(float amount)
    {
        hp -= amount;

        if (hp > 0)
        {
            if (durabilityUI != null)
            {
                durabilitySlider.value = hp;

                // ��ȣ�ۿ� �ϸ� UI Ȱ��ȭ
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