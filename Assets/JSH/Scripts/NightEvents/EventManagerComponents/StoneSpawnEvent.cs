using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSpawnEvent : MonoBehaviour, IEvent
{
    [SerializeField] private GameObject stonePrefab;
    private int _minX, _maxX, _y, _minZ, _maxZ;

    [SerializeField] private int spawningStoneCount;


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
        StoneSpawn();
        Debug.Log("Spawn Stone");
    }

    private void InitSpawnArea(int minX, int maxX, int y, int minZ, int maxZ)
    {
        _minX = minX;
        _maxX = maxX;
        _y = y;
        _minZ = minZ;
        _maxZ = maxZ;
    }

    private void StoneSpawn()
    {
        for (int i = 0; i < spawningStoneCount; i++)
            Instantiate(stonePrefab).transform.position = new Vector3(Random.Range(_minX, _maxX), _y, Random.Range(_minZ, _maxZ));
    }
}
