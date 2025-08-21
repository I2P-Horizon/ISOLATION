using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데이터 정의
/// </summary>
[System.Serializable]
public class MapObject
{
    [Tooltip("맵 오브젝트")] public GameObject mapObject;
    [Tooltip("스폰 확률")][Range(0, 1)] public float spawnChance;
    [Tooltip("유니크 오브젝트")] public bool isUnique;
}

/// <summary>
/// 오브젝트 배치 관리
/// </summary>
public class MapObjectManager
{
    private Transform islandParent;
    private MapObject[] mapObjects;
    private MapScale mapScale;
    private MapHeight mapHeight;
    private MapSeed mapSeed;
    private Lake lake;

    public List<Vector3> uniqueObjectPositions = new List<Vector3>();
    private bool uniquePlaced = false;

    public MapObjectManager(Transform parent, MapObject[] objects, MapScale scale, MapHeight height, MapSeed seed, Lake lakeRef)
    {
        islandParent = parent;
        mapObjects = objects;
        mapScale = scale;
        mapHeight = height;
        mapSeed = seed;
        lake = lakeRef;
    }

    /// <summary>
    /// 유니크 오브젝트 배치
    /// </summary>
    /// <param name="pos"></param>
    public void PlaceUniqueAt(Vector3 pos)
    {
        if (uniquePlaced) return;

        foreach (var mapObj in mapObjects)
        {
            if (mapObj.mapObject == null || !mapObj.isUnique) continue;

            GameObject obj = GameObject.Instantiate(
                mapObj.mapObject,
                pos,
                Quaternion.identity,
                islandParent
            );

            uniquePlaced = true;
            uniqueObjectPositions.Add(obj.transform.position);
            break;
        }
    }

    /// <summary>
    /// 일반 오브젝트 배치
    /// </summary>
    /// <param name="grassBlock"></param>
    public void TryPlaceMapObjects(GameObject grassBlock)
    {
        if (grassBlock == null || mapObjects == null) return;

        float rand = Random.value;
        float cumulativeChance = 0f;

        foreach (var mapObj in mapObjects)
        {
            if (mapObj.mapObject == null || mapObj.isUnique) continue;

            cumulativeChance += mapObj.spawnChance;
            if (rand < cumulativeChance)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-mapScale.block.x / 2f, mapScale.block.x / 2f),
                    0,
                    Random.Range(-mapScale.block.z / 2f, mapScale.block.z / 2f)
                );

                GameObject.Instantiate(
                    mapObj.mapObject,
                    grassBlock.transform.position + offset + new Vector3(0, mapScale.block.y, 0),
                    Quaternion.identity,
                    islandParent
                );
                break;
            }
        }
    }
}