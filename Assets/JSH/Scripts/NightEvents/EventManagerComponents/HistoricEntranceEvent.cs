using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoricEntranceEvent : MonoBehaviour, IEvent
{
    [SerializeField] private GameObject HistoricEntrancePrefab;
    private int _minX, _maxX, _y, _minZ, _maxZ;
    private GameObject _instance;

    private void Awake()
    {
        InitSpawnArea(-30, 30, 50, -30, 30);
    }



    public void ExecuteDayEvent()
    {
        if (!_instance) return;
        Destroy(_instance);
        Debug.Log("Historic entrance eliimated");
    }
    public void ExecuteNightEvent()
    {
        SpawnHeavySnow();
        Debug.Log("Historic entrance spawned");
    }

    private void InitSpawnArea(int minX, int maxX, int y, int minZ, int maxZ)
    {
        _minX = minX;
        _maxX = maxX;
        _y = y;
        _minZ = minZ;
        _maxZ = maxZ;
    }

    private void SpawnHeavySnow()
    {
        _instance = Instantiate(HistoricEntrancePrefab);
        _instance.transform.position = new Vector3(Random.Range(_minX, _maxX), _y, Random.Range(_minZ, _maxZ));
    }
}
