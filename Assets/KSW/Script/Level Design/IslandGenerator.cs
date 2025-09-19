using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Object Data
[System.Serializable]
public class ObjectData
{
    [Header("오브젝트 설정")]
    public GameObject ObjectPrefab;
    [Range(0, 1)] public float spawnChance;

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
}
#endregion

#region Shape
[System.Serializable]
public class Shape
{
    [Header("Shape")]
    public float islandRadius = 200f;
    public float falloffPower = 0.15f;
    public float beachWidth = 3f;
}
#endregion

#region Height
[System.Serializable]
public class Height
{
    [Header("Heights")]
    public int maxHeight = 16;
    public int seaLevel = 4;
}
#endregion

#region Jungle
[System.Serializable]
public class Jungle
{
    [Header("Jungle Settings")]
    public int count = 20;
    public int minTreesPerJungle = 60;
    public int maxTreesPerJungle = 80;
    public float radius = 80f;
    public GameObject[] treePrefabs;
}
#endregion

#region Temple
[System.Serializable]
public class Temple
{
    [Header("Temple Settings")]
    public GameObject prefab;
    public float radius = 10f;
    public float scaleY = 5f;
    public float maxDistanceFromCenter = 50f;
}
#endregion

public class IslandGenerator : MonoBehaviour
{
    public ObjectData[] objectData;
    public BlockData blockData;
    public Grid grid;
    public Noise noise;
    public Shape shape;
    public Height height;
    public Jungle jungle;
    public Temple temple;

    public Transform island;
    public Transform player;

    private Dictionary<GameObject, Vector3> _scaleCache = new Dictionary<GameObject, Vector3>();
    private List<Vector3> sandPositions = new List<Vector3>();
    private List<Vector3> topGrassPositions = new List<Vector3>();

    private bool templeExists = false;
    private Vector3 templePos;

    [HideInInspector] public float generationProgress = 0f;

    private void Start()
    {
        if (noise.seed == 0) noise.seed = Random.Range(1, 100000);

        DetermineTemplePosition();
        StartCoroutine(GenerateIsland());
    }

    private void DetermineTemplePosition()
    {
        if (temple.prefab == null) return;

        for (int i = 0; i < 100; i++)
        {
            Vector2 offset = Random.insideUnitCircle * temple.maxDistanceFromCenter;
            int testX = Mathf.RoundToInt(offset.x);
            int testZ = Mathf.RoundToInt(offset.y);

            float dist = Mathf.Sqrt(testX * testX + testZ * testZ) / shape.islandRadius;
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
                templePos = new Vector3(testX, landHeight, testZ);
                templeExists = true;
                break;
            }
        }
    }

    private IEnumerator GenerateIsland()
    {
        if (island == null) island = new GameObject("Island").transform;

        Vector3 targetSize = Vector3.one;
        foreach (var p in new[] { blockData.grassBlock, blockData.dirtBlock, blockData.sandBlock, blockData.waterPlane, blockData.templeFloorBlock })
            if (p != null && !_scaleCache.ContainsKey(p)) _scaleCache[p] = GetScaleToFit(p, targetSize);

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
                chunkParent.SetParent(island);

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

                        float dist = Mathf.Sqrt(worldX * worldX + worldZ * worldZ) / shape.islandRadius;
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
                        float distanceToEdge = shape.islandRadius - dist * shape.islandRadius * islandMask;

                        if (distanceToEdge < shape.beachWidth)
                        {
                            float t = Mathf.Clamp01(distanceToEdge / shape.beachWidth);
                            sandLayers = (t > 0.5f) ? 2 : 1;
                        }

                        Vector3 posXZ = new Vector3(worldX, 0, worldZ);
                        bool inTempleArea = templeExists && Vector3.Distance(new Vector3(templePos.x, 0, templePos.z), posXZ) <= temple.radius;

                        if (islandMask > 0f && landHeight > height.seaLevel)
                        {
                            if (inTempleArea)
                            {
                                float floorY = Mathf.Max(templePos.y, temple.scaleY);
                                PlaceBlock(blockData.templeFloorBlock, new Vector3(worldX, floorY, worldZ), templeParent);
                                for (int y = height.seaLevel + 1; y < floorY; y++) PlaceBlock(blockData.dirtBlock, new Vector3(worldX, y, worldZ), templeParent);
                                continue;
                            }

                            if (sandLayers > 0)
                            {
                                for (int y = 0; y < sandLayers; y++)
                                {
                                    Vector3 pos = new Vector3(worldX, y, worldZ);
                                    if (pos.y > height.seaLevel)
                                    {
                                        PlaceBlock(blockData.sandBlock, pos, sandParent);
                                        sandPositions.Add(pos);
                                    }
                                }

                                if (landHeight > sandLayers)
                                {
                                    Vector3 grassPos = new Vector3(worldX, sandLayers, worldZ);
                                    PlaceBlock(blockData.grassBlock, grassPos, grassParent);
                                    topGrassPositions.Add(grassPos);
                                }
                            }

                            else
                            {
                                GameObject topBlock = (landHeight == height.seaLevel + 1) ? blockData.sandBlock : blockData.grassBlock;
                                Vector3 pos = new Vector3(worldX, landHeight, worldZ);
                                PlaceBlock(topBlock, pos, topBlock == blockData.grassBlock ? grassParent : topBlock ==  blockData.dirtBlock ? dirtParent : sandParent);

                                if (topBlock == blockData.grassBlock)
                                {
                                    topGrassPositions.Add(pos);

                                    if (templeExists)
                                    {
                                        float distFromTemple = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(templePos.x, 0, templePos.z));

                                        if (Mathf.Abs(distFromTemple - temple.radius) <= 1f)
                                        {
                                            for (int y = 1; y <= 4; y++)
                                            {
                                                Vector3 dirtPos = new Vector3(pos.x, pos.y - y, pos.z);
                                                PlaceBlock(blockData.dirtBlock, dirtPos, dirtParent);
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
                            PlaceBlock(blockData.dirtBlock, dirtUnderWaterPos, dirtParent);
                        }

                        PlaceBlock(blockData.waterPlane, new Vector3(worldX, waterY, worldZ), waterParent);

                        blocksGenerated++;
                        generationProgress = (float)blocksGenerated / totalBlocks;
                    }
                }

                CombineMesh combiner = GetComponent<CombineMesh>();
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

        if (templeExists && temple.prefab != null)
            Instantiate(temple.prefab, new Vector3(templePos.x, Mathf.Max(templePos.y, temple.scaleY), templePos.z), Quaternion.identity, island);

        SpawnPlayer();
        SpawnJungles();
        SpawnObject();

        /* 임시 */
        WorldMapMarker worldMapMarker = FindFirstObjectByType<WorldMapMarker>();
        while (worldMapMarker.isRendering) yield return null;
        SceneManager.UnloadSceneAsync("MainScene");
        yield return new WaitForSeconds(1f);
        Loading.Instance.loadingPanel.SetActive(false);
        Loading.Instance.isLoading = false;
    }

    private void PlaceBlock(GameObject prefab, Vector3 pos, Transform parent)
    {
        if (prefab == null) return;

        GameObject block = Instantiate(prefab, pos, Quaternion.identity, parent);

        if (_scaleCache.TryGetValue(prefab, out Vector3 s)) block.transform.localScale = s;
    }

    private Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
    {
        GameObject temp = Instantiate(prefab);
        Renderer rend = temp.GetComponentInChildren<Renderer>();

        if (rend == null) { DestroyImmediate(temp); return Vector3.one; }

        Vector3 originalSize = rend.bounds.size;
        DestroyImmediate(temp);

        if (originalSize == Vector3.zero) return Vector3.one;

        float yScale = originalSize.y < 0.01f ? 1f : targetSize.y / originalSize.y;

        return new Vector3(targetSize.x / originalSize.x, yScale, targetSize.z / originalSize.z);
    }

    private void SpawnPlayer()
    {
        if (player == null || sandPositions.Count == 0) return;
        Vector3 spawnPos = sandPositions[0];
        float minDist = float.MaxValue;
        foreach (var pos in sandPositions)
        {
            if (pos.y <= height.seaLevel) continue;

            float distFromCenter = new Vector2(pos.x, pos.z).magnitude;

            if (distFromCenter >= shape.islandRadius - shape.beachWidth * 2)
            {
                float diff = Mathf.Abs(distFromCenter - (shape.islandRadius - shape.beachWidth / 2));
                if (diff < minDist)
                {
                    minDist = diff;
                    spawnPos = pos;
                }
            }
        }

        player.position = spawnPos + Vector3.up * 1f;
    }

    private void SpawnJungles()
    {
        if (jungle.treePrefabs == null || jungle.treePrefabs.Length == 0) return;
        for (int i = 0; i < jungle.count; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(shape.islandRadius * 0.3f, shape.islandRadius * 0.8f);
            Vector3 center = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;

            RaycastHit hit;
            if (Physics.Raycast(center + Vector3.up * 100f, Vector3.down, out hit, 200f))
            {
                if (hit.point.y > height.seaLevel + 1f)
                {
                    if (templeExists && Vector3.Distance(new Vector3(hit.point.x, 0, hit.point.z), new Vector3(templePos.x, 0, templePos.z)) <= temple.radius)
                        continue;

                    SpawnJungleCluster(hit.point, island);
                }
            }
        }
    }

    private void SpawnJungleCluster(Vector3 centerPos, Transform parent)
    {
        Transform clusterParent = new GameObject("ForestCluster").transform;
        clusterParent.SetParent(parent);
        clusterParent.position = centerPos;

        GameObject selectedTreePrefab = jungle.treePrefabs[Random.Range(0, jungle.treePrefabs.Length)];
        int treeCount = Random.Range(jungle.minTreesPerJungle, jungle.maxTreesPerJungle + 1);

        for (int i = 0; i < treeCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * jungle.radius;
            Vector3 spawnPos = centerPos + new Vector3(offset2D.x, 0, offset2D.y);

            RaycastHit hit;

            if (Physics.Raycast(spawnPos + Vector3.up * 50f, Vector3.down, out hit, 100f))
            {
                string groundName = hit.collider.gameObject.name.ToLower();

                if (groundName.Contains("sand")) continue;

                if (hit.point.y > height.seaLevel + 1f)
                {
                    if (templeExists && Vector3.Distance(new Vector3(hit.point.x, 0, hit.point.z), new Vector3(templePos.x, 0, templePos.z)) <= temple.radius)
                        continue;

                    spawnPos.y = hit.point.y + 0.1f;
                    Instantiate(selectedTreePrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0), clusterParent);
                }
            }
        }
    }

    private void SpawnObject()
    {
        if (objectData == null || objectData.Length == 0) return;

        CombineMesh combiner = GetComponent<CombineMesh>();

        foreach (var obj in objectData)
        {
            if (obj.ObjectPrefab == null) continue;

            Transform objectParent = new GameObject(obj.ObjectPrefab.name + "_Objects").transform;
            objectParent.SetParent(island);

            Dictionary<string, Transform> chunkParents = new Dictionary<string, Transform>();

            foreach (var grassPos in topGrassPositions)
            {
                if (templeExists && Vector3.Distance(new Vector3(grassPos.x, 0, grassPos.z), new Vector3(templePos.x, 0, templePos.z)) <= temple.radius) continue;

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
                            Transform chunkObjParent = new GameObject($"Chunk_{chunkKey}_{obj.ObjectPrefab.name}").transform;
                            chunkObjParent.SetParent(objectParent);
                            chunkParents.Add(chunkKey, chunkObjParent);
                        }
                        parentForSpawn = chunkParents[chunkKey];
                    }

                    Instantiate(obj.ObjectPrefab, grassPos + Vector3.up * 1f, Quaternion.identity, parentForSpawn);
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
                            combiner.Combine(kvp.Value, obj.ObjectPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{obj.ObjectPrefab.name}_Merged_{kvp.Key}");
                        }
                    }
                }

                else
                {
                    /* 전체 병합 */
                    if (objectParent.childCount > 0)
                    {
                        combiner.Combine(objectParent, obj.ObjectPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{obj.ObjectPrefab.name}_Merged");
                    }
                }
            }
        }
    }
}