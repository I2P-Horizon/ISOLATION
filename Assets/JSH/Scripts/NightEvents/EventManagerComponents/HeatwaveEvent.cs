using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HeatwaveEvent : MonoBehaviour, IEvent
{
    // Global volume profile
    [SerializeField] VolumeProfile globalVolumeProfile;

    public void ExecuteDayEvent()
    {
        globalVolumeProfile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = 0.0f;
        color.contrast.value = 0.0f;
        color.hueShift.value = 0.0f;
        color.saturation.value = 0.0f;
    }

    public void ExecuteNightEvent()
    {
        globalVolumeProfile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = 3.0f;
        color.contrast.value = -50.0f;
        color.hueShift.value = -30.0f;
        color.saturation.value = -20.0f;
    }
}