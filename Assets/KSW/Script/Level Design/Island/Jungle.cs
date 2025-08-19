using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Jungle
{
    public GameObject grass;
    public GameObject dirt;

    private MapScale mapScale = new MapScale();
    private MapHeight mapHeight = new MapHeight();
    private MapSeed mapSeed = new MapSeed();

    private Mountain mountain;
    private MapObjectManager mapObjectManager;

    public void SetMountain(Mountain mountain)
    {
        this.mountain = mountain;
    }

    public void SetMapObjectManager(MapObjectManager manager)
    {
        mapObjectManager = manager;
    }

    /// <summary>
    /// ÀÜµð & Èë ¹èÄ¡
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="sandLayers"></param>
    /// <param name="grassScale"></param>
    /// <param name="dirtScale"></param>
    public void PlaceGrassAndDirt(Vector3 origin, int x, int z, int sandLayers, Vector3 grassScale, Vector3 dirtScale, Transform grassParent, Transform dirtParent)
    {
        float innerDist = Vector2.Distance(new Vector2(x, z), new Vector2(mapScale.total / 2f, mapScale.total / 2f)) - (mapScale.total / 2f - mapScale.beach);
        float maxInnerDist = mapScale.total / 2f - mapScale.beach;
        float t = Mathf.Clamp01(innerDist / maxInnerDist);

        int grassLayersBase = Mathf.RoundToInt(Mathf.Lerp(mapHeight.minGrass, mapHeight.maxGrass, t));
        float noise = Mathf.PerlinNoise((x + mapSeed.heightX) / mapHeight.noiseScale, (z + mapSeed.heightZ) / mapHeight.noiseScale);
        int noiseLayers = Mathf.RoundToInt(noise * (mapHeight.maxGrass - mapHeight.minGrass));

        float mountainBoost = mountain.GetMountainHeight(x, z);
        int mountainExtraLayers = Mathf.RoundToInt(mountainBoost);

        int grassLayers = Mathf.Clamp(
            grassLayersBase + noiseLayers + mountainExtraLayers - (mapHeight.maxGrass - mapHeight.minGrass) / 2,
            mapHeight.minGrass,
            mapHeight.maxGrass + Mathf.RoundToInt(mountain.height)
        );

        GameObject lastGrassBlock = null;

        for (int i = 0; i < grassLayers; i++)
        {
            float y = mapHeight.sandHeight + sandLayers * mapScale.block.y + i * mapScale.block.y;
            Vector3 blockPos = origin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);

            GameObject blockToPlace = (i == grassLayers - 1) ? grass : dirt;
            Transform parentToUse = (i == grassLayers - 1) ? grassParent : dirtParent;
            Vector3 scaleToUse = (i == grassLayers - 1) ? grassScale : dirtScale;

            GameObject placedBlock = MonoBehaviour.Instantiate(blockToPlace, blockPos, Quaternion.identity, parentToUse);
            placedBlock.transform.localScale = scaleToUse;

            if (i == grassLayers - 1) lastGrassBlock = placedBlock;
        }

        mapObjectManager.TryPlaceMapObjects(lastGrassBlock);
    }
}