using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mountain
{
    private MapScale mapScale = new MapScale();

    public Vector2 Pos { get; set; }

    public float radius = 60f;
    public float height = 20f;

    public void Create()
    {
        Pos = new Vector2(
            Random.Range(mapScale.total * 0.65f, mapScale.total * 0.7f),
            Random.Range(mapScale.total * 0.5f, mapScale.total * 0.7f));
    }

    public float GetMountainHeight(int x, int z)
    {
        Vector2 pos = new Vector2(x, z);
        float dist = Vector2.Distance(pos, Pos);

        if (dist > radius) return 0f;

        float t = 1f - (dist / radius);
        return t * height;
    }
}