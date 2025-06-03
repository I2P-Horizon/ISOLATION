using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� ����(ü��, ������ ��)�� �����ϴ� ��ũ��Ʈ.
/// �̱������� �̷���� ������ ���� �� PlayerState.Instance.�޼��� ���.
/// </summary>
public class PlayerState : MonoBehaviour
{
    // �÷��̾� ����
    [Header("State")]
    [SerializeField] private float _maxHp = 100.0f;
    [SerializeField] private float _maxSatiety = 100.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _attackSpeed = 1.0f;

    public float MaxHp => _maxHp;
    public float MaxSatiety => _maxSatiety;
    public float MoveSpeed => _moveSpeed;
    public float AttackSpeed => _attackSpeed;

    [SerializeField] private float _hp;
    [SerializeField] private float _satiety;

    // ���� ��� ����
    private bool _die = false;

    public bool Die => _die;

    private float _timeSinceZeroSatiety = 0; // ���ָ� ���� �ð�

    [Header("Hunger Penalty Settings")]
    [SerializeField] private float _hpDecreaseInterval = 1.0f;
    [SerializeField] private float _hpDecreaseAmount = 0.1f;

    // �̱���
    public static PlayerState Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _hp = MaxHp;
        _satiety = MaxSatiety;
    }

    private void Update()
    {
        if (_satiety <= 0 && !_die)
        {
            _timeSinceZeroSatiety += Time.deltaTime;

            if (_timeSinceZeroSatiety >= _hpDecreaseInterval)
            {
                _timeSinceZeroSatiety = 0;
                _hp -= _hpDecreaseAmount;

                if (_hp <= 0)
                {
                    _hp = 0;
                    _die = true;
                    StartCoroutine(GameManager.Instance.GameOver());
                    Debug.Log("Die......!");
                }
            }
        }
        else
        {
            _timeSinceZeroSatiety = 0;
        }
    }

    // Getter
    public float GetCurrentHp() { return _hp; }

    public float GetCurrentSatiety() { return _satiety; }

    // Setter
    public void RestoreFullHp() { _hp = _maxHp; }

    public void RestoreHullSatiety() { _satiety -= _maxSatiety; }

    public void IncreaseSatiety(float amount)
    {
        _satiety = Mathf.Min(_maxSatiety, _satiety + amount);
    }

    public void IncreaseHp(float amount)
    {
        _hp = Mathf.Min(_maxHp, _hp + amount);
    }

    public void DecreaseSatiety(float amount)
    {
        _satiety = Mathf.Max(0, _satiety - amount);
    }

    public void DecreaseHP(float amount)
    {
        _hp = Mathf.Max(0, _hp - amount);
    }

    public void SetMoveSpeed(float speed)
    {
        if (speed > 0)
        {
            _moveSpeed = speed;
        }
    }

    public void SetAttackSpeed(float speed)
    {
        if (speed > 0)
        {
            _attackSpeed = speed;
        }
    }
}
