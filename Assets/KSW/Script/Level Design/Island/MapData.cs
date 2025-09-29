using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Height
{
    [Header("Heights")]
    public int maxHeight = 32;
    public int seaLevel = 8;
}

public class Shape
{
    [Header("Shape")]
    public float radius = 200f;
    public float falloffPower = 0.15f;
    public float beachWidth = 3f;
}

[System.Serializable]
public class Grid
{
    [Header("Grid")]
    public int width = 800;
    public int height = 800;
    public int chunkSize = 32;
}

[System.Serializable]
public class Noise
{
    [Header("Noise")]
    public float scale = 80f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public int seed = 0;

    public void Seed() { if (seed == 0) seed = Random.Range(1, 100000); }
}

[System.Serializable]
public class ObjectData
{
    [Header("오브젝트 설정")]
    public GameObject prefab;

    [Range(0, 1)]
    [Tooltip("스폰 확률")]
    public float spawnChance = 0.05f;

    [Header("메쉬 병합 설정")]
    [Tooltip("메쉬 병합 여부")] public bool mergeMeshes = true;
    [Tooltip("청크 단위 병합 여부")] public bool chunkSeparate = true;
}

[System.Serializable]
public class BlockData
{
    [Header("Block Prefabs")]
    public GameObject grassBlock;
    public GameObject dirtBlock;
    public GameObject sandBlock;
    public GameObject waterPlane;
    public GameObject templeFloorBlock;
    public GameObject swampBlock;
    public GameObject rockBlock;

    [HideInInspector]
    public Dictionary<GameObject, Vector3> scaleCache = new Dictionary<GameObject, Vector3>();

    public void PlaceBlock(GameObject prefab, Vector3 pos, Transform parent)
    {
        if (prefab == null) return;
        GameObject block = MonoBehaviour.Instantiate(prefab, pos, Quaternion.identity, parent);
        if (scaleCache.TryGetValue(prefab, out Vector3 s)) block.transform.localScale = s;
    }

    public Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
    {
        GameObject temp = MonoBehaviour.Instantiate(prefab);
        Renderer rend = temp.GetComponentInChildren<Renderer>();

        if (rend == null) { MonoBehaviour.DestroyImmediate(temp); return Vector3.one; }

        Vector3 originalSize = rend.bounds.size;
        MonoBehaviour.DestroyImmediate(temp);

        if (originalSize == Vector3.zero) return Vector3.one;

        float yScale = originalSize.y < 0.01f ? 1f : targetSize.y / originalSize.y;
        return new Vector3(targetSize.x / originalSize.x, yScale, targetSize.z / originalSize.z);
    }
}