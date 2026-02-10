using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, ICycleListener
{
    // Day -> Night 전환 시, 크리처 추가 스폰 조건 | 기존에 활성화돼 있는 크리처의 수 기준
    private const int _OnChangeToNightCreatureSpawnThreshold = 10;
    // Night -> Day
    private const int _OnChangeToDayCreatureSpawnThreshold = 5;

    float timer = 0.0f;
    private void Update()
    {
        timer += Time.deltaTime;

        // COMPLETED_JSH TODO: Update()가 아닌, InitSpawn()으로 변경 -> IslandManager.cs 과의 연동 필요 - (김선욱 팀장님) -> 섬 사이즈 고정
    }

    private void OnEnable()
    {
        IslandManager.OnGenerationComplete += InitSpawn;
    }

    private void OnDisable()
    {
        IslandManager.OnGenerationComplete -= InitSpawn;
    }

    // KSW
    private void InitSpawn()
    {
        if (timer >= 5.0f)
        {
            timer = -1000.0f;
            if (!TimeManager.Instance || !CreaturePoolsManager.Instance) return;

            TimeManager.Instance.Register(this);
            
            InitSpawnArea(-100, 100, 300, -100, 100);

            // Spawn creatures when game starts
            Debug.Log("Creature Spawned, when game starts");
            for (int creatureID = 0; creatureID < CreaturePoolsManager.Instance.GetCreaturePoolListCount(); creatureID++)
            {
                StartCoroutine(SpawnCreatureCoroutine(creatureID));
            }
        }
    }

    public void OnCycleChanged()
    {
        // Night -> Day
        if (!TimeManager.Instance.IsNight)
        {
            Debug.Log("Creature Spawned, when day begins");
            for (int creatureID = 0; creatureID < CreaturePoolsManager.Instance.GetCreaturePoolListCount(); creatureID++)
            {
                StartCoroutine(SpawnCreatureCoroutine(creatureID));
            }
        }

        // Day -> Night
        if (TimeManager.Instance.IsNight)
        {
            Debug.Log("Creature Spawned, when night begins");
            for (int creatureID = 0; creatureID < CreaturePoolsManager.Instance.GetCreaturePoolListCount(); creatureID++)
            {
                StartCoroutine(SpawnCreatureCoroutine(creatureID));
            }
        }
    }

    private void SpawnCreature(int creatureID)
    {
        // COMPLETED_JSH TODO: 섬 크기에 따른 스폰 범위 설정 -> IslandManager.cs 과의 연동 필요 - (김선욱 팀장님) -> 섬 사이즈 고정
        Vector3 spawnPos = new Vector3(Random.Range(_minX, _maxX), _y, Random.Range(_minZ, _maxZ));
        CreaturePoolsManager.Instance.SpawnCreature(creatureID, spawnPos);
    }

    private IEnumerator SpawnCreatureCoroutine(int creatureID)
    {
        for (int currentActiveCreatureCount = CreaturePoolsManager.Instance.GetActiveCreatureCount(creatureID); currentActiveCreatureCount < _OnChangeToNightCreatureSpawnThreshold; currentActiveCreatureCount++)
        {
            SpawnCreature(creatureID);
            yield return new WaitForSeconds(0.1f);
        }
    }


    // Tmp
    private int _minX, _maxX, _y, _minZ, _maxZ;
    private void InitSpawnArea(int minX, int maxX, int y, int minZ, int maxZ)
    {
        _minX = minX;
        _maxX = maxX;
        _y = y;
        _minZ = minZ;
        _maxZ = maxZ;
    }
}
