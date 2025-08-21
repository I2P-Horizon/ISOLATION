using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueArea
{
    public Vector2 Pos { get; private set; }

    private MapScale mapScale;

    public UniqueArea(MapScale scale)
    {
        mapScale = scale;
    }

    public void Create()
    {
        Pos = new Vector2(
            Random.Range(mapScale.total * 0.4f, mapScale.total * 0.45f),
            Random.Range(mapScale.total * 0.55f, mapScale.total * 0.7f)
        );
    }

    public bool IsUniqueArea(int x, int z)
    {
        return Vector2.Distance(new Vector2(x, z), Pos) < mapScale.lake * 0.5f;
    }

    public bool IsUniqueCenter(int x, int z)
    {
        return x == Mathf.RoundToInt(Pos.x) && z == Mathf.RoundToInt(Pos.y);
    }

    public void PlaceUniqueBlocks(Vector3 origin, int x, int z, GameObject dirtPrefab, Vector3 dirtScale,
        Transform dirtParent, MapObjectManager mapObjManager, MapHeight mapHeight, MapScale mapScale)
    {
        int dirtLayers = 3;

        for (int i = 0; i < dirtLayers; i++)
        {
            float y = mapHeight.sandHeight + i * mapScale.block.y;
            Vector3 pos = origin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);

            GameObject dirt = MonoBehaviour.Instantiate(dirtPrefab, pos, Quaternion.identity, dirtParent);
            dirt.transform.localScale = dirtScale;

            if (i == dirtLayers - 1 && IsUniqueCenter(x, z))
            {
                mapObjManager.PlaceUniqueAt(pos + new Vector3(0, mapScale.block.y, 0));
            }
        }
    }
}