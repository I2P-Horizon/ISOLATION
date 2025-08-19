using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 데이터 정의 */
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
    [Tooltip("스폰 확률")][Range(0, 1)] public float spawnChance;
    [Tooltip("유니크 오브젝트")] public bool isUnique;
    [Tooltip("배치 타입")] public UniquePlacementType placementType;
}

/* 오브젝트 배치 관리 */
public class MapObjectManager
{
    private Transform islandParent;
    private MapObject[] mapObjects;
    private MapScale mapScale;
    private MapHeight mapHeight;
    private MapSeed mapSeed;
    private Lake lake;

    public List<Vector3> uniqueObjectPositions = new List<Vector3>();

    public MapObjectManager(Transform parent, MapObject[] objects, MapScale scale, MapHeight height, MapSeed seed, Lake lakeRef)
    {
        islandParent = parent;
        mapObjects = objects;
        mapScale = scale;
        mapHeight = height;
        mapSeed = seed;
        lake = lakeRef;
    }

    public void PlaceUniqueObjects()
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

                GameObject.Instantiate(mapObj.mapObject, uniquePos, Quaternion.identity, islandParent);
                uniqueObjectPositions.Add(uniquePos);
                break;
            }
        }
    }

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
                GameObject.Instantiate(mapObj.mapObject, objPos, Quaternion.identity, islandParent);
                break;
            }
        }
    }

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
}