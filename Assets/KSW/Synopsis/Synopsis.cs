using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synopsis : MonoBehaviour
{
    [SerializeField] private GameObject water;
    private CombineMesh combineMesh;

    void Start()
    {
        combineMesh = GetComponent<CombineMesh>();

        // 전체 물 타일 개수
        int tileCountX = 500;
        int tileCountZ = 500;
        int chunkSize = 32; // 청크 단위

        // 프리팹 스케일 계산
        Vector3 scale = GetScaleToFit(water, Vector3.one);

        // 몇 개의 청크가 필요한지 계산
        int chunksX = Mathf.CeilToInt(tileCountX / (float)chunkSize);
        int chunksZ = Mathf.CeilToInt(tileCountZ / (float)chunkSize);

        for (int cx = 0; cx < chunksX; cx++)
        {
            for (int cz = 0; cz < chunksZ; cz++)
            {
                // 청크 부모 생성
                GameObject chunkParent = new GameObject($"WaterChunk_{cx}_{cz}");
                chunkParent.transform.position = Vector3.zero;

                // 청크 내 타일 생성
                for (int x = 0; x < chunkSize; x++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        int globalX = cx * chunkSize + x;
                        int globalZ = cz * chunkSize + z;

                        if (globalX >= tileCountX || globalZ >= tileCountZ) continue; // 범위 벗어나면 스킵

                        GameObject waterTile = Instantiate(water, chunkParent.transform);

                        // 부모 중심 기준으로 배치
                        float offsetX = tileCountX / 2f;
                        float offsetZ = tileCountZ / 2f;

                        waterTile.transform.localPosition = new Vector3(globalX - offsetX, 0, globalZ - offsetZ);
                        waterTile.transform.localScale = scale;
                    }
                }

                // 청크 메쉬 병합
                if (combineMesh != null)
                {
                    Material mat = water.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                    combineMesh.Combine(chunkParent.transform, mat, $"Merged_Chunk_{cx}_{cz}_Water");
                }
            }
        }
    }

    public Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
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