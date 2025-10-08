using System.Collections;
using UnityEngine;

public interface IGeneratable
{
    IEnumerator Generate();
}

public class IslandManager : MonoBehaviour, IGeneratable
{
    #region IslandManager Data
    [Header("Settings")]
    [SerializeField] private IslandGenerator island;
    [SerializeField] private Temple temple;
    [SerializeField] private Jungle jungle;

    [Header("Data")]
    [SerializeField] private BlockData blockData;
    [SerializeField] private ObjectData[] objectData;

    public static float generationProgress = 0f;

    private ObjectSpawner objectSpawner = new ObjectSpawner();
    private MapObject mapObject = new MapObject();


    public MapObject mapObj => mapObject;
    #endregion

    /// <summary>
    /// 섬 생성 코루틴
    /// </summary>
    /// <returns></returns>
    public IEnumerator Generate()
    {
        /* 시드 생성 */
        island.Seed();

        /* 사원 위치 생성 */
        if (temple is IGeneratable t) StartCoroutine(t.Generate());
        /* 섬 생성 */
        if (island is IGeneratable i) yield return StartCoroutine(i.Generate());
        /* 정글 생성 */
        if (jungle is IGeneratable j) yield return StartCoroutine(j.Generate());

        /* 섬 생성이 완료되면 오브젝트/플레이어 생성 */
        objectSpawner.SpawnObjects();
        island.SpawnPlayer();

        /* GameScene으로 변경 */
        yield return StartCoroutine(island.SceneChange());
    }

    private void Start()
    {
        island.Set(temple, blockData, mapObject);
        jungle.Set(island, temple, mapObject);
        objectSpawner.Set(island, temple, objectData, mapObject);

        StartCoroutine(Generate());
    }
}