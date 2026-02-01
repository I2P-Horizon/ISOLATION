using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(DecalProjector))]
public class CharacterShadow : MonoBehaviour
{
    [SerializeField] private Transform _character;
    private DecalProjector _decal;

    [SerializeField] private float _maxJumpHeight = 2f;
    [SerializeField] private float _fadeSpeed = 2f;

    private void Awake()
    {
        _decal = GetComponent<DecalProjector>();

        if (_character == null) return;

        Color c = _decal.material.color;
        c.a = 1f;
        _decal.material.color = c;
    }

    private void Update()
    {
        if (_character == null || TimeManager.Instance == null) return;

        handleHeightFade();
        handleDayNightFade();
    }

    /// <summary>
    /// 점프 높이에 따른 그림자 페이드
    /// </summary>
    private void handleHeightFade()
    {
        float heightAlpha = Mathf.Clamp01(_character.position.y / _maxJumpHeight);
        Color c = _decal.material.color;

        float dayNightAlpha = TimeManager.Instance.IsNight ? 0f : 1f;
        c.a = Mathf.Min(1f - heightAlpha, dayNightAlpha);

        _decal.material.color = c;
    }

    /// <summary>
    /// 낮/밤에 따른 부드러운 페이드
    /// </summary>
    private void handleDayNightFade()
    {
        Color c = _decal.material.color;
        float targetAlpha = TimeManager.Instance.IsNight ? 0f : 1f;

        c.a = Mathf.MoveTowards(c.a, targetAlpha, _fadeSpeed * Time.deltaTime);
        _decal.material.color = c;

        _decal.enabled = c.a > 0.01f;
    }
}