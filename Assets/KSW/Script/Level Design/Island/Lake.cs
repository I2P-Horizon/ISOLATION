using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lake
{
    public GameObject water;
    public GameObject dirt;

    private MapScale mapScale = new MapScale();
    private MapHeight mapHeight = new MapHeight();

    public Vector2 Pos { get; set; }

    public void Create()
    {
        Pos = new Vector2(
            Random.Range(mapScale.total * 0.25f, mapScale.total * 0.45f),
            Random.Range(mapScale.total * 0.25f, mapScale.total * 0.7f));
    }

    public bool IsLakeArea(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        return Vector2.Distance(pos, Pos) < mapScale.lake;
    }

    public bool IsLakeEdge(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        float dist = Vector2.Distance(pos, Pos);
        return dist < mapScale.lake && dist >= mapScale.lake * 0.5f;
    }

    public bool IsLakeInner(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        return Vector2.Distance(pos, Pos) < mapScale.lake * 0.5f;
    }

    /// <summary>
    /// 호수 블록 생성
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="dirtScale"></param>
    /// <param name="waterScale"></param>
    public void PlaceLakeBlocks(Vector3 origin, int x, int z, Vector3 dirtScale, Vector3 waterScale, Transform dirtParent, Transform waterParent)
    {
        int dirtLayers = IsLakeInner(x, z) ? 1 : 2;
        float lakeWaterHeight = mapHeight.sandHeight + mapScale.block.y * 2.5f;

        for (int i = 0; i < dirtLayers; i++)
        {
            float y = mapHeight.sandHeight + i * mapScale.block.y;
            Vector3 pos = origin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);
            GameObject dirt = MonoBehaviour.Instantiate(this.dirt, pos, Quaternion.identity, dirtParent);
            dirt.transform.localScale = dirtScale;
        }

        Vector3 waterPos = origin + new Vector3(x * mapScale.block.x, lakeWaterHeight, z * mapScale.block.z);
        GameObject water = MonoBehaviour.Instantiate(this.water, waterPos, Quaternion.identity, waterParent);
        water.transform.localScale = waterScale;
    }
}