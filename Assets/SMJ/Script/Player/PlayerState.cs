using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� ����(ü��, ������ ��)�� �����ϴ� ��ũ��Ʈ
/// </summary>
public class PlayerState : MonoBehaviour
{
    private Player _player;

    // �÷��̾� ����
    [Header("State")]
    [SerializeField] private float _maxHp = 100.0f;
    [SerializeField] private float _maxSatiety = 100.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _attackSpeed = 1.0f;
    [SerializeField] private float _jumpHeight = 2.0f;

    public float MaxHp => _maxHp;
    public float MaxSatiety => _maxSatiety;
    public float MoveSpeed => _moveSpeed;
    public float AttackSpeed => _attackSpeed;
    public float JumpHeight => _jumpHeight;

    [SerializeField] private float _hp;
    [SerializeField] private float _satiety;

    // ���� ��� ����
    private bool _die = false;

    public bool Die => _die;

    private float _timeSinceZeroSatiety = 0; // ���ָ� ���� �ð�

    public bool IsSatietyZero => _satiety <= 0;

    [Header("Hunger Penalty Settings")]
    [SerializeField] private float _hpDecreaseInterval = 1.0f;
    [SerializeField] private float _hpDecreaseAmount = 0.1f;

    private void Awake()
    {
        _player = GetComponent<Player>();

        _hp = MaxHp;
        _satiety = MaxSatiety;
    }

    private void Update()
    {
        if (_satiety <= 0 && !_die)
        {
            _timeSinceZeroSatiety += Time.deltaTime;

            float hpLose = _player.Movement.IsMoving ? _hpDecreaseAmount * 2f : _hpDecreaseAmount;

            if (_timeSinceZeroSatiety >= _hpDecreaseInterval)
            {
                _timeSinceZeroSatiety = 0;
                _hp -= hpLose;

                if (_hp <= 0)
                {
                    _hp = 0;
                    _die = true;
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

    public void RestoreFullSatiety() { _satiety -= _maxSatiety; }

    /// <returns>�̹� �������� ���� �� ������ false</returns>
    public bool IncreaseSatiety(float amount)
    {
        if (_satiety >= _maxSatiety)
            return false;

        _satiety = Mathf.Min(_maxSatiety, _satiety + amount);

        return true;
    }

    /// <returns>�̹� hp�� ���� �� ������ false</returns>
    public bool IncreaseHp(float amount)
    {
        if (_hp >= _maxHp)
            return false;

        _hp = Mathf.Min(_maxHp, _hp + amount);

        return true;
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
