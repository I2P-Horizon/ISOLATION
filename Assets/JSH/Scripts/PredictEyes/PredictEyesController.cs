using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PredictEyesController : MonoBehaviour
{
    [SerializeField] private GameObject predictEyesRadius;
    [SerializeField] private float durationTime;
    [SerializeField] Material tree1Material;
    [SerializeField] Material tree2Material;
    [SerializeField] Material tree3Material;
    [SerializeField] private GameObject dummyTombstonePrefab;
    [SerializeField] private int spawningTombstoneCount;
    [SerializeField] private GameObject dummyStonePrefab;
    [SerializeField] private int spawningStoneCount;
    [SerializeField] private GameObject heavySnowPrefab;

    [SerializeField] private VolumeProfile globalVolumeProfile;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private int spawningGhostCount;
    [SerializeField] private GameObject historicEntrancePrefab;
    [SerializeField] private Transform playerTranfsorm;
    private int _minX, _maxX, _y, _minZ, _maxZ;

    private int cachedEventNum;

    void Awake()
    {
        durationTime = 30.0f;

        InitSpawnArea(-300, 300, 50, -300, 300);
    }
    


    void Start()
    {
        cachedEventNum = EventManager.Instance.EventNum;
    }

    void OnEnable()
    {
        EventManager.OnNextEventNumberSet += ChangeNextEventNumber;
    }

    void OnDisable()
    {
        EventManager.OnNextEventNumberSet -= ChangeNextEventNumber;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(IncreaseRadius());
            StartCoroutine(PredictEyesActivation(cachedEventNum));
        }
        predictEyesRadius.transform.LookAt(Camera.main.transform);
    }



    private void InitSpawnArea(int minX, int maxX, int y, int minZ, int maxZ)
    {
        _minX = minX;
        _maxX = maxX;
        _y = y;
        _minZ = minZ;
        _maxZ = maxZ;
    }



    void ChangeNextEventNumber()
    {
        if (EventManager.Instance.IsDirty())
        {
            cachedEventNum = EventManager.Instance.EventNum;
            EventManager.Instance.SetDirty(false);
        }
    }

    // EventManager에 등록해둔 순서 == EventNum
    // EventNumList
    // 0: ChangeTreesEmission
    // 1: TombStoneSpawn
    // 2: StoneSpawn
    // 3: HeavySnow
    // 4: Fear
    // 5: Heatwave
    // 6: HistoricEntrance

    IEnumerator PredictEyesActivation(int cachedEventNum)
    {
        switch(cachedEventNum)
        {
            case 0:
                Debug.Log("Phenomenon of dream: ChangeTreesEmission");
                ChangeTreesEmissionPhenomenon();
                break;
            case 1:
                Debug.Log("Phenomenon of dream: SpawnTombstone");
                SpawnTombstonePhenomenon();
                break;
            case 2:
                Debug.Log("Phenomenon of dream: SpawnStone");
                SpawnStonePhenomenon();
                break;
            case 3:
                Debug.Log("Phenomenon of dream: HeavySnow");
                HeavySnowPhenomenon();
                break;
            case 4:
                Debug.Log("Phenomenon of dream: Fear");
                FearPhenomenon();
                break;
            case 5:
                Debug.Log("Phenomenon of dream: Heatwave");
                HeatWavePhenomenon();
                break;
            case 6:
                Debug.Log("Phenomenon of dream: HistoricEntrance");
                HistoricEntrancePhenomenon();
                break;
        }

        yield return new WaitForSecondsRealtime(durationTime);
        switch (cachedEventNum)
        {
            case 0:
                Debug.Log("End Phenomenon of dream: ChangeTreesEmission");
                EndChangeTreesEmissionPhenomenon();
                break;
            case 1:
                Debug.Log("End Phenomenon of dream: SpawnTombstone");
                break;
            case 2:
                Debug.Log("End Phenomenon of dream: SpawnStone");
                break;
            case 3:
                Debug.Log("End Phenomenon of dream: HeavySnow");
                break;
            case 4:
                Debug.Log("End Phenomenon of dream: Fear");
                EndFearPhenomenon();
                break;
            case 5:
                Debug.Log("End Phenomenon of dream: Heatwave");
                EndHeatWavePhenomenon();
                break;
            case 6:
                Debug.Log("End Phenomenon of dream: HistoricEntrance");
                break;
        }

        StopCoroutine(PredictEyesActivation(cachedEventNum));
    }

    IEnumerator IncreaseRadius()
    {
        predictEyesRadius.SetActive(true);
        while (predictEyesRadius.transform.localScale.z <= 15.0f)
        {
            predictEyesRadius.transform.localScale += new Vector3(20.0f * Time.deltaTime, 20.0f * Time.deltaTime, 20.0f * Time.deltaTime);
            yield return null;
        }
        
        StopCoroutine(IncreaseRadius());
        yield return new WaitForSecondsRealtime(durationTime);
        StartCoroutine(DecreaseRadius());
    }

    IEnumerator DecreaseRadius()
    {
        while (predictEyesRadius.transform.localScale.z >= 0.0f)
        {
            predictEyesRadius.transform.localScale -= new Vector3(20.0f * Time.deltaTime, 20.0f * Time.deltaTime, 20.0f * Time.deltaTime);
            yield return null;
        }
        predictEyesRadius.SetActive(false);

        StopCoroutine(DecreaseRadius());
        StopCoroutine(PredictEyesActivation(cachedEventNum));
    }
    private void ChangeTreesEmissionPhenomenon()
    {
        tree1Material.DisableKeyword("_EMISSION");
        tree2Material.DisableKeyword("_EMISSION");
        tree3Material.DisableKeyword("_EMISSION");
    }

    private void EndChangeTreesEmissionPhenomenon()
    {
        tree1Material.EnableKeyword("_EMISSION");
        tree2Material.EnableKeyword("_EMISSION");
        tree3Material.EnableKeyword("_EMISSION");
    }

    private void SpawnStonePhenomenon()
    {
        for (int i = 0; i < spawningStoneCount; i++)
        {
            GameObject instance = Instantiate(dummyStonePrefab);
            instance.transform.position = new Vector3(Random.Range(_minX, _maxX), _y, Random.Range(_minZ, _maxZ));
            Destroy(instance.gameObject, durationTime);
        }
    }

    private void SpawnTombstonePhenomenon()
    {
        for (int i = 0; i < spawningTombstoneCount; i++)
        {
            GameObject instance = Instantiate(dummyTombstonePrefab);
            instance.transform.position = new Vector3(Random.Range(_minX, _maxX), _y, Random.Range(_minZ, _maxZ));
            Destroy(instance.gameObject, durationTime);
        }
    }

    private void HeavySnowPhenomenon()
    {
        GameObject instance = Instantiate(heavySnowPrefab);
        instance.transform.position = new Vector3(Random.Range(-30.0f, 30.0f), _y, Random.Range(-30.0f, 30.0f));
        Destroy(instance.gameObject, durationTime);
    }

    private void HeatWavePhenomenon()
    {
        globalVolumeProfile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = 3.0f;
        color.contrast.value = -50.0f;
        color.hueShift.value = -30.0f;
        color.saturation.value = -20.0f;
    }
    private void EndHeatWavePhenomenon()
    {
        globalVolumeProfile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = 0.0f;
        color.contrast.value = 0.0f;
        color.hueShift.value = 0.0f;
        color.saturation.value = 0.0f;
    }

    private void FearPhenomenon()
    {
        for (int i = 0; i < spawningGhostCount; i++)
        {
            GameObject instance = Instantiate(ghostPrefab);
            instance.transform.position = new Vector3(Random.Range(_minX, _maxX), _y, Random.Range(_minZ, _maxZ));
            Destroy(instance.gameObject, durationTime);
        }

        globalVolumeProfile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = 3.0f;
        color.contrast.value = 40.0f;
        color.hueShift.value = 10.0f;
        color.saturation.value = -50.0f;
    }

    private void EndFearPhenomenon()
    {
        globalVolumeProfile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = 0.0f;
        color.contrast.value = 0.0f;
        color.hueShift.value = 0.0f;
        color.saturation.value = 0.0f;
    }

    private void HistoricEntrancePhenomenon()
    {
        GameObject instance = Instantiate(historicEntrancePrefab);
        instance.transform.position = playerTranfsorm.transform.position + new Vector3(30.0f, 0.0f, 0.0f);
        Destroy(instance.gameObject, durationTime);
    }
}

