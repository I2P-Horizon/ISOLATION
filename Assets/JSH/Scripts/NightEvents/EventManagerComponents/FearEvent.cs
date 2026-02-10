using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FearEvent : MonoBehaviour, IEvent
{
    // Global volume profile
    [SerializeField] VolumeProfile globalVolumeProfile;
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] private int _spawningGhostCount;
    private Transform _ghostInstancesContainer;

    // Life cycle functions //
    private void Awake()
    {
        GameObject container = new GameObject("GhostInstancesParent");
        _ghostInstancesContainer = container.transform;
    }
    private void Start()
    {
        InitSpawnArea(-100, 100, 3, 20, -100, 100);
    }

    // Interface implementation functions //
    public void ExecuteDayEvent()
    {
        globalVolumeProfile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = 0.0f;
        color.contrast.value = 0.0f;
        color.hueShift.value = 0.0f;
        color.saturation.value = 0.0f;

        DestoryGhostInstances();
    }

    public void ExecuteNightEvent()
    {
        globalVolumeProfile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = 3.0f;
        color.contrast.value = 40.0f;
        color.hueShift.value = 10.0f;
        color.saturation.value = -50.0f;

        SpawnGhost();
    }

    // Original functions //
    private void SpawnGhost()
    {
        for (int i = 0; i < _spawningGhostCount; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY), Random.Range(_minZ, _maxZ));
            Instantiate(ghostPrefab, spawnPos, Quaternion.identity, _ghostInstancesContainer);
        }
    }
    private void DestoryGhostInstances()
    {
        if (_ghostInstancesContainer == null) return;

        for(int i = _ghostInstancesContainer.childCount - 1; i >= 0 ; i--)
        {
            Destroy(_ghostInstancesContainer.GetChild(i).gameObject);
        }
    }

    private int _minX, _maxX, _minY, _maxY, _minZ, _maxZ;
    private void InitSpawnArea(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    {
        _minX = minX;
        _maxX = maxX;
        _minY = minY;
        _maxY = maxY;
        _minZ = minZ;
        _maxZ = maxZ;
    }
}