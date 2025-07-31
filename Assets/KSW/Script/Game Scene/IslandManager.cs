using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 블록 관리
[System.Serializable]
public class Block
{
    public GameObject grass;
    public GameObject dirt;
    public GameObject sand;
    public GameObject water;
}
#endregion

#region 맵 크기 관리
[System.Serializable]
public class MapScale
{
    [Tooltip("섬의 크기")] public int total = 250;
    [Tooltip("해변의 크기")] public float beach = 15f;
    [Tooltip("호수의 크기")] public float lake = 20f;
    [Tooltip("노이즈 크기")] public float noise = 5f;
    [Tooltip("블록 크기")] public Vector3 block = new Vector3(1, 1, 1);
}
#endregion

#region 맵 높이 관리
[System.Serializable]
public class MapHight
{
    [Tooltip("최대 잔디 높이")] public int maxGrass = 4;
    [Tooltip("최소 잔디 높이")] public int minGrass = 1;
    [Tooltip("최대 노이즈 규모")] public float noiseScale = 8f;
    [Header("블록 높이 기준선")] public float sandHeight = 0f;
}
#endregion

#region 시드 값 관리
[System.Serializable]
public class MapSeed
{
    public float x;
    public float z;
    public float heightX;
    public float heightZ;
}
#endregion

#region 맵 오브젝트 관리

/// <summary>
/// 고유 오브젝트 위치 타입 (수정 필요)
/// </summary>
public enum UniquePlacementType
{
    CenterArea,
    EdgeArea,
    Custom
}

[System.Serializable]
public class MapObject
{
    public GameObject mapObject;
    public float spawnChance;
    public bool isUnique;
    public UniquePlacementType placementType;
}
#endregion

public class IslandManager : MonoBehaviour
{
    [Header("부모 오브젝트 (섬 전체)")]
    public Transform islandParent;

    [Header("블록")] public Block block;
    [Header("맵 크기")] public MapScale mapScale;
    [Header("맵 높이")] public MapHight mapHight;
    [Header("시드 값")] public MapSeed mapSeed;
    [Header("맵 오브젝트")] public MapObject[] mapObjects;

    private Vector2 lakeCenter;

    private List<Vector3> uniqueObjectPositions = new List<Vector3>();

    /// <summary>
    /// 랜덤 시드 값 생성
    /// </summary>
    public void Init()
    {
        mapSeed.x = Random.Range(0f, 10000f);
        mapSeed.z = Random.Range(0f, 10000f);
        mapSeed.heightX = Random.Range(0f, 10000f);
        mapSeed.heightZ = Random.Range(0f, 10000f);

        // 호수
        lakeCenter = new Vector2(
            Random.Range(mapScale.total * 0.3f, mapScale.total * 0.7f),
            Random.Range(mapScale.total * 0.3f, mapScale.total * 0.7f));
    }

    /// <summary>
    /// 섬 삭제
    /// </summary>
    public void Clear()
    {
        if (islandParent == null) return;
        for (int i = islandParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(islandParent.GetChild(i).gameObject);
    }

    /// <summary>
    /// 섬 생성
    /// </summary>
    public void Create()
    {
        Vector3 islandOrigin = islandParent.position + new Vector3(-mapScale.total / 2f * mapScale.block.x, 0, -mapScale.total / 2f * mapScale.block.z);
        Vector3 grassScale = GetScaleToFit(block.grass, mapScale.block);
        Vector3 dirtScale = GetScaleToFit(block.dirt, mapScale.block);
        Vector3 sandScale = GetScaleToFit(block.sand, mapScale.block);
        Vector3 waterScale = GetScaleToFit(block.water, mapScale.block);

        Vector2 center = new Vector2(mapScale.total / 2f, mapScale.total / 2f);

        UniqueObjectPlacement();

        for (int x = 0; x < mapScale.total; x++)
        {
            for (int z = 0; z < mapScale.total; z++)
            {
                Vector2 pos2D = new Vector2(x, z);
                float dist = Vector2.Distance(pos2D, center);
                float islandNoise = Mathf.PerlinNoise((x + mapSeed.x) / mapScale.noise, (z + mapSeed.z) / mapScale.noise) * 5f;
                float finalDist = dist - islandNoise;

                if (finalDist > mapScale.total / 2f) continue;

                // 호수
                if (IsLakeEdge(x, z) || IsLakeInner(x, z))
                {
                    int dirtLayers = 0;

                    if (IsLakeInner(x, z)) dirtLayers = 1;
                    else dirtLayers = 2;

                    for (int i = 0; i < dirtLayers; i++)
                    {
                        float y = mapHight.sandHeight + i * mapScale.block.y;
                        Vector3 pos = islandOrigin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);
                        GameObject dirt = Instantiate(block.dirt, pos, Quaternion.identity, islandParent);
                        dirt.transform.localScale = dirtScale;
                    }

                    float waterY = mapHight.sandHeight + 2 * mapScale.block.y + 0.5f * mapScale.block.y;
                    Vector3 waterPos = islandOrigin + new Vector3(x * mapScale.block.x, waterY, z * mapScale.block.z);
                    GameObject water = Instantiate(block.water, waterPos, Quaternion.identity, islandParent);
                    water.transform.localScale = waterScale;

                    continue;
                }

                int sandLayers = 1;
                if (finalDist <= mapScale.total / 2f && finalDist > mapScale.total / 2f - mapScale.beach)
                {
                    float beachDepth = mapScale.total / 2f - finalDist;
                    float t = Mathf.Clamp01(beachDepth / mapScale.beach);
                    sandLayers = (t > 0.5f) ? 2 : 1;
                }

                else if (finalDist <= mapScale.total / 2f - mapScale.beach) sandLayers = 2;

                bool isGrass = finalDist <= mapScale.total / 2f - mapScale.beach;
                bool placeSand = true;

                if (isGrass)
                {
                    float innerDistance = finalDist - (mapScale.total / 2f - mapScale.beach);
                    float maxInnerDistance = mapScale.total / 2f - mapScale.beach;
                    float t = Mathf.Clamp01(innerDistance / maxInnerDistance);

                    int grassLayers = Mathf.RoundToInt(Mathf.Lerp(mapHight.minGrass, mapHight.maxGrass, t));
                    float noise = Mathf.PerlinNoise((x + mapSeed.heightX) / mapHight.noiseScale, (z + mapSeed.heightZ) / mapHight.noiseScale);
                    int noiseLayers = Mathf.RoundToInt(noise * (mapHight.maxGrass - mapHight.minGrass));

                    grassLayers = Mathf.Clamp(grassLayers + noiseLayers - (mapHight.maxGrass - mapHight.minGrass) / 2, mapHight.minGrass, mapHight.maxGrass);
                    
                    if (grassLayers > 0) placeSand = false;
                }

                if (placeSand && sandLayers > 0)
                {
                    int topIndex = sandLayers - 1;
                    float y = mapHight.sandHeight + topIndex * mapScale.block.y;
                    Vector3 sandPos = islandOrigin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);
                    GameObject sand = Instantiate(block.sand, sandPos, Quaternion.identity, islandParent);
                    sand.transform.localScale = sandScale;
                }

                if (isGrass)
                {
                    float innerDistance = finalDist - (mapScale.total / 2f - mapScale.beach);
                    float maxInnerDistance = mapScale.total / 2f - mapScale.beach;
                    float t = Mathf.Clamp01(innerDistance / maxInnerDistance);

                    int grassLayers = Mathf.RoundToInt(Mathf.Lerp(mapHight.minGrass, mapHight.maxGrass, t));
                    float noise = Mathf.PerlinNoise((x + mapSeed.heightX) / mapHight.noiseScale, (z + mapSeed.heightZ) / mapHight.noiseScale);
                    int noiseLayers = Mathf.RoundToInt(noise * (mapHight.maxGrass - mapHight.minGrass));

                    grassLayers = Mathf.Clamp(grassLayers + noiseLayers - (mapHight.maxGrass - mapHight.minGrass) / 2, mapHight.minGrass, mapHight.maxGrass);

                    GameObject lastGrassBlock = null;

                    for (int i = 0; i < grassLayers; i++)
                    {
                        float y = mapHight.sandHeight + sandLayers * mapScale.block.y + i * mapScale.block.y;
                        Vector3 blockPos = islandOrigin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);

                        GameObject b;
                        if (i == grassLayers - 1)
                        {
                            b = Instantiate(block.grass, blockPos, Quaternion.identity, islandParent);
                            b.transform.localScale = grassScale;
                            lastGrassBlock = b;
                        }
                        else
                        {
                            b = Instantiate(block.dirt, blockPos, Quaternion.identity, islandParent);
                            b.transform.localScale = dirtScale;
                        }
                    }

                    if (lastGrassBlock != null && mapObjects != null)
                    {
                        float rand = Random.value;
                        float accumulatedChance = 0f;

                        foreach (MapObject mapObj in mapObjects)
                        {
                            if (mapObj.isUnique || mapObj.mapObject == null) continue;
                            if (!UniqueObjectDistance(lastGrassBlock.transform.position, 10f)) continue;

                            accumulatedChance += mapObj.spawnChance;
                            if (rand < accumulatedChance)
                            {
                                Vector3 objPos = lastGrassBlock.transform.position + new Vector3(0, mapScale.block.y, 0);
                                Instantiate(mapObj.mapObject, objPos, Quaternion.identity, islandParent);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    bool IsLakeArea(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        float dist = Vector2.Distance(pos, lakeCenter);
        return dist < mapScale.lake;
    }

    bool IsLakeEdge(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        float dist = Vector2.Distance(pos, lakeCenter);
        return dist < mapScale.lake && dist >= mapScale.lake * 0.5f;
    }

    bool IsLakeInner(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        float dist = Vector2.Distance(pos, lakeCenter);
        return dist < mapScale.lake * 0.5f;
    }

    /// <summary>
    /// 유니크 오브젝트와 충분히 떨어져 있는지 확인
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="minDistance"></param>
    /// <returns></returns>
    bool UniqueObjectDistance(Vector3 pos, float minDistance)
    {
        foreach (var uniquePos in uniqueObjectPositions)
        {
            Vector2 a = new Vector2(pos.x, pos.z);
            Vector2 b = new Vector2(uniquePos.x, uniquePos.z);

            if (Vector2.Distance(a, b) < minDistance) return false;
        }
        return true;
    }

    /// <summary>
    /// 유니크 오브젝트를 설정된 위치 타입에 맞게 배치
    /// </summary>
    void UniqueObjectPlacement()
    {
        Vector2 center = new Vector2(mapScale.total / 2f, mapScale.total / 2f);

        foreach (MapObject mapObj in mapObjects)
        {
            if (!mapObj.isUnique || mapObj.mapObject == null) continue;

            for (int attempt = 0; attempt < 100; attempt++)
            {
                int randX = Random.Range(0, mapScale.total);
                int randZ = Random.Range(0, mapScale.total);
                Vector2 pos2D = new Vector2(randX, randZ);
                float dist = Vector2.Distance(pos2D, center);

                if (IsLakeArea(randX, randZ)) continue;

                bool valid = false;
                switch (mapObj.placementType)
                {
                    // 가운데 쪽 랜덤 위치
                    case UniquePlacementType.CenterArea: valid = (dist >= 10f && dist <= 30f); break;

                    // 가장가리 쪽 랜덤 위치 (수정 필요)
                    case UniquePlacementType.EdgeArea: valid = (dist >= mapScale.total / 2f - 40f); break;

                    // 임의 랜덤 위치 (수정 필요)
                    case UniquePlacementType.Custom: valid = true; break;
                }

                if (!valid) continue;

                float islandNoise = Mathf.PerlinNoise((randX + mapSeed.x) / mapScale.noise, (randZ + mapSeed.z) / mapScale.noise) * 5f;
                float finalDist = dist - islandNoise;

                if (finalDist > mapScale.total / 2f - mapScale.beach) continue;

                float innerDistance = finalDist - (mapScale.total / 2f - mapScale.beach);
                float maxInnerDistance = mapScale.total / 2f - mapScale.beach;
                float t = Mathf.Clamp01(innerDistance / maxInnerDistance);

                int sandLayers = 2;
                int grassLayers = Mathf.RoundToInt(Mathf.Lerp(mapHight.minGrass, mapHight.maxGrass, t));
                float noise = Mathf.PerlinNoise((randX + mapSeed.heightX) / mapHight.noiseScale, (randZ + mapSeed.heightZ) / mapHight.noiseScale);
                int noiseLayers = Mathf.RoundToInt(noise * (mapHight.maxGrass - mapHight.minGrass));

                grassLayers = Mathf.Clamp(grassLayers + noiseLayers - (mapHight.maxGrass - mapHight.minGrass) / 2, mapHight.minGrass, mapHight.maxGrass);

                float y = mapHight.sandHeight + sandLayers * mapScale.block.y + (grassLayers - 1) * mapScale.block.y + mapScale.block.y;

                Vector3 uniquePos = islandParent.position + new Vector3((randX - mapScale.total / 2f) * mapScale.block.x, y,
                    (randZ - mapScale.total / 2f) * mapScale.block.z);

                Instantiate(mapObj.mapObject, uniquePos, Quaternion.identity, islandParent);
                uniqueObjectPositions.Add(uniquePos);
                break;
            }
        }
    }

    /// <summary>
    /// 프리팹의 실제 크기를 측정하여 원하는 크기에 맞도록 스케일 비율을 계산
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="desiredSize"></param>
    /// <returns></returns>
    Vector3 GetScaleToFit(GameObject prefab, Vector3 desiredSize)
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

        float yScale = (originalSize.y < 0.01f) ? 1f : desiredSize.y / originalSize.y;

        return new Vector3(
            desiredSize.x / originalSize.x,
            yScale,
            desiredSize.z / originalSize.z
        );
    }

    void Start()
    {
        Init();
        Create();
    }
}