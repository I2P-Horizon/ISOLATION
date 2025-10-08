using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Temple : Island, IGeneratable
{
    #region Temple Data

    [Header("Temple Settings")]
    public GameObject prefab;
    public float scaleY = 5f;
    public float maxDistanceFromCenter = 50f;

    [HideInInspector]
    public Vector3 pos;
    public bool exists = false;
    #endregion

    protected override IEnumerator Generate()
    {
        exists = false;

        if (prefab == null) yield break;

        for (int i = 0; i < 100; i++)
        {
            Vector2 offset = Random.insideUnitCircle * maxDistanceFromCenter;
            int testX = Mathf.RoundToInt(offset.x);
            int testZ = Mathf.RoundToInt(offset.y);

            float dist = Mathf.Sqrt(testX * testX + testZ * testZ) * radius;
            float noiseMask = Mathf.PerlinNoise((testX + seed) / scale, (testZ + seed) / scale);
            float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.45f);
            islandMask = Mathf.Pow(islandMask, falloffPower);
            islandMask = Mathf.Max(islandMask, 0.05f);

            float heightNoise = 0f; float amplitude = 1f; float frequency = 1f; float maxAmp = 0f;

            for (int o = 0; o < octaves; o++)
            {
                float sampleX = (testX + seed) / scale * frequency;
                float sampleZ = (testZ + seed) / scale * frequency;
                heightNoise += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                maxAmp += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            if (maxAmp > 0f) heightNoise /= maxAmp;

            int landHeight = Mathf.RoundToInt(heightNoise * islandMask * maxHeight);

            if (landHeight <= seaLevel) landHeight = seaLevel + 1;

            if (islandMask > 0f && landHeight > seaLevel + 1)
            {
                pos = new Vector3(testX, landHeight, testZ);
                exists = true;
                break;
            }
        }
    }

    IEnumerator IGeneratable.Generate() => Generate();
}