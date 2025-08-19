using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UniquePlacementType
{
    CenterArea,
    EdgeArea,
    Custom
}

[System.Serializable]
public class MapObject
{
    [Tooltip("맵 오브젝트")] public GameObject mapObject;
    [Tooltip("스폰 확률")][Range(0, 1)] public float spawnChance;
    [Tooltip("유니크 오브젝트")] public bool isUnique;
    [Tooltip("배치 타입")] public UniquePlacementType placementType;
}