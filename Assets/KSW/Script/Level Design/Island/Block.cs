using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block
{
    /// <summary>
    /// ��� ������ ���� ũ�⿡ ���� ������ ����
    /// </summary>
    public Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
    {
        GameObject temp = MonoBehaviour.Instantiate(prefab);
        Renderer rend = temp.GetComponentInChildren<Renderer>();

        if (rend == null)
        {
            MonoBehaviour.DestroyImmediate(temp);
            return Vector3.one;
        }

        Vector3 originalSize = rend.bounds.size;
        MonoBehaviour.DestroyImmediate(temp);

        if (originalSize == Vector3.zero) return Vector3.one;

        float yScale = originalSize.y < 0.01f ? 1f : targetSize.y / originalSize.y;

        return new Vector3(targetSize.x / originalSize.x, yScale, targetSize.z / originalSize.z);
    }
}