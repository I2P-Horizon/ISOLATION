using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/*
 * ============================================================
 * Procedural Island Generation System
 * ============================================================
 * 
 * 섬을 절차적으로 생성하고,
 * 오브젝트, 나무, 돌, 사원 등을 배치하며,
 * 플레이어 스폰 및 씬 전환까지 관리하는 구조.
 * 
 * 1. Data Classes
 *    - Shape: 섬/지형 기본 형태 정보
 *    - Height: 높이/해수면 설정
 *    - Grid: 지형 블록 그리드 정보
 *    - Noise: 펄린 노이즈 설정
 *    - ObjectData: 오브젝트 스폰 정보
 *    - BlockData: 블록 프리팹과 배치/스케일 관련 기능
 * 
 * 2. RockArea
 *    - 섬 내 돌 영역 생성 및 블록/오브젝트 배치
 *    - 메쉬 병합 지원
 * 
 * 3. MapObject
 *    - 생성된 모든 오브젝트 관리
 *    - 특정 영역 오브젝트 비활성화 기능 제공
 * 
 * 4. ObjectSpawner
 *    - 섬 위 오브젝트 배치 (풀, 나무 등)
 *    - 청크 단위 메쉬 병합 지원
 * 
 * 5. Jungle (Shape 상속)
 *    - 정글 생성
 *    - 나무 클러스터 배치
 * 
 * 6. Temple (Shape 상속)
 *    - 사원 위치 결정 및 배치
 * 
 * 7. Island (Shape 상속)
 *    - 섬 생성의 핵심 클래스
 *    - 지형 블록 배치, 물, 모래, 돌, 늪지대 처리
 *    - RockArea 및 ObjectSpawner, Jungle, Temple과 연동
 *    - 플레이어 스폰과 씬 전환 기능 포함
 * 
 * 8. IslandManager (MonoBehaviour)
 *    - 전체 생성 과정을 코루틴으로 관리
 *    - 시드 생성, 사원 위치 배치, 섬 생성, 오브젝트/플레이어 생성
 *    - 씬 전환 처리
 *
 * ============================================================
 */

#region Data
public class Shape
{
    [Header("Shape")]
    public float radius = 200f;
    public float falloffPower = 0.15f;
    public float beachWidth = 3f;
}

[System.Serializable]
public class Height
{
    [Header("Heights")]
    public int maxHeight = 32;
    public int seaLevel = 8;
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
#endregion

#region RockArea
[System.Serializable]
public class RockArea
{
    private Temple temple;
    private MapObject mapObject;
    private BlockData blockData;

    public Transform Parent { get; private set; }

    private Transform rockBlockParent;
    private Transform rockObjectParent;

    public GameObject rockObject;
    [Range(0, 1)] public float spawnChance;

    public void Set(Temple temple, MapObject mapObject, BlockData blockData)
    {
        this.temple = temple;
        this.mapObject = mapObject;
        this.blockData = blockData;
    }

    /// <summary>
    /// 부모 오브젝트 설정
    /// </summary>
    /// <param name="parent"></param>
    public void SetParentObject(Transform parent)
    {
        Parent = new GameObject("RockArea").transform;
        Parent.SetParent(parent);

        rockBlockParent = new GameObject("RockBlocks").transform;
        rockBlockParent.SetParent(Parent);

        rockObjectParent = new GameObject("RockObjects").transform;
        rockObjectParent.SetParent(Parent);
    }

    /// <summary>
    /// 돌 영역 생성
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rockBlock"></param>
    /// <param name="rockObject"></param>
    public void Generate(Vector3 pos, GameObject rockBlock, GameObject rockObject)
    {
        if (temple.exists && Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(temple.pos.x, 0, temple.pos.z)) <= temple.radius)
            return;

        /* 돌 블록 생성 */
        GameObject rock = MonoBehaviour.Instantiate(rockBlock, pos, Quaternion.identity, rockBlockParent);

        /* BlockData 에서 지정한 캐시 값에 따라 블록 스케일 조정 */
        if (blockData != null && blockData.scaleCache.TryGetValue(rockBlock, out Vector3 scale))
            rock.transform.localScale = scale;
        else rock.transform.localScale = Vector3.one;

        /* RockObject 생성 확률 처리 (0, 1) */
        if (rockObject != null && Random.value <= spawnChance)
        {
            /* 돌 블록 위에 돌 오브젝트 배치 */
            /* 오일러 함수를 사용하여 Y축 랜덤 회전 값을 정하고, RockObject를 랜덤한 방향으로 스폰되도록 설정. */
            Vector3 objPos = pos + Vector3.up * rock.transform.localScale.y;
            GameObject obj = MonoBehaviour.Instantiate(rockObject, objPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0), rockObjectParent);

            /* RockObject 랜덤 크기 적용 */
            /* 랜덤 크기 값은 추후 클래스 멤버로 올려 인스펙터에서 관리하도록 수정 예정. */
            float rockSize = Random.Range(0.8f, 1.3f);
            obj.transform.localScale = Vector3.one * rockSize;

            /* 맵 매니저에 등록 (아이템 배치 시 비활성화 위함) */
            if (mapObject != null) mapObject.RegisterObject(obj);
        }

        /* 돌 블록 병합 */
        /* 돌 영역에 있는 블록과 프리팹을 모두 같은 부모 오브젝트에 두면 병합 시 문제 발생. */
        /* rockBlockParent, rockObjectParent로 부모 오브젝트를 분리하여 rockBlockParent만 병합되도록 설정. */
        CombineMesh combiner = MonoBehaviour.FindAnyObjectByType<CombineMesh>();
        if (combiner != null) combiner.Combine(rockBlockParent, rockBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{Parent.name}_Rock");
    }
}
#endregion

#region MapObject
public class MapObject
{
    private Grid grid;
    private List<GameObject> allObjects = new List<GameObject>();

    private Dictionary<Vector2Int, GameObject> chunkMap = new Dictionary<Vector2Int, GameObject>();

    public void Set(Grid grid)
    {
        this.grid = grid;
    }

    public void RegisterChunk(Vector2Int chunkIndex, GameObject chunkObj)
    {
        chunkMap[chunkIndex] = chunkObj;
    }

    public void RegisterObject(GameObject obj)
    {
        allObjects.Add(obj);
    }

    public void Deactivate(Vector3 pos, Vector3 size)
    {
        Vector3 halfSize = size / 2f;
        Bounds deactivateBounds = new Bounds(pos, size);

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

#region ObjectSpawner
public class ObjectSpawner
{
    private Island island;
    private Grid grid;
    private Temple temple;
    private ObjectData[] objectData;
    private MapObject mapObject;

    public void Set(Island island, Grid grid, Temple temple, ObjectData[] objectData, MapObject mapObject)
    {
        this.island = island; this.grid = grid; this.temple = temple; this.objectData = objectData; this.mapObject = mapObject;
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
#endregion

#region Junble : Shape
[System.Serializable]
public class Jungle : Shape
{
    private Island island;
    private Height height;
    private Temple temple;
    private MapObject mapObject;

    [Header("Jungle Settings")]
    public int count = 20;
    public int minTreesPerJungle = 60;
    public int maxTreesPerJungle = 70;
    public GameObject[] treePrefabs;

    public void Set(Island island, Height height, Temple temple, MapObject mapObject)
    {
        this.island = island; this.height = height; this.temple = temple; this.mapObject = mapObject;
    }

    public void Spawn()
    {
        if (treePrefabs == null || treePrefabs.Length == 0) return;

        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(radius * 0.3f, radius * 0.8f);
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

                /* 모래 블록, 돌 블록 위에는 생성되지 않도록 함. */
                if (groundName.Contains("sand") || groundName.Contains("rock")) continue;

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

#region Temple : Shape
[System.Serializable]
public class Temple : Shape
{
    private Height height;
    private Noise noise;

    [Header("Temple Settings")]
    public GameObject prefab;
    public float scaleY = 5f;
    public float maxDistanceFromCenter = 50f;

    [HideInInspector]
    public Vector3 pos;
    public bool exists = false;

    public void Set(Height height, Noise noise)
    {
        this.height = height; this.noise = noise;
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

            float dist = Mathf.Sqrt(testX * testX + testZ * testZ) / radius;
            float noiseMask = Mathf.PerlinNoise((testX + noise.seed) / noise.scale, (testZ + noise.seed) / noise.scale);
            float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.45f);
            islandMask = Mathf.Pow(islandMask, falloffPower);
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

#region Island : Shape
[System.Serializable]
public class Island : Shape
{
    private Height height;
    private Grid grid;
    private Noise noise;
    private Temple temple;
    private MapObject mapObject;
    private BlockData blockData;

    public Transform pos;
    public Transform player;

    [SerializeField] private RockArea rockArea;

    public Transform Root;
    public Transform grassRoot;
    public Transform dirtRoot;
    public Transform sandRoot;
    public Transform waterRoot;
    public Transform templeRoot;
    public Transform swampRoot;
    public Transform rockRoot;

    [HideInInspector] public List<Vector3> rockPositions = new List<Vector3>();

    [HideInInspector] public List<Vector3> sandPositions = new List<Vector3>();
    [HideInInspector] public List<Vector3> TopGrassPositions { get; private set; } = new List<Vector3>();

    public void Set(Height height, Grid grid, Noise noise, Temple temple, BlockData blockData, MapObject mapObject)
    {
        this.height = height; this.grid = grid; this.noise = noise; this.temple = temple; this.blockData = blockData;
    }

    /// <summary>
    /// 주변 블록과 계단식으로 맞춰서 자연스럽게 평탄화
    /// </summary>
    private void flattenTempleArea(int steps = 3)
    {
        if (!temple.exists) return;

        int r = Mathf.CeilToInt(temple.radius);
        int centerY = Mathf.RoundToInt(temple.pos.y);

        /* 고대 사원 반경 안 모든 블록 높이 계산 */
        for (int x = -r; x <= r; x++)
        {
            for (int z = -r; z <= r; z++)
            {
                Vector2 offset = new Vector2(x, z);
                float dist = offset.magnitude;

                if (dist > temple.radius) continue;

                /* 계단식 높이 계산 */
                float t = dist / temple.radius; // 0 ~ 1
                int stepHeight = Mathf.RoundToInt(centerY - t * steps);

                int worldX = Mathf.RoundToInt(temple.pos.x + x);
                int worldZ = Mathf.RoundToInt(temple.pos.z + z);

                /* 기존 블록 제거 후 계단식으로 사원 바닥 배치 */
                Vector3 posXZ = new Vector3(worldX, stepHeight, worldZ);

                /* 기존 블록 제거 */
                TopGrassPositions.RemoveAll(p => Mathf.RoundToInt(p.x) == worldX && Mathf.RoundToInt(p.z) == worldZ);

                /* 고대 사원 바닥 블록 배치 */
                blockData.PlaceBlock(blockData.templeFloorBlock, posXZ, templeRoot);

                /* 고대 사원 바닥 아래는 흙 블록으로 채움 */
                for (int y = height.seaLevel + 1; y < stepHeight; y++)
                {
                    blockData.PlaceBlock(blockData.dirtBlock, new Vector3(worldX, y, worldZ), templeRoot);
                }
            }
        }
    }

    public void SpawnMineEntrance(GameObject minePrefab)
    {
        if (rockPositions.Count == 0 || minePrefab == null) return;

        /* y >= 13.7 필터링 */
        var validRocks = rockPositions.Where(r => r.y >= 13.7f).ToList();
        if (validRocks.Count == 0) return;

        Vector3 baseRockPos = validRocks[Random.Range(0, validRocks.Count)];

        /* 같은 x, z 좌표의 돌들 중 최대 y값 찾기 */
        float maxY = rockPositions
            .Where(r => Mathf.RoundToInt(r.x) == Mathf.RoundToInt(baseRockPos.x) && Mathf.RoundToInt(r.z) == Mathf.RoundToInt(baseRockPos.z))
            .Max(r => r.y);

        Vector3 spawnPos = new Vector3(baseRockPos.x, maxY, baseRockPos.z);

        MonoBehaviour.Instantiate(minePrefab, spawnPos, Quaternion.identity, Root);
    }

    public IEnumerator Spawn(Transform parent)
    {
        Root.SetParent(parent);

        Vector3 targetSize = Vector3.one;
        foreach (var p in new[] { blockData.grassBlock, blockData.dirtBlock, blockData.sandBlock, blockData.waterPlane, blockData.templeFloorBlock, blockData.swampBlock, blockData.rockBlock })
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

                /* 청크 단위로 나누기 위해 자식 오브젝트 생성 */
                Transform grassParent = new GameObject("Grass").transform; grassParent.SetParent(chunkParent);
                Transform dirtParent = new GameObject("Dirt").transform; dirtParent.SetParent(chunkParent);
                Transform sandParent = new GameObject("Sand").transform; sandParent.SetParent(chunkParent);
                Transform waterParent = new GameObject("Water").transform; waterParent.SetParent(chunkParent);
                Transform templeParent = new GameObject("Temple").transform; templeParent.SetParent(chunkParent);
                Transform swampParent = new GameObject("Swamp").transform; swampParent.SetParent(chunkParent);
                Transform rockParent = new GameObject("Rock").transform; rockParent.SetParent(chunkParent);

                /* 영역마다 부모 오브젝트로 나눔 */
                grassParent.SetParent(grassRoot);
                dirtParent.SetParent(dirtRoot);
                sandParent.SetParent(sandRoot);
                waterParent.SetParent(waterRoot);
                templeParent.SetParent(templeRoot);
                swampParent.SetParent(swampRoot);
                rockParent.SetParent(rockRoot);

                for (int x = 0; x < grid.chunkSize; x++)
                {
                    for (int z = 0; z < grid.chunkSize; z++)
                    {
                        int worldX = cx * grid.chunkSize + x - (int)halfW;
                        int worldZ = cz * grid.chunkSize + z - (int)halfH;

                        float dist = Mathf.Sqrt(worldX * worldX + worldZ * worldZ) / radius;
                        float noiseMask = Mathf.PerlinNoise((worldX + noise.seed) / noise.scale, (worldZ + noise.seed) / noise.scale);
                        float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.45f);
                        islandMask = Mathf.Pow(islandMask, falloffPower);

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

                        int sandLayers = 0;
                        float distanceToEdge = radius - dist * radius * islandMask;

                        if (distanceToEdge < beachWidth)
                        {
                            float t = Mathf.Clamp01(distanceToEdge / beachWidth);
                            sandLayers = (t > 0.5f) ? 2 : 1;
                        }

                        Vector3 posXZ = new Vector3(worldX, 0, worldZ);
                        bool inTempleArea = temple.exists && Vector3.Distance(new Vector3(temple.pos.x, 0, temple.pos.z), posXZ) <= temple.radius;

                        int landHeight = Mathf.RoundToInt(heightNoise * islandMask * height.maxHeight);
                        float floorY = Mathf.Max(temple.pos.y, temple.scaleY + 2);

                        if (islandMask > 0f && landHeight > height.seaLevel)
                        {
                            if (inTempleArea)
                            {
                                blockData.PlaceBlock(blockData.templeFloorBlock, new Vector3(worldX, floorY, worldZ), templeParent);
                                for (int y = height.seaLevel + 1; y < floorY; y++) blockData.PlaceBlock(blockData.dirtBlock, new Vector3(worldX, y, worldZ), templeParent);
                                continue;
                            }

                            bool placed = false;

                            ///* 늪지대 생성 */
                            //if (!placed && landHeight > sandLayers)
                            //{
                            //    float swampMask = Mathf.PerlinNoise(worldX / 10f, worldZ / 10f);
                            //    if (swampMask > 0.6f && dist > shape.radius * 0.5f)
                            //    {
                            //        Vector3 swampPos = new Vector3(worldX, landHeight, worldZ);
                            //        blockData.PlaceBlock(blockData.swampBlock, swampPos, swampParent);
                            //        placed = true;
                            //    }
                            //}

                            rockArea.Set(temple, mapObject, blockData);
                            rockArea.SetParentObject(chunkParent);

                            /* 돌 영역 생성 */
                            if (!placed && landHeight > sandLayers)
                            {
                                /* worldX/worldZ 좌표를 80으로 나누어 노이즈의 스케일을 크게 만들어 큰 영역 단위로 돌 생성 */
                                float rockMask = Mathf.PerlinNoise(worldX / 80f, worldZ / 80f);
                                if (rockMask > 0.85f)
                                {
                                    Vector3 rockPos = new Vector3(worldX, landHeight, worldZ);
                                    rockArea.Generate(rockPos, blockData.rockBlock, rockArea.rockObject);

                                    rockPositions.Add(rockPos);

                                    /* 돌을 생성했으므로 placed를 true로 변경 */
                                    /* 이후 모래나 잔디를 배치하는 로직에서는 이 좌표를 건너뜀. */
                                    placed = true;
                                }
                            }

                            /* 모래/잔디 처리 */
                            if (!placed)
                            {
                                if (sandLayers > 0)
                                {
                                    for (int y = 0; y < sandLayers; y++)
                                    {
                                        Vector3 pos = new Vector3(worldX, y, worldZ);
                                        blockData.PlaceBlock(blockData.sandBlock, pos, sandParent);
                                        sandPositions.Add(pos);
                                    }

                                    if (landHeight > sandLayers)
                                    {
                                        Vector3 grassPos = new Vector3(worldX, sandLayers, worldZ);
                                        blockData.PlaceBlock(blockData.grassBlock, grassPos, grassParent);
                                        TopGrassPositions.Add(grassPos);

                                        if (inTempleArea == false && Vector3.Distance(new Vector3(temple.pos.x, 0, temple.pos.z), new Vector3(worldX, 0, worldZ)) <= temple.radius + 1f)
                                        {
                                            for (int i = 0; i < 6; i++)
                                            {
                                                Vector3 dirtPos = new Vector3(worldX, sandLayers - i, worldZ);
                                                blockData.PlaceBlock(blockData.dirtBlock, dirtPos, dirtParent);
                                            }
                                        }
                                    }
                                }

                                else
                                {
                                    GameObject topBlock = (landHeight == height.seaLevel + 1) ? blockData.sandBlock : blockData.grassBlock;
                                    Vector3 pos = new Vector3(worldX, landHeight, worldZ);
                                    blockData.PlaceBlock(topBlock, pos, topBlock == blockData.grassBlock ? grassParent : sandParent);

                                    if (topBlock == blockData.grassBlock)
                                    {
                                        TopGrassPositions.Add(pos);

                                        if (inTempleArea == false && Vector3.Distance(new Vector3(temple.pos.x, 0, temple.pos.z), new Vector3(worldX, 0, worldZ)) <= temple.radius + 1f)
                                        {
                                            for (int i = 1; i <= 6; i++)
                                            {
                                                Vector3 dirtPos = new Vector3(worldX, landHeight - i, worldZ);
                                                blockData.PlaceBlock(blockData.dirtBlock, dirtPos, dirtParent);
                                            }
                                        }
                                    }

                                    if (topBlock == blockData.sandBlock && pos.y > height.seaLevel) sandPositions.Add(pos);
                                }
                            }
                        }

                        if (islandMask > 0f && landHeight <= height.seaLevel)
                        {
                            Vector3 dirtUnderWaterPos = new Vector3(worldX, waterY - 1.5f, worldZ);
                            blockData.PlaceBlock(blockData.dirtBlock, dirtUnderWaterPos, dirtParent);
                        }

                        blockData.PlaceBlock(blockData.waterPlane, new Vector3(worldX, waterY, worldZ), waterParent);

                        blocksGenerated++;
                        IslandManager.generationProgress = (float)blocksGenerated / totalBlocks;
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
                    if (swampParent.childCount > 0)
                        combiner.Combine(swampParent, blockData.swampBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Swamp");
                    if (rockParent.childCount > 0)
                        combiner.Combine(rockParent, blockData.rockBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Rock");
                }

                yield return null;
            }
        }

        flattenTempleArea();

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

            if (distFromCenter >= radius - beachWidth * 2)
            {
                float diff = Mathf.Abs(distFromCenter - (radius - beachWidth / 2));

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
            yield return new WaitForSeconds(0.5f);
        }
    }
}
#endregion

public class IslandManager : MonoBehaviour
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

    /// <summary>
    /// 섬 생성 완료 시점에 보내는 신호
    /// </summary>
    public static event Action OnGenerationComplete;

    /// <summary>
    /// 섬 생성 진행도 (Loading.cs 에서 필요)
    /// </summary>
    public static float generationProgress = 0f;

    private ObjectSpawner objectSpawner = new ObjectSpawner();
    private MapObject mapObject = new MapObject();

    public MapObject mapObj => mapObject;

    public GameObject mine;

    private void navMeshBuild(GameObject obj)
    {
        NavMeshSurface navMesh = obj.AddComponent<NavMeshSurface>();
        navMesh.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        navMesh.collectObjects = CollectObjects.Children;
        navMesh.BuildNavMesh();
    }

    /// <summary>
    /// 섬 생성 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGeneration()
    {
        /* 시드 생성 */
        noise.Seed();

        /* 사원 위치 생성 */
        temple.Placement();

        /* 섬 생성 */
        yield return StartCoroutine(island.Spawn(island.pos));

        /* 섬 생성이 완료되면 오브젝트/플레이어 생성 */
        objectSpawner.SpawnObjects();
        jungle.Spawn();
        island.SpawnMineEntrance(mine);
        island.SpawnPlayer();

        /* 경로 Bake */
        navMeshBuild(island.grassRoot.gameObject);
        navMeshBuild(island.sandRoot.gameObject);

        /* Game Scene 으로 변경 */
        yield return StartCoroutine(island.SceneChange());

        /* 섬 생성 완료 신호 */
        OnGenerationComplete?.Invoke();
    }

    private void Start()
    {
        island.Set(height, grid, noise, temple, blockData, mapObject);
        jungle.Set(island, height, temple, mapObject);
        temple.Set(height, noise);
        mapObject.Set(grid);
        objectSpawner.Set(island, grid, temple, objectData, mapObject);

        StartCoroutine(StartGeneration());
    }
}