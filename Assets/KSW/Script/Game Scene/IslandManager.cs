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
public class MapHeight
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

public enum UniquePlacementType
{
    CenterArea,
    EdgeArea,
    Custom
}

[System.Serializable]
public class MapObject
{
    [Tooltip("맵 오브젝트")] public GameObject mapObject;
    [Tooltip("스폰 확률")] [Range(0, 1)] public float spawnChance;
    [Tooltip("유니크 오브젝트")] public bool isUnique;
    [Tooltip("배치 타입")] public UniquePlacementType placementType;
}
#endregion

public class IslandManager : MonoBehaviour
{
    [Header("부모 오브젝트 (섬 전체)")]
    public Transform islandParent;

    [Header("블록")]
    public Block block;
    [Header("맵 크기")]
    public MapScale mapScale;
    [Header("맵 높이")]
    public MapHeight mapHeight;
    [Header("시드 값")]
    public MapSeed mapSeed;
    [Header("맵 오브젝트")]
    public MapObject[] mapObjects;

    private Vector2 lakePos;
    private List<Vector3> uniqueObjectPositions = new List<Vector3>();

    void Start()
    {
        Init();
        Create();
    }

    /// <summary>
    /// 시드 값 초기화
    /// </summary>
    public void Init()
    {
        mapSeed.x = Random.Range(0f, 10000f);
        mapSeed.z = Random.Range(0f, 10000f);
        mapSeed.heightX = Random.Range(0f, 10000f);
        mapSeed.heightZ = Random.Range(0f, 10000f);
    }

    /// <summary>
    /// 섬 삭제
    /// </summary>
    public void Clear()
    {
        if (islandParent == null) return;

        for (int i = islandParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(islandParent.GetChild(i).gameObject);

        uniqueObjectPositions.Clear();
    }

    /// <summary>
    /// 섬 생성
    /// </summary>
    public void Create()
    {
        Clear();

        lakePos = new Vector2(
            Random.Range(mapScale.total * 0.2f, mapScale.total * 0.8f),
            Random.Range(mapScale.total * 0.2f, mapScale.total * 0.8f));

        Vector3 origin = islandParent.position + new Vector3(-mapScale.total / 2f * mapScale.block.x, 0, -mapScale.total / 2f * mapScale.block.z);

        Vector3 grassScale = GetScaleToFit(block.grass, mapScale.block);
        Vector3 dirtScale = GetScaleToFit(block.dirt, mapScale.block);
        Vector3 sandScale = GetScaleToFit(block.sand, mapScale.block);
        Vector3 waterScale = GetScaleToFit(block.water, mapScale.block);

        Vector2 center = new Vector2(mapScale.total / 2f, mapScale.total / 2f);

        PlaceUniqueObjects();

        for (int x = 0; x < mapScale.total; x++)
        {
            for (int z = 0; z < mapScale.total; z++)
            {
                Vector2 pos2D = new Vector2(x, z);
                float distToCenter = Vector2.Distance(pos2D, center);
                float noiseOffset = Mathf.PerlinNoise((x + mapSeed.x) / mapScale.noise, (z + mapSeed.z) / mapScale.noise) * 5f;
                float finalDist = distToCenter - noiseOffset;

                if (finalDist > mapScale.total / 2f) continue;

                if (IsLakeEdge(x, z) || IsLakeInner(x, z))
                {
                    PlaceLakeBlocks(origin, x, z, dirtScale, waterScale);
                    continue;
                }

                bool isGrassArea = finalDist <= mapScale.total / 2f - mapScale.beach;

                int sandLayers = CalculateSandLayers(finalDist);
                if (sandLayers > 0 && !isGrassArea) PlaceSand(origin, x, z, sandLayers, sandScale);
                if (isGrassArea) PlaceGrassAndDirt(origin, x, z, sandLayers, grassScale, dirtScale);
            }
        }
    }

    /// <summary>
    /// 모래는 따로 쌓기
    /// </summary>
    /// <param name="finalDist"></param>
    /// <returns></returns>
    int CalculateSandLayers(float finalDist)
    {
        if (finalDist > mapScale.total / 2f - mapScale.beach)
        {
            float beachDepth = mapScale.total / 2f - finalDist;
            float t = Mathf.Clamp01(beachDepth / mapScale.beach);
            return (t > 0.5f) ? 2 : 1;
        }

        else return 2;
    }

    /// <summary>
    /// 호수 블록 생성
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="dirtScale"></param>
    /// <param name="waterScale"></param>
    void PlaceLakeBlocks(Vector3 origin, int x, int z, Vector3 dirtScale, Vector3 waterScale)
    {
        int dirtLayers = IsLakeInner(x, z) ? 1 : 2;
        float lakeWaterHeight = mapHeight.sandHeight + mapScale.block.y * 2.5f;

        for (int i = 0; i < dirtLayers; i++)
        {
            float y = mapHeight.sandHeight + i * mapScale.block.y;
            Vector3 pos = origin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);
            GameObject dirt = Instantiate(block.dirt, pos, Quaternion.identity, islandParent);
            dirt.transform.localScale = dirtScale;
        }

        Vector3 waterPos = origin + new Vector3(x * mapScale.block.x, lakeWaterHeight, z * mapScale.block.z);
        GameObject water = Instantiate(block.water, waterPos, Quaternion.identity, islandParent);
        water.transform.localScale = waterScale;
    }

    /// <summary>
    /// 모래 블록 생성
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="layers"></param>
    /// <param name="sandScale"></param>
    void PlaceSand(Vector3 origin, int x, int z, int layers, Vector3 sandScale)
    {
        float y = mapHeight.sandHeight + (layers - 1) * mapScale.block.y;
        Vector3 sandPos = origin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);
        GameObject sandBlock = Instantiate(block.sand, sandPos, Quaternion.identity, islandParent);
        sandBlock.transform.localScale = sandScale;
    }

    /// <summary>
    /// 잔디 & 흙 배치
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="sandLayers"></param>
    /// <param name="grassScale"></param>
    /// <param name="dirtScale"></param>
    void PlaceGrassAndDirt(Vector3 origin, int x, int z, int sandLayers, Vector3 grassScale, Vector3 dirtScale)
    {
        float innerDist = Vector2.Distance(new Vector2(x, z), new Vector2(mapScale.total / 2f, mapScale.total / 2f)) - (mapScale.total / 2f - mapScale.beach);
        float maxInnerDist = mapScale.total / 2f - mapScale.beach;
        float t = Mathf.Clamp01(innerDist / maxInnerDist);

        int grassLayersBase = Mathf.RoundToInt(Mathf.Lerp(mapHeight.minGrass, mapHeight.maxGrass, t));
        float noise = Mathf.PerlinNoise((x + mapSeed.heightX) / mapHeight.noiseScale, (z + mapSeed.heightZ) / mapHeight.noiseScale);
        int noiseLayers = Mathf.RoundToInt(noise * (mapHeight.maxGrass - mapHeight.minGrass));

        int grassLayers = Mathf.Clamp(grassLayersBase + noiseLayers - (mapHeight.maxGrass - mapHeight.minGrass) / 2, mapHeight.minGrass, mapHeight.maxGrass);

        GameObject lastGrassBlock = null;

        for (int i = 0; i < grassLayers; i++)
        {
            float y = mapHeight.sandHeight + sandLayers * mapScale.block.y + i * mapScale.block.y;
            Vector3 blockPos = origin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);

            GameObject blockToPlace = (i == grassLayers - 1) ? block.grass : block.dirt;
            GameObject placedBlock = Instantiate(blockToPlace, blockPos, Quaternion.identity, islandParent);
            placedBlock.transform.localScale = (i == grassLayers - 1) ? grassScale : dirtScale;

            if (i == grassLayers - 1) lastGrassBlock = placedBlock;
        }

        TryPlaceMapObjects(lastGrassBlock);
    }

    /// <summary>
    /// 확률적으로 잔디 위에 오브젝트 배치
    /// </summary>
    /// <param name="grassBlock"></param>
    void TryPlaceMapObjects(GameObject grassBlock)
    {
        if (grassBlock == null || mapObjects == null) return;

        float rand = Random.value;
        float cumulativeChance = 0f;

        foreach (var mapObj in mapObjects)
        {
            if (mapObj.isUnique || mapObj.mapObject == null) continue;
            if (!IsFarFromUniqueObjects(grassBlock.transform.position, 10f)) continue;

            cumulativeChance += mapObj.spawnChance;
            if (rand < cumulativeChance)
            {
                Vector3 objPos = grassBlock.transform.position + new Vector3(0, mapScale.block.y, 0);
                Instantiate(mapObj.mapObject, objPos, Quaternion.identity, islandParent);
                break;
            }
        }
    }

    /// <summary>
    /// 유니크 오브젝트들과 일정 거리 이상 떨어져 있는지 검사
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="minDist"></param>
    /// <returns></returns>
    bool IsFarFromUniqueObjects(Vector3 pos, float minDist)
    {
        Vector2 pos2D = new Vector2(pos.x, pos.z);

        foreach (var uniquePos in uniqueObjectPositions)
        {
            Vector2 uniquePos2D = new Vector2(uniquePos.x, uniquePos.z);
            if (Vector2.Distance(pos2D, uniquePos2D) < minDist) return false;
        }

        return true;
    }

    /// <summary>
    /// 유니크 오브젝트 배치
    /// </summary>
    void PlaceUniqueObjects()
    {
        Vector2 center = new Vector2(mapScale.total / 2f, mapScale.total / 2f);

        foreach (var mapObj in mapObjects)
        {
            if (!mapObj.isUnique || mapObj.mapObject == null) continue;

            for (int attempt = 0; attempt < 100; attempt++)
            {
                int randX = Random.Range(0, mapScale.total);
                int randZ = Random.Range(0, mapScale.total);
                Vector2 pos2D = new Vector2(randX, randZ);
                float dist = Vector2.Distance(pos2D, center);

                if (IsLakeArea(randX, randZ)) continue;

                bool valid = mapObj.placementType switch
                {
                    UniquePlacementType.CenterArea => dist >= 10f && dist <= 30f,
                    UniquePlacementType.EdgeArea => dist >= mapScale.total / 2f - 40f,
                    UniquePlacementType.Custom => true, _ => false
                };

                if (!valid) continue;

                float noiseOffset = Mathf.PerlinNoise((randX + mapSeed.x) / mapScale.noise, (randZ + mapSeed.z) / mapScale.noise) * 5f;
                float finalDist = dist - noiseOffset;

                if (finalDist > mapScale.total / 2f - mapScale.beach) continue;

                float innerDist = finalDist - (mapScale.total / 2f - mapScale.beach);
                float maxInnerDist = mapScale.total / 2f - mapScale.beach;
                float t = Mathf.Clamp01(innerDist / maxInnerDist);

                int sandLayers = 2;
                int grassLayersBase = Mathf.RoundToInt(Mathf.Lerp(mapHeight.minGrass, mapHeight.maxGrass, t));
                float noise = Mathf.PerlinNoise((randX + mapSeed.heightX) / mapHeight.noiseScale, (randZ + mapSeed.heightZ) / mapHeight.noiseScale);
                int noiseLayers = Mathf.RoundToInt(noise * (mapHeight.maxGrass - mapHeight.minGrass));

                int grassLayers = Mathf.Clamp(grassLayersBase + noiseLayers - (mapHeight.maxGrass - mapHeight.minGrass) / 2, mapHeight.minGrass, mapHeight.maxGrass);

                float y = mapHeight.sandHeight + sandLayers * mapScale.block.y + (grassLayers - 1) * mapScale.block.y + mapScale.block.y;

                Vector3 uniquePos = islandParent.position + new Vector3((randX - mapScale.total / 2f) * mapScale.block.x, y, (randZ - mapScale.total / 2f) * mapScale.block.z);

                Instantiate(mapObj.mapObject, uniquePos, Quaternion.identity, islandParent);
                uniqueObjectPositions.Add(uniquePos);
                break;
            }
        }
    }

    bool IsLakeArea(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        return Vector2.Distance(pos, lakePos) < mapScale.lake;
    }

    bool IsLakeEdge(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        float dist = Vector2.Distance(pos, lakePos);
        return dist < mapScale.lake && dist >= mapScale.lake * 0.5f;
    }

    bool IsLakeInner(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        return Vector2.Distance(pos, lakePos) < mapScale.lake * 0.5f;
    }

    /// <summary>
    /// 프리팹 실제 크기에 맞춰 스케일 조정
    /// </summary>
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
}