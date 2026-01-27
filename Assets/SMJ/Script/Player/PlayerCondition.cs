using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    private Player _player;

    // 활성화된 상태 저장 (상태타입, 남은시간)
    // 남은시간이 -1이면 무한 지속 상태
    private Dictionary<ConditionType, float> _activeConditions = new Dictionary<ConditionType, float>();

    // 상태별 틱 타이머 저장 (상태타입, 마지막 발동 후 경과 시간)
    private Dictionary<ConditionType, float> _tickTimers = new Dictionary<ConditionType, float>();

    // 기본 스탯 저장 (버프/디버프 해제 시 복구용)
    private float _baseMoveSpeed;
    private float _baseAttackSpeed;

    public bool IsConfused;

    private void Start()
    {
        _player = GetComponent<Player>();

        _baseMoveSpeed = _player.State.MoveSpeed;
        _baseAttackSpeed = _player.State.AttackSpeed;
    }

    private void Update()
    {
        if (_player.State.Die) return;

        updateTimersAndEffects();
        checkDehydrationAuto();
    }

    private void updateTimersAndEffects()
    {
        List<ConditionType> keys = new List<ConditionType>(_activeConditions.Keys);

        foreach (var type in keys)
        {
            handleTickEffect(type);

            float remainingTime = _activeConditions[type];

            if (remainingTime != -1)
            {
                remainingTime -= Time.deltaTime;
                _activeConditions[type] = remainingTime;

                if (remainingTime <= 0)
                {
                    RemoveCondition(type);
                }
            }
        }
    }

    private void handleTickEffect(ConditionType type)
    {
        if (!_tickTimers.ContainsKey(type)) _tickTimers[type] = 0f;

        float interval = getTickInterval(type);

        if (interval > 0)
        {
            _tickTimers[type] += Time.deltaTime;

            if (_tickTimers[type] >= interval)
            {
                _tickTimers[type] = 0f;
                applyInstantEffect(type);
            }
        }
    }

    private float getTickInterval(ConditionType type)
    {
        switch (type)
        {
            case ConditionType.Bleeding: return 2.0f;
            case ConditionType.Frostbite: return 5.0f;
            case ConditionType.Heatstroke: return 3.0f;
            case ConditionType.Indigestion: return 10.0f;
            case ConditionType.Dehydration: return 5.0f;
            default: return 0f;
        }
    }

    private void applyInstantEffect(ConditionType type)
    {
        switch (type)
        {
            case ConditionType.Bleeding:
                _player.State.DecreaseHP(3.0f);
                break;
            case ConditionType.Frostbite:
                _player.State.DecreaseHP(2.0f);
                break;
            case ConditionType.Heatstroke:
                _player.State.DecreaseHP(2.0f);
                break;
            case ConditionType.Indigestion:
                _player.State.DecreaseHP(1.0f);
                break;
            case ConditionType.Dehydration:
                _player.State.DecreaseHP(3.0f);
                break;
        }
    }

    public void AddCondition(ConditionType type, float duration)
    {
        // 저항 버프가 있을 경우 해당 디버프 면역
        if (type == ConditionType.Frostbite && HasCondition(ConditionType.FrostResist)) return;
        if (type == ConditionType.Heatstroke && HasCondition(ConditionType.FireResist)) return;

        // 상반되는 상태 처리
        if (type == ConditionType.Heatstroke)
        {
            RemoveCondition(ConditionType.Frostbite);
        }
        else if (type == ConditionType.Frostbite)
        {
            RemoveCondition(ConditionType.Heatstroke);
        }
        else if (type == ConditionType.FireResist ||
                 type == ConditionType.FrostResist)
        {
            RemoveCondition(type == ConditionType.FireResist ? ConditionType.Heatstroke : ConditionType.Frostbite);
        }

        // 상태 추가 또는 갱신
        if (_activeConditions.ContainsKey(type))
        {
            if (duration == -1 || _activeConditions[type] < duration)
            {
                _activeConditions[type] = duration;
            }
        }
        else
        {
            _activeConditions.Add(type, duration);

            if (_tickTimers.ContainsKey(type)) _tickTimers.Add(type, 0f);
            else _tickTimers[type] = 0f;

                recalculateStats();
            Debug.Log($"[PlayerCondition] Added: {type}");
        }
    }

    public void RemoveCondition(ConditionType type)
    {
        if (_activeConditions.ContainsKey(type))
        {
            _activeConditions.Remove(type);

            if (_tickTimers.ContainsKey(type)) _tickTimers.Remove(type);

            recalculateStats();
            Debug.Log($"[PlayerCondition] Removed: {type}");
        }
    }

    public bool HasCondition(ConditionType type)
    {
        return _activeConditions.ContainsKey(type);
    }

    private void recalculateStats()
    {
        float moveSpeedMultiplier = 1.0f;
        float attackSpeedMultiplier = 1.0f;

        // 디버프 계산
        if (HasCondition(ConditionType.Frostbite))
        {
            moveSpeedMultiplier *= 0.95f;
        }
        if (HasCondition(ConditionType.Indigestion))
        {
            moveSpeedMultiplier *= 0.97f;
        }
        if (HasCondition(ConditionType.Dehydration))
        {
            moveSpeedMultiplier *= 0.97f;
            attackSpeedMultiplier *= 0.97f;
        }

        // 버프 계산
        if (HasCondition(ConditionType.StrengthUp))
        {
            attackSpeedMultiplier *= 1.02f;
        }
        if (HasCondition(ConditionType.Electrolyte))
        {
            moveSpeedMultiplier *= 1.02f;
        }

        _player.State.SetMoveSpeed(_baseMoveSpeed * moveSpeedMultiplier);
        _player.State.SetAttackSpeed(_baseAttackSpeed * attackSpeedMultiplier);
    }

    private void checkDehydrationAuto()
    {
        float hydrationPercent = (_player.State.GetCurrentHydration() / _player.State.MaxHydration) * 100f;
        
        if (hydrationPercent < 30.0f)
        {
            if (!HasCondition(ConditionType.Dehydration))
            {
                AddCondition(ConditionType.Dehydration, -1);
            }
        }
        else
        {
            if (HasCondition(ConditionType.Dehydration))
            {
                RemoveCondition(ConditionType.Dehydration);
            }
        }
    }
}
