using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScreenEffectManager : MonoBehaviour
{
    [System.Serializable]
    public class ConditionFullscreenEffect
    {
        public ConditionType conditionType;

        [Header("URP")]
        public ScriptableRendererFeature rendererFeature;
        public Material material;

        [Header("Effect Param")]
        [Range(0f, 1f)] public float intensity = 0.5f;
        public string intensityProperty = "_Intensity";
    }

    [SerializeField]
    private List<ConditionFullscreenEffect> effects;

    private PlayerState _playerState;
    private PlayerCondition _playerCondition;

    private Dictionary<ConditionType, bool> _lastState = new();

    private void Awake()
    {
        _playerCondition = FindObjectOfType<PlayerCondition>();
        _playerState = FindObjectOfType<PlayerState>();

        foreach (var e in effects)
        {
            e.rendererFeature.SetActive(false);

            if (!_lastState.ContainsKey(e.conditionType))
                _lastState.Add(e.conditionType, false);

            if (e.material.HasProperty(e.intensityProperty))
                e.material.SetFloat(e.intensityProperty, 0f);
        }
    }

    private void Update()
    {
        if (_playerState == null || _playerCondition == null) return;

        lowHpEffect();
        conditionEffects();
    }

    /// <summary>
    /// LowHP 효과
    /// </summary>
    private void lowHpEffect()
    {
        if (effects.Count == 0) return;

        var lowHpEffect = effects[0];
        bool isLowHp = _playerState.GetCurrentHp() <= 20;

        lowHpEffect.rendererFeature.SetActive(isLowHp);

        if (lowHpEffect.material.HasProperty(lowHpEffect.intensityProperty))
            lowHpEffect.material.SetFloat(lowHpEffect.intensityProperty, isLowHp ? lowHpEffect.intensity : 0f);
    }

    /// <summary>
    /// 상태 이상 효과
    /// </summary>
    private void conditionEffects()
    {
        for (int i = 1; i < effects.Count; i++)
        {
            var e = effects[i];
            bool hasCondition = _playerCondition.HasCondition(e.conditionType);

            if (_lastState[e.conditionType] != hasCondition)
            {
                e.rendererFeature.SetActive(hasCondition);
                _lastState[e.conditionType] = hasCondition;
            }

            if (hasCondition && e.material.HasProperty(e.intensityProperty))
                e.material.SetFloat(e.intensityProperty, e.intensity);
        }
    }
}