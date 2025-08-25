using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandGenerator : MonoBehaviour
{
    [Header("Parent Object")]
    public Transform islandParent;

    [Header("Grid (Sea Area)")]
    public int width = 800;
    public int height = 800;
    public int chunkSize = 64;

    [Header("Noise")]
    public float noiseScale = 30f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public int seed = 0;

    [Header("Shape")]
    public float islandRadius = 200f;
    public float falloffPower = 0.5f;
    public float beachWidth = 5f;

    [Header("Heights")]
    public int maxHeight = 16;
    public int seaLevel = 4;

    [Header("Prefabs")]
    public GameObject grassBlock;
    public GameObject dirtBlock;
    public GameObject sandBlock;
    public GameObject waterPlane;
    public GameObject templePrefab;
    public GameObject templeFloorBlock;

    [Header("Player")]
    public Transform player;

    [Header("Forest Settings")]
    public GameObject[] treePrefabs;
    public int forestCount = 5;
    public int minTreesPerForest = 20;
    public int maxTreesPerForest = 50;
    public float forestRadius = 15f;

    [Header("Pineapple Settings")]
    public GameObject pineapplePrefab;
    [Range(0f, 1f)]
    public float pineappleSpawnChance = 0.2f;

    [Header("Temple Settings")]
    public float templeRadius = 10f;
    public float templeY = 5f;
    public float templeMaxDistanceFromCenter = 50f;

    private Dictionary<GameObject, Vector3> _scaleCache = new Dictionary<GameObject, Vector3>();
    private List<Vector3> sandPositions = new List<Vector3>();
    private List<Vector3> topGrassPositions = new List<Vector3>();

    private bool templeExists = false;
    private Vector3 templePos;

    [HideInInspector] public float generationProgress = 0f;

    void Start()
    {
        if (seed == 0) seed = Random.Range(1, 100000);

        DetermineTemplePosition();
        StartCoroutine(GenerateIsland());
    }

    void DetermineTemplePosition()
    {
        if (templePrefab == null) return;

        for (int i = 0; i < 100; i++)
        {
            Vector2 offset = Random.insideUnitCircle * templeMaxDistanceFromCenter;
            int testX = Mathf.RoundToInt(offset.x);
            int testZ = Mathf.RoundToInt(offset.y);

            float dist = Mathf.Sqrt(testX * testX + testZ * testZ) / islandRadius;
            float noiseMask = Mathf.PerlinNoise((testX + seed) / noiseScale, (testZ + seed) / noiseScale);
            float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.5f);
            islandMask = Mathf.Pow(islandMask, falloffPower);

            float heightNoise = 0f;
            float amplitude = 1f;
            float frequency = 1f;
            float maxAmp = 0f;
            for (int o = 0; o < octaves; o++)
            {
                float sampleX = (testX + seed) / noiseScale * frequency;
                float sampleZ = (testZ + seed) / noiseScale * frequency;
                heightNoise += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                maxAmp += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }
            if (maxAmp > 0f) heightNoise /= maxAmp;

            int landHeight = Mathf.RoundToInt(heightNoise * islandMask * maxHeight);

            if (islandMask > 0f && landHeight > seaLevel + 1)
            {
                templePos = new Vector3(testX, landHeight, testZ);
                templeExists = true;
                break;
            }
        }
    }

    public IEnumerator GenerateIsland()
    {
        if (islandParent == null)
            islandParent = new GameObject("Island").transform;

        Vector3 targetSize = Vector3.one;
        foreach (var p in new[] { grassBlock, dirtBlock, sandBlock, waterPlane, templeFloorBlock })
        {
            if (p != null && !_scaleCache.ContainsKey(p))
                _scaleCache[p] = GetScaleToFit(p, targetSize);
        }

        float halfW = width / 2f;
        float halfH = height / 2f;

        int chunkXCount = Mathf.CeilToInt((float)width / chunkSize);
        int chunkZCount = Mathf.CeilToInt((float)height / chunkSize);

        int totalBlocks = chunkXCount * chunkZCount * chunkSize * chunkSize;
        int blocksGenerated = 0;

        float waterY = seaLevel + 1.5f;
        sandPositions.Clear();
        topGrassPositions.Clear();

        for (int cx = 0; cx < chunkXCount; cx++)
        {
            for (int cz = 0; cz < chunkZCount; cz++)
            {
                Transform chunkParent = new GameObject($"Chunk_{cx}_{cz}").transform;
                chunkParent.SetParent(islandParent);

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

                for (int x = 0; x < chunkSize; x++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        int worldX = cx * chunkSize + x - (int)halfW;
                        int worldZ = cz * chunkSize + z - (int)halfH;

                        float dist = Mathf.Sqrt(worldX * worldX + worldZ * worldZ) / islandRadius;
                        float noiseMask = Mathf.PerlinNoise((worldX + seed) / noiseScale, (worldZ + seed) / noiseScale);
                        float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.45f);
                        islandMask = Mathf.Pow(islandMask, falloffPower);

                        float heightNoise = 0f;
                        float amplitude = 1f;
                        float frequency = 1f;
                        float maxAmp = 0f;
                        for (int o = 0; o < octaves; o++)
                        {
                            float sampleX = (worldX + seed) / noiseScale * frequency;
                            float sampleZ = (worldZ + seed) / noiseScale * frequency;
                            heightNoise += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                            maxAmp += amplitude;
                            amplitude *= persistence;
                            frequency *= lacunarity;
                        }
                        if (maxAmp > 0f) heightNoise /= maxAmp;

                        int landHeight = Mathf.RoundToInt(heightNoise * islandMask * maxHeight);

                        int sandLayers = 0;
                        float distanceToEdge = islandRadius - dist * islandRadius * islandMask;
                        if (distanceToEdge < beachWidth)
                        {
                            float t = Mathf.Clamp01(distanceToEdge / beachWidth);
                            sandLayers = (t > 0.5f) ? 2 : 1;
                        }

                        Vector3 posXZ = new Vector3(worldX, 0, worldZ);
                        bool inTempleArea = templeExists && Vector3.Distance(new Vector3(templePos.x, 0, templePos.z), posXZ) <= templeRadius;

                        if (islandMask > 0f && landHeight > seaLevel)
                        {
                            if (inTempleArea)
                            {
                                float floorY = Mathf.Max(templePos.y, templeY);
                                PlaceBlock(templeFloorBlock, new Vector3(worldX, floorY, worldZ), templeParent);

                                for (int y = seaLevel + 1; y < floorY; y++)
                                {
                                    PlaceBlock(dirtBlock, new Vector3(worldX, y, worldZ), templeParent);
                                }

                                continue;
                            }

                            if (sandLayers > 0)
                            {
                                for (int y = 0; y < sandLayers; y++)
                                {
                                    Vector3 pos = new Vector3(worldX, y, worldZ);
                                    if (pos.y > seaLevel)
                                    {
                                        PlaceBlock(sandBlock, pos, sandParent);
                                        sandPositions.Add(pos);
                                    }
                                }
                                if (landHeight > sandLayers)
                                {
                                    Vector3 grassPos = new Vector3(worldX, sandLayers, worldZ);
                                    PlaceBlock(grassBlock, grassPos, grassParent);
                                    topGrassPositions.Add(grassPos);
                                }
                            }
                            else
                            {
                                GameObject topBlock = (landHeight == seaLevel + 1) ? sandBlock : grassBlock;
                                Vector3 pos = new Vector3(worldX, landHeight, worldZ);
                                PlaceBlock(topBlock, pos, topBlock == grassBlock ? grassParent : topBlock == dirtBlock ? dirtParent : sandParent);
                                if (topBlock == grassBlock) topGrassPositions.Add(pos);
                                if (topBlock == sandBlock && pos.y > seaLevel) sandPositions.Add(pos);
                            }
                        }

                        PlaceBlock(waterPlane, new Vector3(worldX, waterY, worldZ), waterParent);

                        blocksGenerated++;
                        generationProgress = (float)blocksGenerated / totalBlocks;
                    }
                }

                CombineMesh combiner = GetComponent<CombineMesh>();
                if (combiner != null)
                {
                    if (grassParent.childCount > 0)
                        combiner.Combine(grassParent, grassBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Grass");
                    if (dirtParent.childCount > 0)
                        combiner.Combine(dirtParent, dirtBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Dirt");
                    if (sandParent.childCount > 0)
                        combiner.Combine(sandParent, sandBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Sand");
                    if (waterParent.childCount > 0)
                        combiner.Combine(waterParent, waterPlane.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Water");
                    if (templeParent.childCount > 0)
                        combiner.Combine(templeParent, templeFloorBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Temple");
                }

                yield return null;
            }
        }

        if (templeExists && templePrefab != null)
        {
            Instantiate(templePrefab, new Vector3(templePos.x, Mathf.Max(templePos.y, templeY), templePos.z), Quaternion.identity, islandParent);
        }

        SpawnPlayer();
        SpawnForests();
        SpawnPineapples();

        yield return null;
        SceneManager.UnloadSceneAsync("MainScene");
        yield return new WaitForSeconds(1f);
        Loading.Instance.loadingPanel.SetActive(false);
        Loading.Instance.isLoading = false;
    }

    void PlaceBlock(GameObject prefab, Vector3 pos, Transform parent)
    {
        if (prefab == null) return;
        GameObject block = Instantiate(prefab, pos, Quaternion.identity, parent);
        if (_scaleCache.TryGetValue(prefab, out Vector3 s))
            block.transform.localScale = s;
    }

    public Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
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

    void SpawnPlayer()
    {
        if (player == null || sandPositions.Count == 0) return;
        Vector3 spawnPos = sandPositions[0];
        float minDist = float.MaxValue;
        foreach (var pos in sandPositions)
        {
            if (pos.y <= seaLevel) continue;
            float distFromCenter = new Vector2(pos.x, pos.z).magnitude;
            if (distFromCenter >= islandRadius - beachWidth * 2)
            {
                float diff = Mathf.Abs(distFromCenter - (islandRadius - beachWidth / 2));
                if (diff < minDist)
                {
                    minDist = diff;
                    spawnPos = pos;
                }
            }
        }
        player.position = spawnPos + Vector3.up * 1f;
    }

    void SpawnForests()
    {
        if (treePrefabs == null || treePrefabs.Length == 0) return;
        for (int i = 0; i < forestCount; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(islandRadius * 0.3f, islandRadius * 0.8f);
            Vector3 center = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;

            RaycastHit hit;
            if (Physics.Raycast(center + Vector3.up * 100f, Vector3.down, out hit, 200f))
            {
                if (hit.point.y > seaLevel + 1f)
                {
                    if (templeExists && Vector3.Distance(new Vector3(hit.point.x, 0, hit.point.z), new Vector3(templePos.x, 0, templePos.z)) <= templeRadius)
                        continue;

                    SpawnForestCluster(hit.point, islandParent);
                }
            }
        }
    }

    void SpawnForestCluster(Vector3 centerPos, Transform parent)
    {
        Transform clusterParent = new GameObject("ForestCluster").transform;
        clusterParent.SetParent(parent);
        clusterParent.position = centerPos;

        GameObject selectedTreePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
        int treeCount = Random.Range(minTreesPerForest, maxTreesPerForest + 1);

        for (int i = 0; i < treeCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * forestRadius;
            Vector3 spawnPos = centerPos + new Vector3(offset2D.x, 0, offset2D.y);

            RaycastHit hit;
            if (Physics.Raycast(spawnPos + Vector3.up * 50f, Vector3.down, out hit, 100f))
            {
                string groundName = hit.collider.gameObject.name.ToLower();
                if (groundName.Contains("sand")) continue;
                if (hit.point.y > seaLevel + 1f)
                {
                    if (templeExists && Vector3.Distance(new Vector3(hit.point.x, 0, hit.point.z), new Vector3(templePos.x, 0, templePos.z)) <= templeRadius)
                        continue;

                    spawnPos.y = hit.point.y + 0.1f;
                    Instantiate(selectedTreePrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0), clusterParent);
                }
            }
        }
    }

    void SpawnPineapples()
    {
        if (pineapplePrefab == null) return;
        foreach (var grassPos in topGrassPositions)
        {
            if (templeExists && Vector3.Distance(new Vector3(grassPos.x, 0, grassPos.z), new Vector3(templePos.x, 0, templePos.z)) <= templeRadius)
                continue;

            if (Random.value < pineappleSpawnChance)
                Instantiate(pineapplePrefab, grassPos + Vector3.up * 1f, Quaternion.identity, islandParent);
        }
    }
}