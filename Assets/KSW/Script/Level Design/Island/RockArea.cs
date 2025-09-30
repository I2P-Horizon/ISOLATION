using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RockArea
{
    private Temple temple;
    private MapObject mapObject;
    private BlockData blockData;

    public List<Vector3> RockPositions { get; private set; } = new List<Vector3>();

    public Transform Parent { get; private set; }

    public GameObject rockObject;
    [Range(0, 1)] public float spawnChance;

    private Transform rockBlockParent;
    private Transform rockObjectParent;

    public void Set(Temple temple, MapObject mapObject, BlockData blockData)
    {
        this.temple = temple;
        this.mapObject = mapObject;
        this.blockData = blockData;
    }

    public void Initialize(Transform parent)
    {
        Parent = new GameObject("RockArea").transform;
        Parent.SetParent(parent);
        RockPositions.Clear();

        rockBlockParent = new GameObject("RockBlocks").transform;
        rockBlockParent.SetParent(Parent);

        rockObjectParent = new GameObject("RockObjects").transform;
        rockObjectParent.SetParent(Parent);
    }

    public void Generate(Vector3 pos, GameObject rockBlock, GameObject rockObject)
    {
        if (temple.exists && Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(temple.pos.x, 0, temple.pos.z)) <= temple.radius)
            return;

        GameObject rock = GameObject.Instantiate(rockBlock, pos, Quaternion.identity, rockBlockParent);

        if (blockData != null && blockData.scaleCache.TryGetValue(rockBlock, out Vector3 scale))
            rock.transform.localScale = scale;
        else
            rock.transform.localScale = Vector3.one;

        RockPositions.Add(pos);

        if (mapObject != null) mapObject.RegisterObject(rock);

        if (rockObject != null && Random.value <= spawnChance)
        {
            Vector3 objPos = pos + Vector3.up * rock.transform.localScale.y;
            GameObject obj = GameObject.Instantiate(rockObject, objPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0), rockObjectParent);

            float sizeFactor = Random.Range(0.8f, 1.3f);
            obj.transform.localScale = Vector3.one * sizeFactor;

            if (mapObject != null) mapObject.RegisterObject(obj);
        }

        CombineMesh combiner = MonoBehaviour.FindAnyObjectByType<CombineMesh>();
        if (combiner != null)
        {
            combiner.Combine(rockBlockParent, rockBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{Parent.name}_Rock");
        }
    }
}