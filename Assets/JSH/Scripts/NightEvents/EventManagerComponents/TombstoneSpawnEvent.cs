using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TombstoneSpawnEvent : MonoBehaviour, IEvent
{
    [SerializeField] private GameObject tombstonePrefab;
    private int _minX, _maxX, _y, _minZ, _maxZ;

    [SerializeField] private int spawningTombstoneCount;


    private void Awake()
    {
        InitSpawnArea(-100, 100, 50, -100, 100);
    }



    public void ExecuteDayEvent()
    {
        Debug.Log("None");
    }
    public void ExecuteNightEvent()
    {
        TombstoneSpawn();
        Debug.Log("Spawn TombStone");
    }

    private void InitSpawnArea(int minX, int maxX, int y, int minZ, int maxZ)
    {
        _minX = minX;
        _maxX = maxX;
        _y = y;
        _minZ = minZ;
        _maxZ = maxZ;
    }

    private void TombstoneSpawn()
    {
        for(int i = 0; i < spawningTombstoneCount; i++)
            Instantiate(tombstonePrefab).transform.position = new Vector3(Random.Range(_minX, _maxX), _y, Random.Range(_minZ, _maxZ));
    }
}
