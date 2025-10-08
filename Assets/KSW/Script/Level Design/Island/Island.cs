using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class IslandGenerator : Island, IGeneratable
{
    #region Island Data
    private Temple temple;
    private MapObject mapObject;
    private BlockData blockData;

    public Transform Root { get; private set; }
    public Transform pos;
    public Transform player;

    [SerializeField] private RockArea rockArea;

    [HideInInspector] public List<Vector3> sandPositions = new List<Vector3>();
    [HideInInspector] public List<Vector3> TopGrassPositions { get; private set; } = new List<Vector3>();

    public void Set(Temple temple, BlockData blockData, MapObject mapObject)
    {
        this.temple = temple; this.blockData = blockData;
    }
    #endregion

    protected override IEnumerator Generate()
    {
        Root = new GameObject("Island").transform;
        Root.SetParent(pos);

        Vector3 targetSize = Vector3.one;
        foreach (var p in new[] { blockData.grassBlock, blockData.dirtBlock, blockData.sandBlock, blockData.waterPlane, blockData.templeFloorBlock, blockData.swampBlock, blockData.rockBlock })
            if (p != null && !blockData.scaleCache.ContainsKey(p)) blockData.scaleCache[p] = blockData.GetScaleToFit(p, targetSize);

        float halfW = width / 2f;
        float halfH = height / 2f;

        int chunkXCount = Mathf.CeilToInt((float)width / chunkSize);
        int chunkZCount = Mathf.CeilToInt((float)height / chunkSize);

        int totalBlocks = chunkXCount * chunkZCount * chunkSize * chunkSize;
        int blocksGenerated = 0;

        float waterY = seaLevel + 1.5f;
        sandPositions.Clear();
        TopGrassPositions.Clear();

        for (int cx = 0; cx < chunkXCount; cx++)
        {
            for (int cz = 0; cz < chunkZCount; cz++)
            {
                Transform chunkParent = new GameObject($"Chunk_{cx}_{cz}").transform;
                chunkParent.SetParent(Root);

                Transform grassParent = new GameObject("Grass").transform; grassParent.SetParent(chunkParent);
                Transform dirtParent = new GameObject("Dirt").transform; dirtParent.SetParent(chunkParent);
                Transform sandParent = new GameObject("Sand").transform; sandParent.SetParent(chunkParent);
                Transform waterParent = new GameObject("Water").transform; waterParent.SetParent(chunkParent);
                Transform templeParent = new GameObject("Temple").transform; templeParent.SetParent(chunkParent);
                Transform swampParent = new GameObject("Swamp").transform; swampParent.SetParent(chunkParent);
                Transform rockParent = new GameObject("Rock").transform; rockParent.SetParent(chunkParent);

                for (int x = 0; x < chunkSize; x++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        int worldX = cx * chunkSize + x - (int)halfW;
                        int worldZ = cz * chunkSize + z - (int)halfH;

                        float dist = Mathf.Sqrt(worldX * worldX + worldZ * worldZ) / radius;
                        float noiseMask = Mathf.PerlinNoise((worldX + seed) / scale, (worldZ + seed) / scale);
                        float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.45f);
                        islandMask = Mathf.Pow(islandMask, falloffPower);

                        float heightNoise = 0f; float amplitude = 1f; float frequency = 1f; float maxAmp = 0f;

                        for (int o = 0; o < octaves; o++)
                        {
                            float sampleX = (worldX + seed) / scale * frequency;
                            float sampleZ = (worldZ + seed) / scale * frequency;
                            heightNoise += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                            maxAmp += amplitude;
                            amplitude *= persistence;
                            frequency *= lacunarity;
                        }

                        if (maxAmp > 0f) heightNoise /= maxAmp;

                        int sandLayers = 0;
                        float distanceToEdge = radius - dist * radius * islandMask;

                        if (distanceToEdge < beachWidth)
                        {
                            float t = Mathf.Clamp01(distanceToEdge / beachWidth);
                            sandLayers = (t > 0.5f) ? 2 : 1;
                        }

                        Vector3 posXZ = new Vector3(worldX, 0, worldZ);
                        bool inTempleArea = temple.exists && Vector3.Distance(new Vector3(temple.pos.x, 0, temple.pos.z), posXZ) <= radius;

                        int landHeight = Mathf.RoundToInt(heightNoise * islandMask * maxHeight);
                        float floorY = Mathf.Max(temple.pos.y, temple.scaleY + 2);

                        if (islandMask > 0f && landHeight > seaLevel)
                        {
                            if (inTempleArea)
                            {
                                blockData.PlaceBlock(blockData.templeFloorBlock, new Vector3(worldX, floorY, worldZ), templeParent);
                                for (int y = seaLevel + 1; y < floorY; y++) blockData.PlaceBlock(blockData.dirtBlock, new Vector3(worldX, y, worldZ), templeParent);
                                continue;
                            }

                            bool placed = false;

                            ///* 늪지대 생성 */
                            //if (!placed && landHeight > sandLayers)
                            //{
                            //    float swampMask = Mathf.PerlinNoise(worldX / 10f, worldZ / 10f);
                            //    if (swampMask > 0.6f && dist > shape.radius * 0.5f)
                            //    {
                            //        Vector3 swampPos = new Vector3(worldX, landHeight, worldZ);
                            //        blockData.PlaceBlock(blockData.swampBlock, swampPos, swampParent);
                            //        placed = true;
                            //    }
                            //}

                            rockArea.Set(temple, mapObject, blockData);
                            rockArea.SetParentObject(chunkParent);

                            /* 돌 영역 생성 */
                            if (!placed && landHeight > sandLayers)
                            {
                                /* worldX/worldZ 좌표를 80으로 나누어 노이즈의 스케일을 크게 만들어 큰 영역 단위로 돌 생성 */
                                float rockMask = Mathf.PerlinNoise(worldX / 80f, worldZ / 80f);
                                if (rockMask > 0.85f)
                                {
                                    Vector3 rockPos = new Vector3(worldX, landHeight, worldZ);
                                    rockArea.Generate(rockPos, blockData.rockBlock, rockArea.rockObject);

                                    /* 돌을 생성했으므로 placed를 true로 변경 */
                                    /* 이후 모래나 잔디를 배치하는 로직에서는 이 좌표를 건너뜀. */
                                    placed = true;
                                }
                            }

                            /* 모래/잔디 처리 */
                            if (!placed)
                            {
                                if (sandLayers > 0)
                                {
                                    for (int y = 0; y < sandLayers; y++)
                                    {
                                        Vector3 pos = new Vector3(worldX, y, worldZ);
                                        blockData.PlaceBlock(blockData.sandBlock, pos, sandParent);
                                        sandPositions.Add(pos);
                                    }

                                    if (landHeight > sandLayers)
                                    {
                                        Vector3 grassPos = new Vector3(worldX, sandLayers, worldZ);
                                        blockData.PlaceBlock(blockData.grassBlock, grassPos, grassParent);
                                        TopGrassPositions.Add(grassPos);

                                        if (inTempleArea == false && Vector3.Distance(new Vector3(temple.pos.x, 0, temple.pos.z), new Vector3(worldX, 0, worldZ)) <= radius + 1f)
                                        {
                                            for (int i = 0; i < 6; i++)
                                            {
                                                Vector3 dirtPos = new Vector3(worldX, sandLayers - i, worldZ);
                                                blockData.PlaceBlock(blockData.dirtBlock, dirtPos, dirtParent);
                                            }
                                        }
                                    }
                                }

                                else
                                {
                                    GameObject topBlock = (landHeight == seaLevel + 1) ? blockData.sandBlock : blockData.grassBlock;
                                    Vector3 pos = new Vector3(worldX, landHeight, worldZ);
                                    blockData.PlaceBlock(topBlock, pos, topBlock == blockData.grassBlock ? grassParent : sandParent);

                                    if (topBlock == blockData.grassBlock)
                                    {
                                        TopGrassPositions.Add(pos);

                                        if (inTempleArea == false && Vector3.Distance(new Vector3(temple.pos.x, 0, temple.pos.z), new Vector3(worldX, 0, worldZ)) <= radius + 1f)
                                        {
                                            for (int i = 1; i <= 6; i++)
                                            {
                                                Vector3 dirtPos = new Vector3(worldX, landHeight - i, worldZ);
                                                blockData.PlaceBlock(blockData.dirtBlock, dirtPos, dirtParent);
                                            }
                                        }
                                    }

                                    if (topBlock == blockData.sandBlock && pos.y > seaLevel) sandPositions.Add(pos);
                                }
                            }
                        }

                        if (islandMask > 0f && landHeight <= seaLevel)
                        {
                            Vector3 dirtUnderWaterPos = new Vector3(worldX, waterY - 1.5f, worldZ);
                            blockData.PlaceBlock(blockData.dirtBlock, dirtUnderWaterPos, dirtParent);
                        }

                        blockData.PlaceBlock(blockData.waterPlane, new Vector3(worldX, waterY, worldZ), waterParent);

                        blocksGenerated++;
                        IslandManager.generationProgress = (float)blocksGenerated / totalBlocks;
                    }
                }

                CombineMesh combiner = MonoBehaviour.FindAnyObjectByType<CombineMesh>();
                if (combiner != null)
                {
                    if (grassParent.childCount > 0)
                        combiner.Combine(grassParent, blockData.grassBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Grass");
                    if (dirtParent.childCount > 0)
                        combiner.Combine(dirtParent, blockData.dirtBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Dirt");
                    if (sandParent.childCount > 0)
                        combiner.Combine(sandParent, blockData.sandBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Sand");
                    if (waterParent.childCount > 0)
                        combiner.Combine(waterParent, blockData.waterPlane.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Water");
                    if (templeParent.childCount > 0 && blockData.templeFloorBlock != null)
                        combiner.Combine(templeParent, blockData.templeFloorBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Temple");
                    if (swampParent.childCount > 0)
                        combiner.Combine(swampParent, blockData.swampBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Swamp");
                    if (rockParent.childCount > 0)
                        combiner.Combine(rockParent, blockData.rockBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{chunkParent.name}_Rock");
                }

                yield return null;
            }
        }

        if (temple.exists && temple.prefab != null)
            MonoBehaviour.Instantiate(temple.prefab, new Vector3(temple.pos.x, Mathf.Max(temple.pos.y, temple.scaleY), temple.pos.z), Quaternion.identity, Root);
    }

    IEnumerator IGeneratable.Generate() => Generate();

    public void SpawnPlayer()
    {
        if (player == null || sandPositions.Count == 0) return;
        Vector3 spawnPos = sandPositions[0];
        float minDist = float.MaxValue;
        foreach (var pos in sandPositions)
        {
            if (pos.y <= seaLevel) continue;

            float distFromCenter = new Vector2(pos.x, pos.z).magnitude;

            if (distFromCenter >= radius - beachWidth * 2)
            {
                float diff = Mathf.Abs(distFromCenter - (radius - beachWidth / 2));

                if (diff < minDist)
                {
                    minDist = diff;
                    spawnPos = pos;
                }
            }
        }

        player.position = spawnPos + Vector3.up * 1f;
    }

    public IEnumerator SceneChange()
    {
        WorldMapMarker worldMapMarker = MonoBehaviour.FindFirstObjectByType<WorldMapMarker>();
        while (worldMapMarker != null && worldMapMarker.isRendering) yield return null;
        SceneManager.UnloadSceneAsync("MainScene");
        yield return new WaitForSeconds(0.1f);

        if (Loading.Instance != null)
        {
            Loading.Instance.loadingPanel.SetActive(false);
            Loading.Instance.isLoading = false;
        }
    }
}