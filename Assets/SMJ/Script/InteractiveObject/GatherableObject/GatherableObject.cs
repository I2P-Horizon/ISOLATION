using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GatherableObject : DestructibleObject
{
    [Header("UI Settings")]
    /// <summary>
    /// 내구도 표시 UI
    /// </summary>
    [SerializeField] private GameObject _hpUI;

    private Slider _hpSlider;
    private Canvas _hpCanvas;

    /// <summary>
    /// 플레이어와 상호작용하고 몇초가 지난 후 UI를 숨길건지
    /// </summary>
    [SerializeField] private float _UIHideTime = 3.0f;
    private Coroutine _hideUICoroutine;

    [Header("Respawn Settings")]
    [SerializeField] private bool _isRespawnable = true;
    [SerializeField] private float _respawnTime = 180f;
    [SerializeField] private GameObject _destroyEffect;
    [SerializeField] private GameObject _modelObject;

    private Collider _collider;

    protected override void Awake()
    {
        base.Awake();

        _collider = GetComponent<Collider>();

        if (_hpUI != null)
        {
            _hpCanvas = _hpUI.GetComponentInChildren<Canvas>();
            _hpSlider = _hpUI.GetComponentInChildren<Slider>();

            _hpSlider.maxValue = _maxhp;
            _hpSlider.value = _hp;

            _hpSlider.gameObject.SetActive(false);
        }

        if (_isRespawnable && _modelObject == null)
        {
            Debug.LogWarning($"{name}: 리스폰 가능한 오브젝트에 model이 할당되지 않았음.");
        }
    }

    private void Update()
    {
        if (_hpCanvas != null)
        {
            // UI가 카메라를 바라보도록 방향 전환
            _hpCanvas.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
        }
    }

    /// <summary>
    /// 채집 상호작용 시 내구도를 깎는 함수 (플레이어가 상호작용 시 호출)
    /// </summary>
    /// <param name="amount">줄일 내구도 수치</param>
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

                    // 상호작용 하면 UI 활성화
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

    protected override void DestroyObject()
    {
        if (_isDestroyed) return;
        
        _isDestroyed = true;

        DropItems();

        if (_destroyEffect != null)
        {
            Instantiate(_destroyEffect, transform.position, Quaternion.identity);
        }

        if (_isRespawnable)
        {
            if (_hpUI != null) _hpSlider.gameObject.SetActive(false);
            StartCoroutine(respawnProcess());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator respawnProcess()
    {
        if (_collider != null) _collider.enabled = false;
        if (_modelObject != null) _modelObject.SetActive(false);

        yield return new WaitForSeconds(_respawnTime);

        _hp = _maxhp;
        _isDestroyed = false;

        if (_collider != null) _collider.enabled = true;
        if (_modelObject != null) _modelObject.SetActive(true);
        if (_hpSlider != null) _hpSlider.value = _hp;

        OnRespawn();
    }

    protected virtual void OnRespawn() { }
}