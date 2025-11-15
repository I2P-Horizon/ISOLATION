using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CreaturePoolsManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _creaturePrefabs;

    public static CreaturePoolsManager Instance { get; private set; }

    // Creature pool
    private List<ObjectPool<GameObject>> _creaturePoolList;

    private List<int> _creaturePoolInitSize;
    private List<int> _creaturePoolMaxSize;



    /// <summary>
    /// index:     Creature Type   Default Capacity
    /// 0:         Gorilla              15
    /// 1:         Crocodile            15
    /// 2:         Tiger                15
    /// 3:         Armadillo            20
    /// </summary>
    private void Awake()
    {
        Instance = this;

        InitCreaturePool();
    }


    
    public void OnCreatureDie(GameObject creatureObject)
    {
        var creatureType = creatureObject.GetComponent<CreatureBase>();

        Debug.Log(creatureType + " back to Pool");

        // Creature가 늘어날 경우, 하위 코드 내, 한 줄 추가 필요
        if (creatureType is Gorilla) _creaturePoolList[0].Release(creatureObject);
        else if (creatureType is Crocodile) _creaturePoolList[1].Release(creatureObject);
        else if (creatureType is Tiger) _creaturePoolList[2].Release(creatureObject);
        else if (creatureType is Armadillo) _creaturePoolList[3].Release(creatureObject);
        else Debug.Log("CreaturePoolManager.cs | OnCreatureDie() | creature type error");
    }
    public int GetCreaturePoolListCount()
    {
        return _creaturePoolList.Count;
    }
    public int GetCreaturePoolInitSize(int creatureID)
    {
        return _creaturePoolInitSize[creatureID];
    }
    public int GetActiveCreatureCount(int creatureID)
    {
        return _creaturePoolList[creatureID].CountActive;
    }
    public void SpawnCreature(int creatureID, Vector3 position)
    {
        GameObject creature = _creaturePoolList[creatureID].Get();
        creature.transform.position = position;
    }



    private void ActivatePoolCreature(GameObject creatureObject)
    {
        creatureObject.SetActive(true);
    }
    private void DisablePoolCreature(GameObject creatureObject)
    {
        creatureObject.SetActive(false);
    }
    private void DestroyPoolCreature(GameObject creatureObject)
    {
        Destroy(creatureObject);
    }



    private void InitCreaturePool()
    {
        _creaturePoolList = new List<ObjectPool<GameObject>>(_creaturePrefabs.Count);
        _creaturePoolInitSize = new List<int>(_creaturePrefabs.Count);
        _creaturePoolMaxSize = new List<int>(_creaturePrefabs.Count);

        // 최대 개체수
        for (int creatureID = 0; creatureID < _creaturePrefabs.Count; creatureID++)
        {
            if (creatureID == 3) _creaturePoolInitSize.Add(20);
            else _creaturePoolInitSize.Add(15);

            _creaturePoolMaxSize.Add(20);
        }


        // 각 Creature별 Pool 생성 -> 람다식 및 캡처 개념을 이용하여 각 Pool마다 Instantiate()시 생성되는 creaturePrefab을 다르게 함.
        for (int creatureID = 0; creatureID < _creaturePrefabs.Count; creatureID++)
        {
            GameObject creaturePrefab = _creaturePrefabs[creatureID];
            
            ObjectPool<GameObject> creaturePool = new ObjectPool<GameObject>(
                () => Instantiate(creaturePrefab),
                ActivatePoolCreature, DisablePoolCreature, DestroyPoolCreature,
                false, _creaturePoolInitSize[creatureID], _creaturePoolMaxSize[creatureID]
                );

            _creaturePoolList.Add(creaturePool);
        }
    }
}
