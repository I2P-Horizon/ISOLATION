using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GatherableObject : DestructibleObject
{
    /// <summary>
    /// ������ ǥ�� UI
    /// </summary>
    [SerializeField] private GameObject _hpUI;

    private Slider _hpSlider;
    private Canvas _hpCanvas;

    /// <summary>
    /// �÷��̾�� ��ȣ�ۿ��ϰ� ���ʰ� ���� �� UI�� �������
    /// </summary>
    [SerializeField] private float _UIHideTime = 3.0f;
    private Coroutine _hideUICoroutine;

    private void Awake()
    {
        if (_hpUI != null)
        {
            _hpCanvas = _hpUI.GetComponentInChildren<Canvas>();
            _hpSlider = _hpUI.GetComponentInChildren<Slider>();

            _hpSlider.maxValue = _hp;
            _hpSlider.value = _hp;

            _hpSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_hpCanvas != null)
        {
            // UI�� ī�޶� �ٶ󺸵��� ���� ��ȯ
            _hpCanvas.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
        }
    }

    /// <summary>
    /// ä�� ��ȣ�ۿ� �� �������� ��� �Լ� (�÷��̾ ��ȣ�ۿ� �� ȣ��)
    /// </summary>
    /// <param name="amount">���� ������ ��ġ</param>
    public override void Interact(object context = null)
    {
        if (context is float amount)
        {
            _hp -= amount;

            if (_hp > 0)
            {
                if (_hpUI != null)
                {
                    _hpSlider.value = _hp;

                    // ��ȣ�ۿ� �ϸ� UI Ȱ��ȭ
                    _hpSlider.gameObject.SetActive(true);

                    if (_hideUICoroutine != null)
                    {
                        StopCoroutine(_hideUICoroutine);
                    }

                    _hideUICoroutine = StartCoroutine(HideUIAfterDelay(_UIHideTime));
                }
            }
            else
            {
                DestroyObject();
            }
        }
    }

    private IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_hpCanvas != null)
        {
            _hpSlider.gameObject.SetActive(false);
        }
    }
}