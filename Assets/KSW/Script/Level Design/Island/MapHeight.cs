using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapHeight
{
    [Tooltip("최대 잔디 높이")] public int maxGrass = 4;
    [Tooltip("최소 잔디 높이")] public int minGrass = 1;
    [Tooltip("최대 노이즈 규모")] public float noiseScale = 8f;
    [Header("블록 높이 기준선")] public float sandHeight = 0f;
}