using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IslandData
{
    [Header("Shape")]
    [SerializeField] protected float radius = 200f;

    [Header("Terrain")]
    protected float falloffPower = 0.15f;
    protected float beachWidth = 3f;

    [Header("Heights")]
    protected int maxHeight = 32;
    protected int seaLevel = 8;

    [Header("Grid")]
    protected int width = 800;
    protected int height = 800;
    protected int chunkSize = 32;

    [Header("Noise")]
    protected float scale = 80f;
    protected float persistence = 0.5f;
    protected float lacunarity = 2f;
    protected int octaves = 4;
    protected int seed = 0;   
}

public abstract class Island : IslandData
{
    protected virtual IEnumerator Generate() { yield return null; }
    public virtual void Seed() { if (seed == 0) seed = Random.Range(1, 100000); }
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

    public virtual void PlaceBlock(GameObject prefab, Vector3 pos, Transform parent)
    {
        if (prefab == null) return;
        GameObject block = MonoBehaviour.Instantiate(prefab, pos, Quaternion.identity, parent);
        if (scaleCache.TryGetValue(prefab, out Vector3 s)) block.transform.localScale = s;
    }

    public virtual Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
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