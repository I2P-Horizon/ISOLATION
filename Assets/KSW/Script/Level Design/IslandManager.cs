using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [Header("부모 오브젝트 (섬 전체)")] public Transform islandParent;

    [Header("블록")] public Block block;
    [Header("맵 크기")] public MapScale mapScale;
    [Header("맵 높이")] public MapHeight mapHeight;
    [Header("시드 값")] public MapSeed mapSeed;
    [Header("맵 오브젝트")] public MapObject[] mapObjects;

    [Header("정글")] public Jungle jungle;
    [Header("해변")] public Beach beach;
    [Header("호수")] public Lake lake;
    [Header("산")] public Mountain mountain;

    private List<Vector3> uniqueObjectPositions = new List<Vector3>();

    void Awake()
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
        lake.Create();
        mountain.Create();

        Vector3 origin = islandParent.position + new Vector3(-mapScale.total / 2f * mapScale.block.x, 0, -mapScale.total / 2f * mapScale.block.z);

        Vector3 grassScale = block.GetScaleToFit(jungle.grass, mapScale.block);
        Vector3 dirtScale = block.GetScaleToFit(jungle.dirt, mapScale.block);
        Vector3 sandScale = block.GetScaleToFit(beach.sand, mapScale.block);
        Vector3 waterScale = block.GetScaleToFit(lake.water, mapScale.block);

        Vector2 center = new Vector2(mapScale.total / 2f, mapScale.total / 2f);

        Transform grassParent = new GameObject("Grass Parent").transform;
        Transform dirtParent = new GameObject("Dirt Parent").transform;
        Transform sandParent = new GameObject("Sand Parent").transform;
        Transform waterParent = new GameObject("Water Parent").transform;

        grassParent.SetParent(islandParent);
        dirtParent.SetParent(islandParent);
        sandParent.SetParent(islandParent);
        waterParent.SetParent(islandParent);

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

                if (lake.IsLakeEdge(x, z) || lake.IsLakeInner(x, z))
                {
                    lake.PlaceLakeBlocks(origin, x, z, dirtScale, waterScale, dirtParent, waterParent);
                    continue;
                }

                bool isGrassArea = finalDist <= mapScale.total / 2f - mapScale.beach;

                int sandLayers = beach.CalculateSandLayers(finalDist);
                if (sandLayers > 0 && !isGrassArea) beach.PlaceSand(origin, x, z, sandLayers, sandScale, sandParent);
                if (isGrassArea) jungle.PlaceGrassAndDirt(origin, x, z, sandLayers, grassScale, dirtScale, grassParent, dirtParent);
            }
        }

        GetComponent<CombineMesh>().Combine(grassParent, jungle.grass.GetComponentInChildren<MeshRenderer>().sharedMaterial, "Merged Grass");
        GetComponent<CombineMesh>().Combine(dirtParent, jungle.dirt.GetComponentInChildren<MeshRenderer>().sharedMaterial, "Merged Dirt");
        GetComponent<CombineMesh>().Combine(sandParent, beach.sand.GetComponentInChildren<MeshRenderer>().sharedMaterial, "Merged Sand");
    }

    /// <summary>
    /// 확률적으로 잔디 위에 오브젝트 배치
    /// </summary>
    /// <param name="grassBlock"></param>
    public void TryPlaceMapObjects(GameObject grassBlock)
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

                if (lake.IsLakeArea(randX, randZ)) continue;

                bool valid = mapObj.placementType switch
                {
                    UniquePlacementType.CenterArea => dist >= 10f && dist <= 30f,
                    UniquePlacementType.EdgeArea => dist >= mapScale.total / 2f - 40f,
                    UniquePlacementType.Custom => true,
                    _ => false
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
}