using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CaveObject
{
    public GameObject objectPrefab;
    [Range(0f, 1f)] public float spawnChance = 0.1f;
}

public class CaveManager : MonoBehaviour
{
    public GameObject rockBlock;
    public Vector3 caveSize = new Vector3(50, 15, 50);
    public float blockSize = 1f;
    public float threshold = 0.5f;
    public float noiseScale = 0.1f;

    public List<CaveObject> objectPrefabs;

    private float offsetX;
    private float offsetY;
    private float offsetZ;

    private void Awake()
    {
        offsetX = Random.Range(0f, 100f);
        offsetY = Random.Range(0f, 100f);
        offsetZ = Random.Range(0f, 100f);

        BuildCave();
    }

    private void Start()
    {
        SpawnExtrasOnTop();
    }

    Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
    {
        GameObject temp = Instantiate(prefab);
        Renderer rend = temp.GetComponentInChildren<Renderer>();

        if (rend == null)
        {
            DestroyImmediate(temp);
            return Vector3.one;
        }

        Vector3 originalSize = rend.bounds.size;
        DestroyImmediate(temp);

        if (originalSize == Vector3.zero) return Vector3.one;

        float yScale = originalSize.y < 0.01f ? 1f : targetSize.y / originalSize.y;

        return new Vector3(targetSize.x / originalSize.x, yScale, targetSize.z / originalSize.z);
    }

    public void BuildCave()
    {
        if (rockBlock == null) return;

        ClearCave();

        int width = Mathf.RoundToInt(caveSize.x / blockSize);
        int height = Mathf.RoundToInt(caveSize.y / blockSize);
        int depth = Mathf.RoundToInt(caveSize.z / blockSize);

        Vector3 startPos = transform.position - new Vector3(caveSize.x / 2f, 0, caveSize.z / 2f);
        Vector3 rockScale = GetScaleToFit(rockBlock, new Vector3(blockSize, blockSize, blockSize));

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    float noiseValue = PerlinNoise3D(
                        (x + offsetX) * noiseScale,
                        (y + offsetY) * noiseScale,
                        (z + offsetZ) * noiseScale);

                    bool isFloor = (y == 0);
                    bool isCeiling = (y == height - 1);
                    bool isWall = (x == 0 || x == width - 1 || z == 0 || z == depth - 1);

                    if (noiseValue > threshold || isFloor || isCeiling || isWall)
                    {
                        Vector3 pos = startPos + new Vector3(x * blockSize + blockSize / 2f, y * blockSize + blockSize / 2f, z * blockSize + blockSize / 2f);
                        GameObject block = Instantiate(rockBlock, pos, Quaternion.identity, transform);
                        block.transform.localScale = rockScale;
                    }
                }
            }
        }

        GetComponent<CombineMesh>().Combine(gameObject.transform, rockBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, "Rock Block");
    }

    public void SpawnExtrasOnTop()
    {
        if (objectPrefabs == null || objectPrefabs.Count == 0) return;

        int width = Mathf.RoundToInt(caveSize.x / blockSize);
        int depth = Mathf.RoundToInt(caveSize.z / blockSize);

        Vector3 startPos = transform.position - new Vector3(caveSize.x / 2f, 0, caveSize.z / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                int y = 1;

                foreach (var caveObj in objectPrefabs)
                {
                    if (caveObj.objectPrefab == null) continue;

                    if (Random.Range(0f, 1f) < caveObj.spawnChance)
                    {
                        Vector3 extraScale = GetScaleToFit(caveObj.objectPrefab, new Vector3(blockSize, blockSize, blockSize));
                        Vector3 pos = startPos + new Vector3(x * blockSize + blockSize / 2f, 1.9f, z * blockSize + blockSize / 2f);
                        GameObject extraObj = Instantiate(caveObj.objectPrefab, pos, Quaternion.identity, transform);
                        extraObj.transform.localScale = extraScale;
                    }
                }
            }
        }
    }

    public void ClearCave()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }
}