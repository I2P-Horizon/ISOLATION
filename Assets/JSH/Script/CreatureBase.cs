using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class CreatureBase : DestructibleObject, ICycleListener // Creture�� ���ΰ�, �߻� Ŭ���� Object���� ��� �޾� ���
{
    [Header("Animal")]
    [SerializeField] protected bool AnimalState;
    [SerializeField] private GameObject animalShape;


    [Header("Monster")]
    [SerializeField] protected bool MonsterState;
    [SerializeField] private GameObject monsterShape;

    [Header("Stat")]
    [SerializeField] protected float moveSpeed;

    protected virtual void Awake()
    {
        if (!TimeManager.instance) return;

        if (!TimeManager.instance.IsNight) AnimalState = true;
        else if (TimeManager.instance.IsNight) MonsterState = true;

        TimeManager.instance.Register(this);
        Debug.Log("Creature Register Complete");
    }

    protected override void DestroyObject()
    {
        if (!TimeManager.instance) return;
        
        TimeManager.instance.UnRegister(this);
        base.DestroyObject();

        Debug.Log("Creature UnRegister Complete");
    }

    public virtual void OnCycleChanged(bool isNight)
    {
        if (!isNight)
        {
            //Debug.Log("Animal");
            AnimalState = true;
            MonsterState = false;
            ToAnimalState();
        }

        else if(isNight)
        {
            //Debug.Log("Monster");
            AnimalState = false;
            MonsterState = true;
            ToMonsterState();
        }
    }

    private void ToAnimalState() // �̰� ��ȯ �������� OnTransform() �ְ� ���� �����Ű�� �͵� �� ��. 
                                 // �ڷ�ƾ �̿�
    {
        transform.localScale = Vector3.one;
        //animalShape.SetActive(true);
        //monsterShape.SetActive(false);
    }
    private void ToMonsterState()
    {
        transform.localScale = Vector3.one * 3;
        //animalShape.SetActive(false);
        //monsterShape.SetActive(true);
    }

    // AI
    protected NavMeshAgent agent;
    [SerializeField] protected float patrolRadius = 5f;

    protected virtual void SetNextPatrolPoint() 
    {
        Debug.Log("New Destination Setting");
        Vector3 randomDirection = Random.insideUnitCircle * patrolRadius;

        randomDirection += new Vector3 (transform.position.x, 0, transform.position.z);

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    protected virtual void AnimalAi() { }
    protected virtual void MonsterAi() { }
}
