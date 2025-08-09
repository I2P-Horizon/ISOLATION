using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rendering
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    // Observer
    private List<ICycleListener> listeners = new List<ICycleListener>();
    // 외부에서는 값 읽기만, 내부에서는 값 수정 가능
    public static TimeManager instance { get;  private set; } // set 잠깐 private 없애둠 JSG FLAG

    public bool IsNight => isNight;

    public float CurrentTime => currentTime;
    public float RealTimePerDaySec => realTimePerDaySec;

    // Time
    [Header("Time")]
    [SerializeField] private float realTimePerDay;
    [SerializeField] private float realTimePerDaySec;
    [SerializeField] private float currentTime;
    [SerializeField] private float timeSpeed = 1.0f;
    [SerializeField] private int dayStartTime;
    [SerializeField] private int nightStartTime;
    private const int minToSec = 60;

    // Flag
    [SerializeField] private bool isNight;
    private bool previousIsNight;

    // Sun
    [Header("Sun")]
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private float sunAngle = -90f;
    [SerializeField] private AnimationCurve sunIntensity;

    // Skybox
    [Header("Skybox")]
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private AnimationCurve skyboxIntensity;

    // Post processing
    [Header("PostProcessing")]
    [SerializeField] private Volume postProcessingVolume;
    private ColorAdjustments colorAdjustment;

    // Color
    private Color dayColor = new Color(1.0f, 1.0f, 1.0f);
    private Color nightColor = new Color(0.6f, 0.7f, 1.0f);

    // Singleton
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        previousIsNight = isNight;
        currentTime = dayStartTime;

        realTimePerDaySec = realTimePerDay * minToSec;

        dayStartTime = (int)realTimePerDaySec / 4;       // 0.25 == 25.0%
        nightStartTime = (int)realTimePerDaySec * 7 / 8; // 0.875 == 87.5%

        if (postProcessingVolume == null) return;
        postProcessingVolume.profile.TryGet(out ColorAdjustments _colorAdjustment);
        colorAdjustment = _colorAdjustment;

        if (colorAdjustment == null) return;

        // Fog 활성화
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
    }
    private void Update()
    {
        UpdateTime();
        RotateSun();
        UpdateLighting();
        UpdatePostProcessing();

        CheckCycleState();
        NotifyCycleChanged();
    }

    private void UpdateTime()
    {
        currentTime += Time.deltaTime * timeSpeed;
        if (currentTime >= realTimePerDaySec) currentTime = 0; // DayCycle
    }
    private void RotateSun() // JSH FLAG 각도 그대로, Intensity & sun.color 변경 고려 / 직접광 변경 최소화, 간접광(Skybox) 변경을 통한 분위기 변화 구현
    {
        // -90 ~ 270 Rotate
        float sunRotation = Mathf.Lerp(-90, 270, currentTime / realTimePerDaySec);
        sun.transform.rotation = Quaternion.Euler(new Vector3(sunRotation, sunAngle, 0f));
    }
    private void UpdateLighting()
    {
        sun.intensity = sunIntensity.Evaluate(currentTime / realTimePerDaySec);

        // Animation Curve - Keyframe (x) : Value (y)
        // 0 ~ 0.25 : sunIntensity 0.8
        // 0.25 ~ 0.5 : sunIntensity 1
        // 0.5 ~ 0.8 : sunIntensity 0.8
        // 0.8 ~ 0.875 : sunIntensity 0
        // 0.875 ~ 0 : sunIntensity 0

        sun.color = sunColor.Evaluate(currentTime / realTimePerDaySec);

        if (skyboxMaterial == null) return;
        skyboxMaterial.SetFloat("_Exposure", skyboxIntensity.Evaluate(currentTime / realTimePerDaySec));
    }
    private void UpdatePostProcessing()
    {
        float timeBlend = (( (currentTime - 10f) >= 0 ) ? (currentTime - 10f) : (10f - currentTime));
        //float timeBlend = Mathf.Abs(currentTime - 10f) / 10f; // 0 ~ 10 
        if (colorAdjustment == null) return;

        colorAdjustment.colorFilter.value = Color.Lerp(dayColor, nightColor, timeBlend);

        if(isNight == true)
        {
            RenderSettings.fogColor = Color.Lerp(dayColor, new Color(0.2f, 0.3f, 0.5f), timeBlend);
            RenderSettings.fogDensity = Mathf.Lerp(0.01f, 0.05f, timeBlend);
        }

        else if(isNight == false)
        {
            RenderSettings.fogColor = Color.Lerp(dayColor, new Color(0.2f, 0.3f, 0.5f), timeBlend);
            RenderSettings.fogDensity = Mathf.Lerp(0.001f, 0.005f, timeBlend);
        }
    }

    public void Register(ICycleListener listener)
    {
        if(listeners.Contains(listener)) return;
        listeners.Add(listener);
    }
    public void UnRegister(ICycleListener listener)
    {
        if (!listeners.Contains(listener)) return;
        listeners.Remove(listener);
    }

    private void NotifyCycleChanged()
    {
        if (isNight != previousIsNight) 
        {
            Debug.Log("Notify DatCycle Changed");
            foreach (var listener in listeners) listener.OnCycleChanged(isNight);
        }

        previousIsNight = isNight;
        
        //Debug.Log(isNight);
        //Debug.Log(previousIsNight);
    }

    private void CheckCycleState()
    {
        if ((int)currentTime >= dayStartTime && (int)currentTime < nightStartTime)
        {
            isNight = false;
            //Debug.Log("Day");
        }

        else if ((nightStartTime <= (int)currentTime && (int)currentTime <= realTimePerDaySec) || (0 <= (int)currentTime && (int)currentTime < dayStartTime))
        {
            isNight = true;
            //Debug.Log("Night");
        }
    }
}
