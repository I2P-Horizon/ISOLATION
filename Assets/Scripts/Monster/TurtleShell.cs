using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                             Monster - TurtleShell

        - 가지는 상태 : Idle, Chase, Attack, Die

        - TurtleShell의 능력치 설정 및 상태 초기화

        - 조건에따른 상태전환
            - ScanRange 안에 플레이어가 들어오는경우 -> ChaseState 돌입
            - 플레이어를 쫓다가 복귀한 경우 -> IdleState 돌입
            - 공격범위에 플레이어가 있는경우 -> AttackState 돌입
            - 체력이 0이하 -> DieState 돌입
 */

public class TurtleShell : Monster
{
    // TurtleShell이 가지는 상태
    public enum States
    {
        Idle,
        Chase,
        Attack,
        Die
    }

    public States curState;                             // 현재 상태
    private StateMachine<TurtleShell> stateMachine;
    
    protected override void Awake()
    {
        // 부모(Monster)의 초기화
        base.Awake();
        Init();
    }

    private void Init()
    {
        // TurtleShell 능력치 초기화
        maxHp = 50;
        curHp = maxHp;
        speed = 1f;
        maxDistance = 5f;
        idleThreshold = 0.1f;
        attackDelay = 2f;
        damage = 5f;
        attackRange = 1.3f;

        // 초기상태는 Idle
        curState = States.Idle;
        // StateMachine 객체 생성(Idle상태)
        stateMachine = new StateMachine<TurtleShell>(new IdleState<TurtleShell>(this));

        Nav.speed = speed;
    }

    private void Update()
    {
        stateMachine.curState.OnStateUpdate();

        DecideState();
    }

    // 상태 변경
    public void ChangeState(States nextState)
    {
        curState = nextState;
        switch(curState)
        {
            case States.Idle:
                stateMachine.ChangeState(new IdleState<TurtleShell>(this));
                break;
            case States.Chase:
                stateMachine.ChangeState(new ChaseState<TurtleShell>(this));
                break;
            case States.Attack:
                stateMachine.ChangeState(new AttackState<TurtleShell>(this));
                break;
            case States.Die:
                stateMachine.ChangeState(new DieState<TurtleShell>(this));
                break;
        }
    }


    // 조건에따른 상태전환 결정
    private void DecideState()
    {
        // 플레이어 <-> 몬스터 거리
        float distanceToPlayer = Vector3.Distance(transform.position, Target.transform.position);

        // Chase상태에서 원점으로 복귀완료 -> Idle상태 톨입
        if (curState == States.Chase && isReset && !isDead)
        {
            ChangeState(States.Idle);
        }

        // 공격범위에 들어서면 Attack 상태 돌입
        if (distanceToPlayer <= attackRange && !isDead)
        {
            ChangeState(States.Attack);
        }
        // 공격상태에서 플레이어가 멀어지면 Chase 상태 돌입
        else if (curState == States.Attack && distanceToPlayer > attackRange && !isDead)
        {
            ChangeState(States.Chase);
        }

        // 체력이 0이하로 떨어지면 죽음
        if (curHp <= 0 && !isDead)
        {
            isDead = true;
            AudioManager.Instance.PlaySFX("TurtleShell_Die");
            ChangeState(States.Die);
        }
    }

    // 근접공격
    public void MeleeAttack()
    {
        // Raycast할 위치, 방향
        Vector3 origin = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 direction = transform.forward;

        // Raycast 결과
        RaycastHit hit;

        // SphereRayCast로 플레이어에 닿았는지 확인
        if(Physics.SphereCast(origin, 0.5f,direction,out hit,1f, LayerMask.GetMask("Player")))
        {
            if(hit.collider.CompareTag("Player"))
            {
                // 플레이어에게 데미지입힘
                PlayerData playerData = DataManager.Instance.GetPlayerData();
                playerData.GetDamaged(damage);
            }
        }
    }

    
}
