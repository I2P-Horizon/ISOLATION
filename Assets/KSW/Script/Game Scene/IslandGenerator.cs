using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    [Header("블록 프리팹")]
    public GameObject grassBlock;
    public GameObject dirtBlock;
    public GameObject sandBlock;

    [Header("섬 설정")]
    [Tooltip("섬의 크기")] public int size = 250;
    [Tooltip("해변의 크기")] public float beachSize = 15f;
    [Tooltip("노이즈 규모")] public float noiseScale = 5f;

    [Header("지형 계단 설정")]
    [Tooltip("최대 잔디 높이")] public int maxGrassHeight = 4;
    [Tooltip("최소 잔디 높이")] public int minGrassHeight = 1;
    [Tooltip("최대 노이즈 규모")] public float heightNoiseScale = 8f;

    [Header("블록 크기 설정")]
    public Vector3 targetSize = new Vector3(1, 1, 1);

    [Header("블록 높이 기준선")]
    public float sandHeight = 0f;

    [Header("기본 나무 배치")]
    public GameObject treePrefab;
    [Tooltip("배치될 확률")] public float treeSpawnChance = 0.04f;

    [Header("파인애플 줄기 배치")]
    public GameObject pineapplePrefab;
    [Tooltip("배치될 확률")] public float pineappleSpawnChance = 0.005f;

    [Header("부모 오브젝트 (섬 전체)")]
    public Transform islandParent;

    [Header("랜덤 시드 값")]
    [SerializeField] private float seedX;
    [SerializeField] private float seedZ;
    [SerializeField] private float heightSeedX;
    [SerializeField] private float heightSeedZ;

    void Start()
    {
        // 시드 랜덤 초기화
        seedX = Random.Range(0f, 10000f);
        seedZ = Random.Range(0f, 10000f);
        heightSeedX = Random.Range(0f, 10000f);
        heightSeedZ = Random.Range(0f, 10000f);

        Vector3 islandOrigin = islandParent.position + new Vector3(-size / 2f * targetSize.x, 0, -size / 2f * targetSize.z);

        Vector3 grassScale = GetScaleToFit(grassBlock, targetSize);
        Vector3 dirtScale = GetScaleToFit(dirtBlock, targetSize);
        Vector3 sandScale = GetScaleToFit(sandBlock, targetSize);

        Vector2 center = new Vector2(size / 2f, size / 2f);

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                Vector2 pos2D = new Vector2(x, z);
                float dist = Vector2.Distance(pos2D, center);

                float islandNoise = Mathf.PerlinNoise((x + seedX) / noiseScale, (z + seedZ) / noiseScale) * 5f;
                float finalDist = dist - islandNoise;

                if (finalDist > size / 2f) continue;

                int sandLayers = 1;
                if (finalDist <= size / 2f && finalDist > size / 2f - beachSize)
                {
                    float beachDepth = size / 2f - finalDist;
                    float t = Mathf.Clamp01(beachDepth / beachSize);
                    sandLayers = (t > 0.5f) ? 2 : 1;
                }

                else if (finalDist <= size / 2f - beachSize) sandLayers = 2;

                bool isGrass = finalDist <= size / 2f - beachSize;
                bool placeSand = true;

                if (isGrass)
                {
                    float innerDistance = finalDist - (size / 2f - beachSize);
                    float maxInnerDistance = size / 2f - beachSize;
                    float t = Mathf.Clamp01(innerDistance / maxInnerDistance);

                    int grassLayers = Mathf.RoundToInt(Mathf.Lerp(minGrassHeight, maxGrassHeight, t));
                    float noise = Mathf.PerlinNoise((x + heightSeedX) / heightNoiseScale, (z + heightSeedZ) / heightNoiseScale);
                    int noiseLayers = Mathf.RoundToInt(noise * (maxGrassHeight - minGrassHeight));

                    grassLayers = Mathf.Clamp(grassLayers + noiseLayers - (maxGrassHeight - minGrassHeight) / 2, minGrassHeight, maxGrassHeight);

                    if (grassLayers > 0) placeSand = false;
                }

                // 모래 쌓기
                // + 맨 위층에만 모래가 생성되도록
                if (placeSand && sandLayers > 0)
                {
                    int topIndex = sandLayers - 1;
                    float y = sandHeight + topIndex * targetSize.y;

                    Vector3 sandPos = islandOrigin + new Vector3(x * targetSize.x, y, z * targetSize.z);

                    GameObject sand = Instantiate(sandBlock, sandPos, Quaternion.identity, islandParent);
                    sand.transform.localScale = sandScale;
                }

                if (finalDist <= size / 2f - beachSize)
                {
                    float innerDistance = finalDist - (size / 2f - beachSize);
                    float maxInnerDistance = size / 2f - beachSize;
                    float t = Mathf.Clamp01(innerDistance / maxInnerDistance);

                    int grassLayers = Mathf.RoundToInt(Mathf.Lerp(minGrassHeight, maxGrassHeight, t));

                    float noise = Mathf.PerlinNoise((x + heightSeedX) / heightNoiseScale, (z + heightSeedZ) / heightNoiseScale);
                    int noiseLayers = Mathf.RoundToInt(noise * (maxGrassHeight - minGrassHeight));

                    grassLayers = Mathf.Clamp(grassLayers + noiseLayers - (maxGrassHeight - minGrassHeight) / 2, minGrassHeight, maxGrassHeight);

                    GameObject lastGrassBlock = null;

                    for (int i = 0; i < grassLayers; i++)
                    {
                        float y = sandHeight + sandLayers * targetSize.y + i * targetSize.y;
                        Vector3 blockPos = islandOrigin + new Vector3(x * targetSize.x, y, z * targetSize.z);

                        GameObject block;

                        if (i == grassLayers - 1)
                        {
                            block = Instantiate(grassBlock, blockPos, Quaternion.identity, islandParent);
                            block.transform.localScale = grassScale;
                            lastGrassBlock = block;
                        }

                        else
                        {
                            block = Instantiate(dirtBlock, blockPos, Quaternion.identity, islandParent);
                            block.transform.localScale = dirtScale;
                        }
                    }

                    // 오브젝트 배치
                    if (lastGrassBlock != null)
                    {
                        float rand = Random.value;

                        // 기본 나무 배치
                        if (treePrefab != null && rand < treeSpawnChance)
                        {
                            Vector3 treePos = lastGrassBlock.transform.position + new Vector3(0, targetSize.y, 0);
                            Instantiate(treePrefab, treePos, Quaternion.identity, islandParent);
                        }

                        // 파인애플 줄기 배치
                        else if (pineapplePrefab != null && rand >= treeSpawnChance && rand < treeSpawnChance + pineappleSpawnChance)
                        {
                            Vector3 pineapplePos = lastGrassBlock.transform.position + new Vector3(0, targetSize.y, 0);
                            Instantiate(pineapplePrefab, pineapplePos, Quaternion.identity, islandParent);
                        }
                    }
                }
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

        if (originalSize == Vector3.zero)
        {
            DestroyImmediate(temp);
            return Vector3.one;
        }

        DestroyImmediate(temp);

        return new Vector3(
            desiredSize.x / originalSize.x,
            desiredSize.y / originalSize.y,
            desiredSize.z / originalSize.z
        );
    }
}