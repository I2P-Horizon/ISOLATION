using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Data
[System.Serializable]
public class Height
{
    [Header("Heights")]
    public int maxHeight = 32;
    public int seaLevel = 8;
}

[System.Serializable]
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
        this.height = height; this.shape = shape; this.noise = noise;
    }

    public void Placement()
    {
        exists = false;

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

            float heightNoise = 0f; float amplitude = 1f; float frequency = 1f; float maxAmp = 0f;

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

#region Jungle
[System.Serializable]
public class Jungle
{
    private Island island;
    private Height height;
    private Shape shape;
    private Temple temple;
    private MapObject mapObject;

    [Header("Jungle Settings")]
    public int count = 20;
    public int minTreesPerJungle = 60;
    public int maxTreesPerJungle = 80;
    public float radius = 80f;
    public GameObject[] treePrefabs;

    public void Set(Island island, Height height, Shape shape, Temple temple, MapObject mapObject)
    {
        this.island = island; this.height = height; this.shape = shape; this.temple = temple; this.mapObject = mapObject;
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

                    SpawnJungleCluster(hit.point, island.Root);
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

        float blockSize = 1f;

        for (int i = 0; i < treeCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * radius;
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

                    spawnPos.x = Mathf.Round(hit.point.x / blockSize) * blockSize;
                    spawnPos.z = Mathf.Round(hit.point.z / blockSize) * blockSize;
                    spawnPos.y = Mathf.Round(hit.point.y / blockSize) * blockSize;

                    GameObject tree = MonoBehaviour.Instantiate(selectedTreePrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0), clusterParent);
                    mapObject.RegisterObject(tree);
                }
            }
        }
    }
}
#endregion

#region Object
public class ObjectSpawner
{
    private Island island;
    private Grid grid;
    private Temple temple;
    private ObjectData[] objectData;
    private BlockData blockData;
    private MapObject mapObject;

    public void Set(Island island, Grid grid, Temple temple, BlockData blockData, ObjectData[] objectData, MapObject mapObject)
    {
        this.island = island; this.grid = grid; this.temple = temple; this.blockData = blockData; this.objectData = objectData; this.mapObject = mapObject;
    }

    public void SpawnObjects()
    {
        if (objectData == null || objectData.Length == 0) return;
        CombineMesh combiner = MonoBehaviour.FindAnyObjectByType<CombineMesh>();

        foreach (var obj in objectData)
        {
            if (obj.prefab == null) continue;
            Transform objectParent = new GameObject(obj.prefab.name + "_Objects").transform;
            objectParent.SetParent(island.Root);

            Dictionary<string, Transform> chunkParents = new Dictionary<string, Transform>();

            foreach (var grassPos in island.TopGrassPositions)
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

                            mapObject.RegisterChunk(new Vector2Int(chunkX, chunkZ), chunkObjParent.gameObject);
                        }

                        parentForSpawn = chunkParents[chunkKey];
                    }

                    GameObject mapObj = MonoBehaviour.Instantiate(obj.prefab, grassPos + Vector3.up * 1f, Quaternion.identity, parentForSpawn);
                    mapObject.RegisterObject(mapObj);
                }
            }

            if (obj.mergeMeshes && combiner != null)
            {
                if (obj.chunkSeparate)
                {
                    foreach (var kvp in chunkParents)
                    {
                        if (kvp.Value.childCount > 0)
                        {
                            GameObject merged = combiner.Combine(kvp.Value, obj.prefab.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{obj.prefab.name}_Merged_{kvp.Key}");
                            if (merged != null) mapObject.RegisterObject(merged);
                        }
                    }
                }
                else
                {
                    if (objectParent.childCount > 0)
                    {
                        GameObject merged = combiner.Combine(objectParent, obj.prefab.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{obj.prefab.name}_Merged");
                        if (merged != null) mapObject.RegisterObject(merged);
                    }
                }
            }
        }
    }
}

public class MapObject
{
    private Grid grid;
    private List<GameObject> allObjects = new List<GameObject>();

    private Dictionary<Vector2Int, GameObject> chunkMap = new Dictionary<Vector2Int, GameObject>();

    public void Set(Grid grid)
    {
        this.grid = grid;
    }

    /// <summary>
    /// 청크 등록
    /// </summary>
    /// <param name="chunkIndex"></param>
    /// <param name="chunkObj"></param>
    public void RegisterChunk(Vector2Int chunkIndex, GameObject chunkObj)
    {
        chunkMap[chunkIndex] = chunkObj;
    }

    /// <summary>
    /// 개별 오브젝트 등록
    /// </summary>
    /// <param name="obj"></param>
    public void RegisterObject(GameObject obj)
    {
        allObjects.Add(obj);
    }

    /// <summary>
    /// <para>지정한 위치와 크기(pos, size)에 포함된 모든 청크와 개별 오브젝트를 비활성화.</para>
    /// <para>pos는 영역의 중심, size는 영역의 크기.</para>
    /// <para>청크 단위로 먼저 비활성화 후, 범위 안 개별 오브젝트도 비활성화.</para>
    /// </summary>
    /// <param name="pos">비활성화할 영역의 중심 위치</param>
    /// <param name="size">비활성화할 영역의 크기</param>
    public void Deactivate(Vector3 pos, Vector3 size)
    {
        Vector3 halfSize = size / 2f;
        Bounds deactivateBounds = new Bounds(pos, size);

        /* 청크 단위 비활성화 */
        foreach (var kvp in chunkMap.ToList())
        {
            GameObject chunk = kvp.Value;
            if (chunk == null) { chunkMap.Remove(kvp.Key); continue; }

            Renderer rend = chunk.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                if (deactivateBounds.Intersects(rend.bounds))
                {
                    chunk.SetActive(false);
                    chunkMap.Remove(kvp.Key);
                }
            }
            else
            {
                Vector3 chunkPos = chunk.transform.position;
                if (Mathf.Abs(chunkPos.x - pos.x) <= halfSize.x &&
                    Mathf.Abs(chunkPos.y - pos.y) <= halfSize.y &&
                    Mathf.Abs(chunkPos.z - pos.z) <= halfSize.z)
                {
                    chunk.SetActive(false);
                    chunkMap.Remove(kvp.Key);
                }
            }
        }

        /* 범위 내 개별 오브젝트 비활성화 */
        for (int i = allObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = allObjects[i];
            if (obj == null) { allObjects.RemoveAt(i); continue; }

            Renderer rend = obj.GetComponentInChildren<Renderer>();
            bool inBounds = false;

            if (rend != null)
            {
                inBounds = deactivateBounds.Intersects(rend.bounds);
            }
            else
            {
                Vector3 objPos = obj.transform.position;
                if (Mathf.Abs(objPos.x - pos.x) <= halfSize.x &&
                    Mathf.Abs(objPos.y - pos.y) <= halfSize.y &&
                    Mathf.Abs(objPos.z - pos.z) <= halfSize.z)
                {
                    inBounds = true;
                }
            }

            if (inBounds)
            {
                obj.SetActive(false);
                allObjects.RemoveAt(i);
            }
        }
    }
}
#endregion

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
    private MapObject mapObject;

    public Transform Root { get; private set; }
    public Transform pos;
    public Transform player;

    [HideInInspector] public List<Vector3> sandPositions = new List<Vector3>();
    [HideInInspector] public List<Vector3> TopGrassPositions { get; private set; } = new List<Vector3>();

    public void Set(Height height, Shape shape, Grid grid, Noise noise, Jungle jungle, Temple temple, BlockData blockData, MapObject mapObject)
    {
        this.height = height; this.shape = shape; this.grid = grid; this.noise = noise; this.jungle = jungle; this.temple = temple; this.blockData = blockData; this.mapObject = mapObject;
    }

    public IEnumerator Spawn(Transform parent)
    {
        Root = new GameObject("Island").transform;
        Root.SetParent(parent);

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
        TopGrassPositions.Clear();

        for (int cx = 0; cx < chunkXCount; cx++)
        {
            for (int cz = 0; cz < chunkZCount; cz++)
            {
                Transform chunkParent = new GameObject($"Chunk_{cx}_{cz}").transform;
                chunkParent.SetParent(Root);

                Transform grassParent = new GameObject("Grass").transform; grassParent.SetParent(chunkParent);
                Transform dirtParent = new GameObject("Dirt").transform; dirtParent.SetParent(chunkParent);
                Transform sandParent = new GameObject("Sand").transform; sandParent.SetParent(chunkParent);
                Transform waterParent = new GameObject("Water").transform; waterParent.SetParent(chunkParent);
                Transform templeParent = new GameObject("Temple").transform; templeParent.SetParent(chunkParent);

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

                        float heightNoise = 0f; float amplitude = 1f; float frequency = 1f; float maxAmp = 0f;

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
                                    TopGrassPositions.Add(grassPos);
                                }
                            }

                            else
                            {
                                GameObject topBlock = (landHeight == height.seaLevel + 1) ? blockData.sandBlock : blockData.grassBlock;
                                Vector3 pos = new Vector3(worldX, landHeight, worldZ);
                                blockData.PlaceBlock(topBlock, pos, topBlock == blockData.grassBlock ? grassParent : topBlock == blockData.dirtBlock ? dirtParent : sandParent);

                                if (topBlock == blockData.grassBlock)
                                {
                                    TopGrassPositions.Add(pos);

                                    if (temple.exists)
                                    {
                                        float distFromTemple = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(temple.pos.x, 0, temple.pos.z));

                                        if (Mathf.Abs(distFromTemple - temple.radius) <= 1f)
                                        {
                                            for (int y = 0; y < 6; y++)
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
                    if (templeParent.childCount > 0 && blockData.templeFloorBlock != null)
                        combiner.Combine(templeParent, blockData.templeFloorBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Temple");
                }

                yield return null;
            }
        }

        if (temple.exists && temple.prefab != null)
            MonoBehaviour.Instantiate(temple.prefab, new Vector3(temple.pos.x, Mathf.Max(temple.pos.y, temple.scaleY), temple.pos.z), Quaternion.identity, Root);
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

    public IEnumerator SceneChange()
    {
        WorldMapMarker worldMapMarker = MonoBehaviour.FindFirstObjectByType<WorldMapMarker>();
        while (worldMapMarker != null && worldMapMarker.isRendering) yield return null;
        SceneManager.UnloadSceneAsync("MainScene");
        yield return new WaitForSeconds(0.1f);

        if (Loading.Instance != null)
        {
            Loading.Instance.loadingPanel.SetActive(false);
            Loading.Instance.isLoading = false;
        }
    }
}
#endregion

public class IslandGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Island island;
    [SerializeField] private Temple temple;
    [SerializeField] private Jungle jungle;
    [SerializeField] private Height height;
    [SerializeField] private Shape shape;
    [SerializeField] private Grid grid;
    [SerializeField] private Noise noise;

    [Header("Data")]
    [SerializeField] private BlockData blockData;
    [SerializeField] private ObjectData[] objectData;

    public static float generationProgress = 0f;

    private ObjectSpawner objectSpawner = new ObjectSpawner();
    private MapObject mapObject = new MapObject();

    public MapObject mapObj => mapObject;

    private IEnumerator RunGeneration()
    {
        yield return StartCoroutine(island.Spawn(island.pos));
        objectSpawner.SpawnObjects();
        jungle.Spawn();
        island.SpawnPlayer();
        yield return StartCoroutine(island.SceneChange());
    }

    private void Start()
    {
        /* Setter */
        island.Set(height, shape, grid, noise, jungle, temple, blockData, mapObject);
        jungle.Set(island, height, shape, temple, mapObject);
        temple.Set(height, shape, noise);
        mapObject.Set(grid);
        objectSpawner.Set(island, grid, temple, blockData, objectData, mapObject);

        /* 시드 생성 및 사원 위치 결정 */
        noise.Seed();
        temple.Placement();

        /* 섬 생성 */
        StartCoroutine(RunGeneration());
    }
}