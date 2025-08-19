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
    [Tooltip("�� ������Ʈ")] public GameObject mapObject;
    [Tooltip("���� Ȯ��")][Range(0, 1)] public float spawnChance;
    [Tooltip("����ũ ������Ʈ")] public bool isUnique;
    [Tooltip("��ġ Ÿ��")] public UniquePlacementType placementType;
}