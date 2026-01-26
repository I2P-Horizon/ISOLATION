using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 스탯(체력, 포만감, 수분량 등)을 관리하는 스크립트
/// </summary>
public class PlayerState : MonoBehaviour
{
    #region ** 변수 **
    private Player _player;

    // 플레이어 스탯
    [Header("Base State")]
    [SerializeField] private float _maxHp = 100.0f;
    [SerializeField] private float _maxSatiety = 100.0f;
    [SerializeField] private float _maxHydration = 100.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _attackSpeed = 1.0f;
    [SerializeField] private float _jumpHeight = 2.0f;

    public float MaxHp => _maxHp;
    public float MaxSatiety => _maxSatiety;
    public float MaxHydration => _maxHydration;
    public float MoveSpeed => _moveSpeed;
    public float AttackSpeed => _attackSpeed;
    public float JumpHeight => _jumpHeight;

    [Header("Current State")]
    [SerializeField] private float _hp;
    [SerializeField] private float _satiety;
    [SerializeField] private float _hydration;

    // 내부 사용 변수
    private bool _die = false;
    public bool Die => _die;

    public bool IsSatietyZero => _satiety <= 0;
    public bool IsHydrationZero => _hydration <= 0;

    [Header("Hp Decrease Settings")]
    [SerializeField] private float _hpNaturalDecreaseInterval = 5.0f; // 체력 자연 감소 간격(초)

    // 포만감 감소 간격
    private const float SATIETY_IDLE_TIME = 30f;
    private const float SATIETY_MOVE_TIME = 20f;
    private const float SATIETY_ACTION_TIME = 8f;

    // 수분 감소 간격
    private const float HYDRATION_IDLE_TIME = 120f;
    private const float HYDRATION_MOVE_TIME = 60f;
    private const float HYDRATION_ACTION_TIME = 12f;

    [Header("Starvation Penalty")]
    [SerializeField] private float _starvationDamageInterval = 1.0f;
    [SerializeField] private float _starvationDamageAmount = 0.1f;

    // 내부 타이머
    private float _hpNaturalTimer = 0f;
    private float _satietyTimer = 0f;
    private float _hydrationTimer = 0f;
    private float _starvationTimer = 0f;
    #endregion

    private void Awake()
    {
        _player = GetComponent<Player>();

        _hp = MaxHp;
        _satiety = MaxSatiety;
        _hydration = MaxHydration;
    }

    private void Update()
    {
        if (_die) return;

        handleNaturalHpDecrease();
        handleSatietyDecrease();
        handleHydrationDecrease();
        handleStarvationDamage();
    }

    /// <summary>
    /// 체력 자연 감소 처리 메서드 (5초마다 1씩 감소, 최소 1 유지)
    /// </summary>
    private void handleNaturalHpDecrease()
    {
        if (_hp <= 1.0f)
        {
            _hpNaturalTimer = 0f;
            return;
        }

        _hpNaturalTimer += Time.deltaTime;
        if (_hpNaturalTimer >= _hpNaturalDecreaseInterval)
        {
            _hpNaturalTimer = 0f;
            DecreaseHP(1.0f);

            // 자연 감소로는 체력이 1 미만으로 떨어지지 않도록 함
            if (_hp < 1.0f) _hp = 1.0f;
        }
    }

    /// <summary>
    /// 포만감 감소 처리 메서드
    /// 정지: 30초마다 1 감소, 이동: 20초마다 1 감소, 행동(채집, 공격 등): 8초마다 1 감소
    /// </summary>
    private void handleSatietyDecrease()
    {
        if (_satiety <= 0) return;

        _satietyTimer += Time.deltaTime;

        float targetInterval = SATIETY_IDLE_TIME;

        if (_player.Interaction.IsInteracting)
        {
            targetInterval = SATIETY_ACTION_TIME;
        }
        else if (_player.Movement.IsMoving)
        {
            targetInterval = SATIETY_MOVE_TIME;
        }

        if (_satietyTimer >= targetInterval)
        {
            _satietyTimer = 0f;
            DecreaseSatiety(1.0f);
        }
    }

    /// <summary>
    /// 수분량 감소 처리 메서드
    /// 정지: 120초마다 1 감소, 이동: 60초마다 1 감소, 행동(채집, 공격 등): 12초마다 1 감소
    /// </summary>
    private void handleHydrationDecrease()
    {
        if (_hydration <= 0) return;

        _hydrationTimer += Time.deltaTime;

        float targetInterval = HYDRATION_IDLE_TIME;

        if (_player.Movement.IsMoving)
        {
            targetInterval = HYDRATION_MOVE_TIME;
        }
        else if (_player.Interaction.IsInteracting)
        {
            targetInterval = HYDRATION_ACTION_TIME;
        }

        if (_hydrationTimer >= targetInterval)
        {
            _hydrationTimer = 0f;
            DecreaseHydration(1.0f);
        }
    }

    private void handleStarvationDamage()
    {
        if (_satiety <= 0)
        {
            _starvationTimer += Time.deltaTime;

            float damage = _player.Movement.IsMoving ? _starvationDamageAmount * 2 : _starvationDamageAmount;

            if (_starvationTimer >= _starvationDamageInterval)
            {
                _starvationTimer = 0f;
                _hp -= damage;

                if (_hp <= 0)
                {
                    _hp = 0;
                    _die = true;
                }
            }
        }
        else
        {
            _starvationTimer = 0f;
        }
    }

    public float GetCurrentHp()
    {
        return _hp;
    }

    public float GetCurrentSatiety()
    {
        return _satiety;
    }

    public bool IncreaseHp(float amount)
    {
        if (_hp >= _maxHp) return false;
        _hp = Mathf.Min(_hp + amount, _maxHp);
        return true;
    }

    public bool IncreaseSatiety(float amount)
    {
        if (_satiety >= _maxSatiety) return false;
        _satiety = Mathf.Min(_satiety + amount, _maxSatiety);
        return true;
    }

    public bool IncreaseHydration(float amount)
    {
        if (_hydration >= _maxHydration) return false;
        _hydration = Mathf.Min(_hydration + amount, _maxHydration);
        return true;
    }

    public void DecreaseHP(float amount)
    {
        _hp = Mathf.Max(0, _hp - amount);
        if (_hp <= 0)
        {
            _hp = 0;
            _die = true;
        }
    }

    public void DecreaseSatiety(float amount)
    {
        _satiety = Mathf.Max(0, _satiety - amount);
    }

    public void DecreaseHydration(float amount)
    {
        _hydration = Mathf.Max(0, _hydration - amount);
    }

    public float GetCurrentHydration()
    {
        return _hydration;
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
