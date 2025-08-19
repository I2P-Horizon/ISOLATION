using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapScale
{
    [Tooltip("섬의 크기")] public int total = 250;
    [Tooltip("해변의 크기")] public float beach = 15f;
    [Tooltip("호수의 크기")] public float lake = 20f;
    [Tooltip("노이즈 크기")] public float noise = 5f;
    [Tooltip("블록 크기")] public Vector3 block = new Vector3(1, 1, 1);
}