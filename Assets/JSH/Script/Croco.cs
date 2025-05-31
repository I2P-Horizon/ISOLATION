using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Croco : CreatureBase
{

    // Stat

    protected override void AnimalAi() // https://docs.unity3d.com/kr/2022.3/Manual/nav-AgentPatrol.html
    {
        if (!agent.pathPending && (agent.remainingDistance <= 0.5f))
            SetNextPatrolPoint();
    }
    protected override void MonsterAi()
    {
        if (!agent.pathPending && (agent.remainingDistance <= 0.5f))
            SetNextPatrolPoint();
    }

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        //SetNextPatrolPoint();
    }
    void Update()
    {
        if (AnimalState) AnimalAi();
        else if(MonsterState) MonsterAi();
    }
}
