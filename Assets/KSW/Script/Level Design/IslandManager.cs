using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [Header("부모 오브젝트 (섬 전체)")] public Transform islandParent;

    [Header("블록")] public Block block = new Block();
    [Header("맵 크기")] public MapScale mapScale;
    [Header("맵 높이")] public MapHeight mapHeight;
    [Header("시드 값")] public MapSeed mapSeed;
    [Header("맵 오브젝트")] public MapObject[] mapObjects;

    [Header("정글")] public Jungle jungle;
    [Header("해변")] public Beach beach;
    [Header("호수")] public Lake lake;
    [Header("산")] public Mountain mountain;

    private MapObjectManager mapObjectManager;

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
        jungle.SetMountain(mountain);

        mapObjectManager = new MapObjectManager(islandParent, mapObjects, mapScale, mapHeight, mapSeed, lake);
        jungle.SetMapObjectManager(mapObjectManager);

        Vector3 origin = islandParent.position + new Vector3(-mapScale.total / 2f * mapScale.block.x, 0, -mapScale.total / 2f * mapScale.block.z);
        Vector2 center = new Vector2(mapScale.total / 2f, mapScale.total / 2f);

        mapObjectManager.PlaceUniqueObjects();

        int chunkSize = 64;
        int chunksX = Mathf.CeilToInt((float)mapScale.total / chunkSize);
        int chunksZ = Mathf.CeilToInt((float)mapScale.total / chunkSize);

        for (int cx = 0; cx < chunksX; cx++)
        {
            for (int cz = 0; cz < chunksZ; cz++)
            {
                Transform chunkParent = new GameObject($"Chunk_{cx}_{cz}").transform;
                chunkParent.SetParent(islandParent);

                Transform grassParent = new GameObject("Grass").transform;
                Transform dirtParent = new GameObject("Dirt").transform;
                Transform sandParent = new GameObject("Sand").transform;
                Transform waterParent = new GameObject("Water").transform;

                grassParent.SetParent(chunkParent);
                dirtParent.SetParent(chunkParent);
                sandParent.SetParent(chunkParent);

                int startX = cx * chunkSize;
                int startZ = cz * chunkSize;
                int endX = Mathf.Min(startX + chunkSize, mapScale.total);
                int endZ = Mathf.Min(startZ + chunkSize, mapScale.total);

                for (int x = startX; x < endX; x++)
                {
                    for (int z = startZ; z < endZ; z++)
                    {
                        Vector2 pos2D = new Vector2(x, z);
                        float distToCenter = Vector2.Distance(pos2D, center);
                        float noiseOffset = Mathf.PerlinNoise((x + mapSeed.x) / mapScale.noise, (z + mapSeed.z) / mapScale.noise) * 5f;
                        float finalDist = distToCenter - noiseOffset;

                        if (finalDist > mapScale.total / 2f) continue;

                        bool isGrassArea = finalDist <= mapScale.total / 2f - mapScale.beach;
                        int sandLayers = beach.CalculateSandLayers(finalDist);

                        Vector3 grassScale = block.GetScaleToFit(jungle.grass, mapScale.block);
                        Vector3 dirtScale = block.GetScaleToFit(jungle.dirt, mapScale.block);
                        Vector3 sandScale = block.GetScaleToFit(beach.sand, mapScale.block);
                        Vector3 waterScale = block.GetScaleToFit(lake.water, mapScale.block);

                        if (lake.IsLakeEdge(x, z) || lake.IsLakeInner(x, z))
                        {
                            lake.PlaceLakeBlocks(origin, x, z, dirtScale, waterScale, dirtParent, waterParent);
                            continue;
                        }

                        if (sandLayers > 0 && !isGrassArea) beach.PlaceSand(origin, x, z, sandLayers, sandScale, sandParent);
                        if (isGrassArea) jungle.PlaceGrassAndDirt(origin, x, z, sandLayers, grassScale, dirtScale, grassParent, dirtParent);
                    }
                }

                CombineMesh combine = GetComponent<CombineMesh>();
                combine.Combine(grassParent, jungle.grass.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"Chunk_{cx}_{cz}_Grass");
                combine.Combine(dirtParent, jungle.dirt.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"Chunk_{cx}_{cz}_Dirt");
                combine.Combine(sandParent, beach.sand.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"Chunk_{cx}_{cz}_Sand");
            }
        }
    }
}