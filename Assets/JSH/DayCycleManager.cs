using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rendering
using UnityEngine.Rendering;
// URP 전환시 using UnityEngine.Rendering.Universal;

public class DayCycleManager : MonoBehaviour
{
    // Time
    [SerializeField] private float currentTime = 6.0f;
    [SerializeField] private float timeSpeed = 1.0f;

    // Sun
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private float sunAngle = 0f;

    // 낮<->밤 전환 간 속도 조절 가능 - 해질녘, 해뜰녘 넣을 경우 조정 필요
    [SerializeField] private AnimationCurve sunIntensity;

    // Skybox
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private AnimationCurve skyboxIntensity;

    /* Post processing - ColorAdjustments 인식 오류
    [SerializeField] private LightProbeProxyVolume postProcessingVolume;
    private ColorAdjustments colorAdjustment;
    */

    void Start()
    {
        /* Post processing 진행중
        if (postProcessingVolume == null) return;
        colorAdjustment = postProcessingVolume.GetComponent<ColorAdjustments>();

        if (colorAdjustmenet == null) return;
        */
    }

    void Update()
    {
        // 현재 시간 업데이트
        UpdateTime();
        // 광원 각도
        RotateSun();
        // skybox 업데이트 SkyBoxUpdate()
        // 후처리 업데이트 UpdatePostProcessing()
    }

    void UpdateTime()
    {
        currentTime += Time.deltaTime * timeSpeed;
        if (currentTime >= 25) currentTime = 0; // 하루 25분
    }
    void RotateSun()
    {
        float sunRotation = (currentTime / 25f) * 360f;
        sun.transform.rotation = Quaternion.Euler(new Vector3(sunRotation, sunAngle, 0f));
    }
}
