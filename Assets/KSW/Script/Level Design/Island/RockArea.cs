using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RockArea : Shape
{
    private Temple temple;
    private MapObject mapObject;

    public List<Vector3> RockPositions { get; private set; } = new List<Vector3>();
    public Transform Parent { get; private set; }

    public GameObject rockObject;

    public void Set(Temple temple, MapObject mapObject)
    {
        this.temple = temple;
        this.mapObject = mapObject;
    }

    public void Initialize(Transform parent)
    {
        Parent = new GameObject("RockArea").transform;
        Parent.SetParent(parent);
        RockPositions.Clear();
    }

    public void Generate(Vector3 pos, GameObject rockPrefab)
    {
        if (temple.exists && Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(temple.pos.x, 0, temple.pos.z)) <= temple.radius) return;

        GameObject rock = GameObject.Instantiate(rockPrefab, pos, Quaternion.identity, Parent);
        RockPositions.Add(pos);

        if (mapObject != null) mapObject.RegisterObject(rock);
    }
}