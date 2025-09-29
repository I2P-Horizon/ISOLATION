using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Island island;
    [SerializeField] private Temple temple;
    [SerializeField] private Jungle jungle;
    [SerializeField] private Height height;
    [SerializeField] private Shape shape;
    [SerializeField] private Grid grid;
    [SerializeField] private Noise noise;

    [Header("Data")]
    [SerializeField] private BlockData blockData;
    [SerializeField] private ObjectData[] objectData;

    public static float generationProgress = 0f;

    private ObjectSpawner objectSpawner = new ObjectSpawner();
    private MapObject mapObject = new MapObject();

    public MapObject mapObj => mapObject;

    private IEnumerator RunGeneration()
    {
        yield return StartCoroutine(island.Spawn(island.pos));
        objectSpawner.SpawnObjects();
        jungle.Spawn();
        island.SpawnPlayer();
        yield return StartCoroutine(island.SceneChange());
    }

    private void Start()
    {
        island.Set(height, shape, grid, noise, jungle, temple, blockData, mapObject);
        jungle.Set(island, height, shape, temple, mapObject);
        temple.Set(height, shape, noise);
        mapObject.Set(grid);
        objectSpawner.Set(island, grid, temple, blockData, objectData, mapObject);

        noise.Seed();
        temple.Placement();

        StartCoroutine(RunGeneration());
    }
}