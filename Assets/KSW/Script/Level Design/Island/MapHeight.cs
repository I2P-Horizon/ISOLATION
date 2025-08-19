using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapHeight
{
    [Tooltip("�ִ� �ܵ� ����")] public int maxGrass = 4;
    [Tooltip("�ּ� �ܵ� ����")] public int minGrass = 1;
    [Tooltip("�ִ� ������ �Ը�")] public float noiseScale = 8f;
    [Header("��� ���� ���ؼ�")] public float sandHeight = 0f;
}