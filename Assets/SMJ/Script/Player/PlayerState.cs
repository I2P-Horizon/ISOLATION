using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // 플레이어 스탯
    [Header("State")]
    [SerializeField] private float _hp = 100.0f;
    [SerializeField] private float _satiety = 100.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _attackSpeed = 1.0f;

    public float HP => _hp;
    public float Ssatiety => _satiety;
    public float MoveSpeed => _moveSpeed;
    public float AttackSpeed => _attackSpeed;

    // 내부 사용 변수
    private bool _die = false;

    public bool Die => _die;

    private float _timeSinceZeroSatiety = 0; // 굶주림 지속 시간

    [Header("Hunger Penalty Settings")]
    [SerializeField] private float _hpDecreaseInterval = 1.0f; 
    [SerializeField] private float _hpDecreaseAmount = 0.1f;

    // 싱글톤
    public static PlayerState Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance !=  this)
        {
            Destroy(gameObject);
            return;
        }    
        
        Instance = this;
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
                    Debug.Log("Die......!");
                }
            }
        }
        else
        {
            _timeSinceZeroSatiety = 0;
        }
    }

    public void DecreaseSatiety(float amount)
    {
        _satiety = Mathf.Max(0, _satiety -  amount);
    }

    public void DecreaseHP(float amount)
    {
        _hp = Mathf.Max(0, _hp - amount);
    }
}
