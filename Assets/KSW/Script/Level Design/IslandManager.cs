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

        mapObjectManager.PlaceUniqueObjects();

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
}