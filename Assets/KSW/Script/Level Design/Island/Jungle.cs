using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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