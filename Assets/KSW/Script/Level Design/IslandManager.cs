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

    /// <summary>
    /// 섬 생성 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator Generation()
    {
        /* 시드 생성 */
        noise.Seed();

        /* 사원 위치 생성 */
        temple.Placement();

        /* 섬 생성 */
        yield return StartCoroutine(island.Spawn(island.pos));

        /* 섬 생성이 완료되면 오브젝트/플레이어 생성 */
        objectSpawner.SpawnObjects();
        jungle.Spawn();
        island.SpawnPlayer();

        /* Game Scene 으로 변경 */
        yield return StartCoroutine(island.SceneChange());
    }

    private void Start()
    {
        island.Set(height, grid, noise, temple, blockData, mapObject);
        jungle.Set(island, height, temple, mapObject);
        temple.Set(height, noise);
        mapObject.Set(grid);
        objectSpawner.Set(island, grid, temple, objectData, mapObject);

        StartCoroutine(Generation());
    }
}