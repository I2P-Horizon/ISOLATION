using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapScale
{
    [Tooltip("���� ũ��")] public int total = 250;
    [Tooltip("�غ��� ũ��")] public float beach = 15f;
    [Tooltip("ȣ���� ũ��")] public float lake = 20f;
    [Tooltip("������ ũ��")] public float noise = 5f;
    [Tooltip("��� ũ��")] public Vector3 block = new Vector3(1, 1, 1);
}