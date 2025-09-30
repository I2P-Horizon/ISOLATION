using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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