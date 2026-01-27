using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictEyesController : MonoBehaviour
{
    [SerializeField] private GameObject predictEyesRadius;
    [SerializeField] private float durationTime;
    [SerializeField] Material treeMaterial;
    [SerializeField] private GameObject dummyTombstonePrefab;
    [SerializeField] private int spawningTombstoneCount;
    [SerializeField] private GameObject dummyStonePrefab;
    [SerializeField] private int spawningStoneCount;
    [SerializeField] private GameObject heavySnowPrefab;
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
    // 1: TombStoneSpawnEvent
    // 2: StoneSpawnEvent
    // 3: HeavySnow
    // 4:

    IEnumerator PredictEyesActivation(int cachedEventNum)
    {
        switch(cachedEventNum)
        {
            case 0:
                Debug.Log("Phenomenon of dream: ChangeTreesEmission");
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
        }

        yield return new WaitForSecondsRealtime(durationTime);
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
        // JSH TODO: Implementation
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
}

