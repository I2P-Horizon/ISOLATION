using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get;  private set; }
    public bool IsNight => _isNight;
    public float CurrentTime => _currentTime;
    public float RealTimePerDaySec => _realTimePerDaySec;

    private List<ICycleListener> _listeners = new List<ICycleListener>();



    // Time
    [Header("Time")]
    [SerializeField] private float _realTimePerDay;
    [SerializeField] private float _realTimePerDaySec;
    [SerializeField] private float _currentTime;
    private const float _timeSpeed = 1.0f;
    [SerializeField] private int _dayStartTime;
    [SerializeField] private int _nightStartTime;
    private const int _minToSec = 60;

    // Flag
    [SerializeField] private bool _isNight;
    private bool _previousIsNight;

    // Environment settings
    [Header("Sun")]
    [SerializeField] private Light _sun;
    [SerializeField] private Gradient _sunColor;
    private const float _sunAngle = -90f;
    [SerializeField] private AnimationCurve _sunIntensity;

    [Header("Skybox")]
    [SerializeField] private Material _skyboxMaterial;
    [SerializeField] private AnimationCurve _skyboxIntensity;

    [Header("PostProcessing")]
    [SerializeField] private Volume _postProcessingVolume;
    private ColorAdjustments _colorAdjustment;
    private Color _dayColor = new Color(1.0f, 1.0f, 1.0f);
    //private Color _nightColor = new Color(0.6f, 0.7f, 1.0f);
    private Color _nightColor = new Color(0.2f, 0.2f, 0.4f);

    private bool _isloaded = false;
    [SerializeField] [Range(0.0f,1.0f)] private float _fog = 0.2f;

    // KSW
    private void OnEnable()
    {
        IslandManager.OnGenerationComplete += isLoadComplted;
    }

    private void OnDisable()
    {
        IslandManager.OnGenerationComplete -= isLoadComplted;
    }

    // LifeCycle
    private void Awake()
    {
        if (Instance == null) Instance = this;
        InitTimeManager();
    }
    private void Update()
    {
        if (!_isloaded) return;
        
        UpdateTime();
        UpdateEnvironmentVisuals();

        //JSH TODO: Logic optimization
        CheckCycleState();
        NotifyCycleChanged();
    }

    // KSW
    private void isLoadComplted()
    {
        _isloaded = true;
    }

    public void Register(ICycleListener listener)
    {
        if (_listeners.Contains(listener)) return;
        _listeners.Add(listener);
    }
    public void UnRegister(ICycleListener listener)
    {
        if (!_listeners.Contains(listener)) return;
        _listeners.Remove(listener);
    }

    private void NotifyCycleChanged()
    {
        if (_isNight != _previousIsNight)
        {
            //Debug.Log("Notify DayCycle Changed");
            foreach (var listener in _listeners) listener.OnCycleChanged();
        }
        _previousIsNight = _isNight;
    }

    private void CheckCycleState()
    {
        if ((int)_currentTime >= _dayStartTime && (int)_currentTime < _nightStartTime)
        {
            _isNight = false;
            //Debug.Log("Day");
        }

        else if ((_nightStartTime <= (int)_currentTime && (int)_currentTime <= _realTimePerDaySec) || (0 <= (int)_currentTime && (int)_currentTime < _dayStartTime))
        {
            _isNight = true;
            //Debug.Log("Night");
        }
    }

    private void InitTimeManager()
    {
        _realTimePerDay = 10;
        _realTimePerDaySec = _realTimePerDay * _minToSec;

        // Game start time setting AM 9:00 | 3/8
        _currentTime = (((int)_realTimePerDaySec * 3) / 8);

        // AM 9:00 | 3/8
        _dayStartTime = (((int)_realTimePerDaySec * 3) / 8);
        // Legacy value AM 6:00 | 0.25 == 25.0% == 1/4

        // PM 9:00 | 0.875 == 87.5% == 7/8
        _nightStartTime = (int)_realTimePerDaySec * 7 / 8;

        _previousIsNight = _isNight;
    }

    private void UpdateTime()
    {
        _currentTime += Time.deltaTime * _timeSpeed;
        if (_currentTime >= _realTimePerDaySec) _currentTime = 0;
    }

    private void UpdateEnvironmentVisuals()
    {
        RotateSun();
        UpdateLighting();
        UpdateFogDensity();
    }

    private void RotateSun()
    {
        float sunRotation = Mathf.Lerp(-90, 270, _currentTime / _realTimePerDaySec);
        _sun.transform.rotation = Quaternion.Euler(new Vector3(sunRotation, _sunAngle, 0f));
    }

    private void UpdateLighting()
    {
        _sun.intensity = _sunIntensity.Evaluate(_currentTime / _realTimePerDaySec);

        // Animation Curve - Keyframe (x) : Value (y)
        // 0 ~ 0.25 : sunIntensity 0.8
        // 0.25 ~ 0.5 : sunIntensity 1
        // 0.5 ~ 0.8 : sunIntensity 0.8
        // 0.8 ~ 0.875 : sunIntensity 0
        // 0.875 ~ 0 : sunIntensity 0

        _sun.color = _sunColor.Evaluate(_currentTime / _realTimePerDaySec);

        if (_skyboxMaterial == null) return;
        _skyboxMaterial.SetFloat("_Exposure", _skyboxIntensity.Evaluate(_currentTime / _realTimePerDaySec));
    }

    private void UpdateFogDensity()
    {
        float nightTimeBlend = Mathf.Abs(_currentTime - _nightStartTime) / (_realTimePerDay / 4);
        float dayTimeBlend = Mathf.Abs(_currentTime - _dayStartTime) / (_realTimePerDay / 4);
        
        if (_isNight == true)
        {
            RenderSettings.fogColor = _nightColor;
            //Debug.Log($"{nightTimeBlend}");

            RenderSettings.fogDensity = Mathf.Lerp(0.0f, _fog, nightTimeBlend);
        }

        else if (_isNight == false)
            RenderSettings.fogDensity = Mathf.Lerp(_fog, 0.0f, dayTimeBlend);
    }
}