using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Temple : Shape
{
    private Height height;
    private Noise noise;

    [Header("Temple Settings")]
    public GameObject prefab;
    public float scaleY = 5f;
    public float maxDistanceFromCenter = 50f;

    [HideInInspector]
    public Vector3 pos;
    public bool exists = false;

    public void Set(Height height, Noise noise)
    {
        this.height = height; this.noise = noise;
    }

    public void Placement()
    {
        exists = false;

        if (prefab == null) return;

        for (int i = 0; i < 100; i++)
        {
            Vector2 offset = Random.insideUnitCircle * maxDistanceFromCenter;
            int testX = Mathf.RoundToInt(offset.x);
            int testZ = Mathf.RoundToInt(offset.y);

            float dist = Mathf.Sqrt(testX * testX + testZ * testZ) / radius;
            float noiseMask = Mathf.PerlinNoise((testX + noise.seed) / noise.scale, (testZ + noise.seed) / noise.scale);
            float islandMask = Mathf.Clamp01(1f - dist * 0.8f + (noiseMask - 0.5f) * 0.45f);
            islandMask = Mathf.Pow(islandMask, falloffPower);
            islandMask = Mathf.Max(islandMask, 0.05f);

            float heightNoise = 0f; float amplitude = 1f; float frequency = 1f; float maxAmp = 0f;

            for (int o = 0; o < noise.octaves; o++)
            {
                float sampleX = (testX + noise.seed) / noise.scale * frequency;
                float sampleZ = (testZ + noise.seed) / noise.scale * frequency;
                heightNoise += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                maxAmp += amplitude;
                amplitude *= noise.persistence;
                frequency *= noise.lacunarity;
            }

            if (maxAmp > 0f) heightNoise /= maxAmp;

            int landHeight = Mathf.RoundToInt(heightNoise * islandMask * height.maxHeight);

            if (landHeight <= height.seaLevel) landHeight = height.seaLevel + 1;

            if (islandMask > 0f && landHeight > height.seaLevel + 1)
            {
                pos = new Vector3(testX, landHeight, testZ);
                exists = true;
                break;
            }
        }
    }
}