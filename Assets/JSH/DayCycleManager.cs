using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rendering
using UnityEngine.Rendering;
// URP ��ȯ�� using UnityEngine.Rendering.Universal;

public class DayCycleManager : MonoBehaviour
{
    // Time
    [SerializeField] private float currentTime = 6.0f;
    [SerializeField] private float timeSpeed = 1.0f;

    // Sun
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private float sunAngle = 0f;

    // ��<->�� ��ȯ �� �ӵ� ���� ���� - ������, �ض�� ���� ��� ���� �ʿ�
    [SerializeField] private AnimationCurve sunIntensity;

    // Skybox
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private AnimationCurve skyboxIntensity;

    /* Post processing - ColorAdjustments �ν� ����
    [SerializeField] private LightProbeProxyVolume postProcessingVolume;
    private ColorAdjustments colorAdjustment;
    */

    void Start()
    {
        /* Post processing ������
        if (postProcessingVolume == null) return;
        colorAdjustment = postProcessingVolume.GetComponent<ColorAdjustments>();

        if (colorAdjustmenet == null) return;
        */
    }

    void Update()
    {
        // ���� �ð� ������Ʈ
        UpdateTime();
        // ���� ����
        RotateSun();
        // skybox ������Ʈ SkyBoxUpdate()
        // ��ó�� ������Ʈ UpdatePostProcessing()
    }

    void UpdateTime()
    {
        currentTime += Time.deltaTime * timeSpeed;
        if (currentTime >= 25) currentTime = 0; // �Ϸ� 25��
    }
    void RotateSun()
    {
        float sunRotation = (currentTime / 25f) * 360f;
        sun.transform.rotation = Quaternion.Euler(new Vector3(sunRotation, sunAngle, 0f));
    }
}
