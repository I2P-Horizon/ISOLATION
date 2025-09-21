using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Island
[System.Serializable]
public class Island
{
    private Height height;
    private Shape shape;
    private Grid grid;
    private Noise noise;
    private Jungle jungle;
    private Temple temple;
    private BlockData blockData;
    private ObjectData[] objectData;

    public Transform pos;
    public Transform player;

    [HideInInspector]
    public List<Vector3> sandPositions = new List<Vector3>();
    public List<Vector3> topGrassPositions = new List<Vector3>();

    public void Set(Height height, Shape shape, Grid grid, Noise noise, Jungle jungle, Temple temple, BlockData blockData, ObjectData[] objectData)
    {
        this.height = height;
        this.shape = shape;
        this.grid = grid;
        this.noise = noise;
        this.jungle = jungle;
        this.temple = temple;
        this.blockData = blockData;
        this.objectData = objectData;
    }

    public IEnumerator Spawn()
    {
        Vector3 targetSize = Vector3.one;
        foreach (var p in new[] { blockData.grassBlock, blockData.dirtBlock, blockData.sandBlock, blockData.waterPlane, blockData.templeFloorBlock })
            if (p != null && !blockData.scaleCache.ContainsKey(p)) blockData.scaleCache[p] = blockData.GetScaleToFit(p, targetSize);

        float halfW = grid.width / 2f;
        float halfH = grid.height / 2f;

        int chunkXCount = Mathf.CeilToInt((float)grid.width / grid.chunkSize);
        int chunkZCount = Mathf.CeilToInt((float)grid.height / grid.chunkSize);

        int totalBlocks = chunkXCount * chunkZCount * grid.chunkSize * grid.chunkSize;
        int blocksGenerated = 0;

        float waterY = height.seaLevel + 1.5f;
        sandPositions.Clear();
        topGrassPositions.Clear();

        for (int cx = 0; cx < chunkXCount; cx++)
        {
            for (int cz = 0; cz < chunkZCount; cz++)
            {
                Transform chunkParent = new GameObject($"Chunk_{cx}_{cz}").transform;
                chunkParent.SetParent(pos);

                Transform grassParent = new GameObject("Grass").transform;
                Transform dirtParent = new GameObject("Dirt").transform;
                Transform sandParent = new GameObject("Sand").transform;
                Transform waterParent = new GameObject("Water").transform;
                Transform templeParent = new GameObject("Temple").transform;

                grassParent.SetParent(chunkParent);
                dirtParent.SetParent(chunkParent);
                sandParent.SetParent(chunkParent);
                waterParent.SetParent(chunkParent);
                templeParent.SetParent(chunkParent);

                for (int x = 0; x < grid.chunkSize; x++)
                {
                    for (int z = 0; z < grid.chunkSize; z++)
                    {
                        int worldX = cx * grid.chunkSize + x - (int)halfW;
                        int worldZ = cz * grid.chunkSize + z - (int)halfH;

                        float dist = Mathf.Sqrt(worldX * worldX + worldZ * worldZ) / shape.radius;
                        float noiseMask = Mathf.PerlinNoise((worldX + noise.seed) / noise.scale, (worldZ + noise.seed) / noise.scale);
                        float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.45f);
                        islandMask = Mathf.Pow(islandMask, shape.falloffPower);

                        float heightNoise = 0f;
                        float amplitude = 1f;
                        float frequency = 1f;
                        float maxAmp = 0f;

                        for (int o = 0; o < noise.octaves; o++)
                        {
                            float sampleX = (worldX + noise.seed) / noise.scale * frequency;
                            float sampleZ = (worldZ + noise.seed) / noise.scale * frequency;
                            heightNoise += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                            maxAmp += amplitude;
                            amplitude *= noise.persistence;
                            frequency *= noise.lacunarity;
                        }

                        if (maxAmp > 0f) heightNoise /= maxAmp;

                        int landHeight = Mathf.RoundToInt(heightNoise * islandMask * height.maxHeight);

                        int sandLayers = 0;
                        float distanceToEdge = shape.radius - dist * shape.radius * islandMask;

                        if (distanceToEdge < shape.beachWidth)
                        {
                            float t = Mathf.Clamp01(distanceToEdge / shape.beachWidth);
                            sandLayers = (t > 0.5f) ? 2 : 1;
                        }

                        Vector3 posXZ = new Vector3(worldX, 0, worldZ);
                        bool inTempleArea = temple.exists && Vector3.Distance(new Vector3(temple.pos.x, 0, temple.pos.z), posXZ) <= temple.radius;

                        if (islandMask > 0f && landHeight > height.seaLevel)
                        {
                            if (inTempleArea)
                            {
                                float floorY = Mathf.Max(temple.pos.y, temple.scaleY);
                                blockData.PlaceBlock(blockData.templeFloorBlock, new Vector3(worldX, floorY, worldZ), templeParent);
                                for (int y = height.seaLevel + 1; y < floorY; y++) blockData.PlaceBlock(blockData.dirtBlock, new Vector3(worldX, y, worldZ), templeParent);
                                continue;
                            }

                            if (sandLayers > 0)
                            {
                                for (int y = 0; y < sandLayers; y++)
                                {
                                    Vector3 pos = new Vector3(worldX, y, worldZ);
                                    if (pos.y > height.seaLevel)
                                    {
                                        blockData.PlaceBlock(blockData.sandBlock, pos, sandParent);
                                        sandPositions.Add(pos);
                                    }
                                }

                                if (landHeight > sandLayers)
                                {
                                    Vector3 grassPos = new Vector3(worldX, sandLayers, worldZ);
                                    blockData.PlaceBlock(blockData.grassBlock, grassPos, grassParent);
                                    topGrassPositions.Add(grassPos);
                                }
                            }

                            else
                            {
                                GameObject topBlock = (landHeight == height.seaLevel + 1) ? blockData.sandBlock : blockData.grassBlock;
                                Vector3 pos = new Vector3(worldX, landHeight, worldZ);
                                blockData.PlaceBlock(topBlock, pos, topBlock == blockData.grassBlock ? grassParent : topBlock == blockData.dirtBlock ? dirtParent : sandParent);

                                if (topBlock == blockData.grassBlock)
                                {
                                    topGrassPositions.Add(pos);

                                    if (temple.exists)
                                    {
                                        float distFromTemple = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(temple.pos.x, 0, temple.pos.z));

                                        if (Mathf.Abs(distFromTemple - temple.radius) <= 1f)
                                        {
                                            for (int y = 1; y <= 4; y++)
                                            {
                                                Vector3 dirtPos = new Vector3(pos.x, pos.y - y, pos.z);
                                                blockData.PlaceBlock(blockData.dirtBlock, dirtPos, dirtParent);
                                            }
                                        }
                                    }
                                }

                                if (topBlock == blockData.sandBlock && pos.y > height.seaLevel) sandPositions.Add(pos);
                            }
                        }

                        if (islandMask > 0f && landHeight <= height.seaLevel)
                        {
                            Vector3 dirtUnderWaterPos = new Vector3(worldX, waterY - 1.5f, worldZ);
                            blockData.PlaceBlock(blockData.dirtBlock, dirtUnderWaterPos, dirtParent);
                        }

                        blockData.PlaceBlock(blockData.waterPlane, new Vector3(worldX, waterY, worldZ), waterParent);

                        blocksGenerated++;
                        IslandGenerator.generationProgress = (float)blocksGenerated / totalBlocks;
                    }
                }

                CombineMesh combiner = MonoBehaviour.FindAnyObjectByType<CombineMesh>();
                if (combiner != null)
                {
                    if (grassParent.childCount > 0)
                        combiner.Combine(grassParent, blockData.grassBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Grass");
                    if (dirtParent.childCount > 0)
                        combiner.Combine(dirtParent, blockData.dirtBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Dirt");
                    if (sandParent.childCount > 0)
                        combiner.Combine(sandParent, blockData.sandBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Sand");
                    if (waterParent.childCount > 0)
                        combiner.Combine(waterParent, blockData.waterPlane.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Water");
                    if (templeParent.childCount > 0)
                        combiner.Combine(templeParent, blockData.templeFloorBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Temple");
                }

                yield return null;
            }
        }

        if (temple.exists && temple.prefab != null)
            MonoBehaviour.Instantiate(temple.prefab, new Vector3(temple.pos.x, Mathf.Max(temple.pos.y, temple.scaleY), temple.pos.z), Quaternion.identity, pos);

        jungle.Spawn();
        SpawnPlayer();
        SpawnObject();

        /* 임시 */
        WorldMapMarker worldMapMarker = MonoBehaviour.FindFirstObjectByType<WorldMapMarker>();
        while (worldMapMarker.isRendering) yield return null;
        SceneManager.UnloadSceneAsync("MainScene");
        yield return new WaitForSeconds(1f);
        Loading.Instance.loadingPanel.SetActive(false);
        Loading.Instance.isLoading = false;
    }

    private void SpawnObject()
    {
        if (objectData == null || objectData.Length == 0) return;

        CombineMesh combiner = MonoBehaviour.FindAnyObjectByType<CombineMesh>();

        foreach (var obj in objectData)
        {
            if (obj.prefab == null) continue;

            Transform objectParent = new GameObject(obj.prefab.name + "_Objects").transform;
            objectParent.SetParent(pos);

            Dictionary<string, Transform> chunkParents = new Dictionary<string, Transform>();

            foreach (var grassPos in topGrassPositions)
            {
                if (temple.exists && Vector3.Distance(new Vector3(grassPos.x, 0, grassPos.z), new Vector3(temple.pos.x, 0, temple.pos.z)) <= temple.radius) continue;

                if (Random.value < obj.spawnChance)
                {
                    Transform parentForSpawn = objectParent;

                    if (obj.chunkSeparate)
                    {
                        int chunkX = Mathf.FloorToInt((grassPos.x + grid.width / 2f) / grid.chunkSize);
                        int chunkZ = Mathf.FloorToInt((grassPos.z + grid.height / 2f) / grid.chunkSize);
                        string chunkKey = $"{chunkX}_{chunkZ}";

                        if (!chunkParents.ContainsKey(chunkKey))
                        {
                            Transform chunkObjParent = new GameObject($"Chunk_{chunkKey}_{obj.prefab.name}").transform;
                            chunkObjParent.SetParent(objectParent);
                            chunkParents.Add(chunkKey, chunkObjParent);
                        }
                        parentForSpawn = chunkParents[chunkKey];
                    }

                    MonoBehaviour.Instantiate(obj.prefab, grassPos + Vector3.up * 1f, Quaternion.identity, parentForSpawn);
                }
            }

            /* 병합 설정 처리 */
            if (obj.mergeMeshes && combiner != null)
            {
                if (obj.chunkSeparate)
                {
                    /* 청크 단위 병합 */
                    foreach (var kvp in chunkParents)
                    {
                        if (kvp.Value.childCount > 0)
                        {
                            combiner.Combine(kvp.Value, obj.prefab.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{obj.prefab.name}_Merged_{kvp.Key}");
                        }
                    }
                }

                else
                {
                    /* 전체 병합 */
                    if (objectParent.childCount > 0)
                    {
                        combiner.Combine(objectParent, obj.prefab.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{obj.prefab.name}_Merged");
                    }
                }
            }
        }
    }

    public void SpawnPlayer()
    {
        if (player == null || sandPositions.Count == 0) return;
        Vector3 spawnPos = sandPositions[0];
        float minDist = float.MaxValue;
        foreach (var pos in sandPositions)
        {
            if (pos.y <= height.seaLevel) continue;

            float distFromCenter = new Vector2(pos.x, pos.z).magnitude;

            if (distFromCenter >= shape.radius - shape.beachWidth * 2)
            {
                float diff = Mathf.Abs(distFromCenter - (shape.radius - shape.beachWidth / 2));
                if (diff < minDist)
                {
                    minDist = diff;
                    spawnPos = pos;
                }
            }
        }

        player.position = spawnPos + Vector3.up * 1f;
    }
}
#endregion

#region Height
[System.Serializable]
public class Height
{
    [Header("Heights")]
    public int maxHeight = 32;
    public int seaLevel = 8;
}
#endregion

#region Shape
[System.Serializable]
public class Shape
{
    [Header("Shape")]
    public float radius = 200f;
    public float falloffPower = 0.15f;
    public float beachWidth = 3f;
}
#endregion

#region Object Data
[System.Serializable]
public class ObjectData
{
    [Header("오브젝트 설정")]
    public GameObject prefab;

    [Range(0, 1)] [Tooltip("스폰 확률")]
    public float spawnChance;

    [Header("메쉬 병합 설정")]
    [Tooltip("메쉬 병합 여부")] public bool mergeMeshes;
    [Tooltip("청크 단위 병합 여부")] public bool chunkSeparate;
}
#endregion

#region BlockData
[System.Serializable]
public class BlockData
{
    [Header("Block Prefabs")]
    public GameObject grassBlock;
    public GameObject dirtBlock;
    public GameObject sandBlock;
    public GameObject waterPlane;
    public GameObject templeFloorBlock;

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
#endregion

#region Grid
[System.Serializable]
public class Grid
{
    [Header("Grid")]
    public int width = 800;
    public int height = 800;
    public int chunkSize = 32;
}
#endregion

#region Noise
[System.Serializable]
public class Noise
{
    [Header("Noise")]
    public float scale = 80f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public int seed = 0;

    public void Seed()
    {
        if (seed == 0) seed = Random.Range(1, 100000);
    }
}
#endregion

#region Jungle
[System.Serializable]
public class Jungle
{
    private Island island;
    private Height height;
    private Shape shape;
    private Temple temple;

    [Header("Jungle Settings")]
    public int count = 20;
    public int minTreesPerJungle = 60;
    public int maxTreesPerJungle = 80;
    public float jungleRadius = 80f;
    public GameObject[] treePrefabs;

    public void Set(Island island, Height height, Shape shape, Temple temple)
    {
        this.island = island;
        this.height = height;
        this.shape = shape;
        this.temple = temple;
    }

    public void Spawn()
    {
        if (treePrefabs == null || treePrefabs.Length == 0) return;
        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(shape.radius * 0.3f, shape.radius * 0.8f);
            Vector3 center = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;

            RaycastHit hit;
            if (Physics.Raycast(center + Vector3.up * 100f, Vector3.down, out hit, 200f))
            {
                if (hit.point.y > height.seaLevel + 1f)
                {
                    if (temple.exists && Vector3.Distance(new Vector3(hit.point.x, 0, hit.point.z), new Vector3(temple.pos.x, 0, temple.pos.z)) <= temple.radius)
                        continue;

                    SpawnJungleCluster(hit.point, island.pos);
                }
            }
        }
    }

    private void SpawnJungleCluster(Vector3 centerPos, Transform parent)
    {
        Transform clusterParent = new GameObject("ForestCluster").transform;
        clusterParent.SetParent(parent);
        clusterParent.position = centerPos;

        GameObject selectedTreePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
        int treeCount = Random.Range(minTreesPerJungle, maxTreesPerJungle + 1);

        for (int i = 0; i < treeCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * jungleRadius;
            Vector3 spawnPos = centerPos + new Vector3(offset2D.x, 0, offset2D.y);

            RaycastHit hit;

            if (Physics.Raycast(spawnPos + Vector3.up * 50f, Vector3.down, out hit, 100f))
            {
                string groundName = hit.collider.gameObject.name.ToLower();

                if (groundName.Contains("sand")) continue;

                if (hit.point.y > height.seaLevel + 1f)
                {
                    if (temple.exists && Vector3.Distance(new Vector3(hit.point.x, 0, hit.point.z), new Vector3(temple.pos.x, 0, temple.pos.z)) <= temple.radius)
                        continue;

                    spawnPos.y = hit.point.y + 0.1f;
                    MonoBehaviour.Instantiate(selectedTreePrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0), clusterParent);
                }
            }
        }
    }
}
#endregion

#region Temple
[System.Serializable]
public class Temple
{
    private Height height;
    private Shape shape;
    private Noise noise;

    [Header("Temple Settings")]
    public GameObject prefab;
    public float radius = 10f;
    public float scaleY = 5f;
    public float maxDistanceFromCenter = 50f;

    [HideInInspector]
    public Vector3 pos;
    public bool exists = false;

    public void Set(Height height, Shape shape, Noise noise)
    {
        this.height = height;
        this.shape = shape;
        this.noise = noise;
    }

    public void Placement()
    {
        if (prefab == null) return;

        for (int i = 0; i < 100; i++)
        {
            Vector2 offset = Random.insideUnitCircle * maxDistanceFromCenter;
            int testX = Mathf.RoundToInt(offset.x);
            int testZ = Mathf.RoundToInt(offset.y);

            float dist = Mathf.Sqrt(testX * testX + testZ * testZ) / shape.radius;
            float noiseMask = Mathf.PerlinNoise((testX + noise.seed) / noise.scale, (testZ + noise.seed) / noise.scale);
            float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.45f);
            islandMask = Mathf.Pow(islandMask, shape.falloffPower);
            islandMask = Mathf.Max(islandMask, 0.05f);

            float heightNoise = 0f;
            float amplitude = 1f;
            float frequency = 1f;
            float maxAmp = 0f;

            for (int o = 0; o < noise.octaves; o++)
            {
                float sampleX = (testX + noise.seed) / noise.scale * frequency;
                float sampleZ = (testZ + noise.seed) / noise.scale * frequency;
                heightNoise += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                maxAmp += amplitude;
                amplitude *= noise.persistence;
                frequency *= noise.lacunarity;
            }

            if (maxAmp > 0f) heightNoise /= maxAmp;

            int landHeight = Mathf.RoundToInt(heightNoise * islandMask * height.maxHeight);

            if (landHeight <= height.seaLevel) landHeight = height.seaLevel + 1;

            if (islandMask > 0f && landHeight > height.seaLevel + 1)
            {
                pos = new Vector3(testX, landHeight, testZ);
                exists = true;
                break;
            }
        }
    }
}
#endregion

public class IslandGenerator : MonoBehaviour
{
    public Island island;
    public Height height;
    public Shape shape;
    public Temple temple;
    public Jungle jungle;
    public Grid grid;
    public Noise noise;

    public BlockData blockData;
    public ObjectData[] objectData;

    public static float generationProgress = 0f;

    private void Start()
    {
        /* Setter */
        island.Set(height, shape, grid, noise, jungle, temple, blockData, objectData);
        jungle.Set(island, height, shape, temple);
        temple.Set(height, shape, noise);

        /* 시드 생성 후 사원 위치 결정 */
        noise.Seed();
        temple.Placement();

        /* 섬 생성 */
        StartCoroutine(island.Spawn());
    }
}