using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                             Monster - TurtleShell

        - ������ ���� : Idle, Chase, Attack, Die

        - TurtleShell�� �ɷ�ġ ���� �� ���� �ʱ�ȭ

        - ���ǿ����� ������ȯ
            - ScanRange �ȿ� �÷��̾ �����°�� -> ChaseState ����
            - �÷��̾ �Ѵٰ� ������ ��� -> IdleState ����
            - ���ݹ����� �÷��̾ �ִ°�� -> AttackState ����
            - ü���� 0���� -> DieState ����
 */

public class TurtleShell : Monster
{
    // TurtleShell�� ������ ����
    public enum States
    {
        Idle,
        Chase,
        Attack,
        Die
    }

    public States curState;                             // ���� ����
    private StateMachine<TurtleShell> stateMachine;
    
    protected override void Awake()
    {
        // �θ�(Monster)�� �ʱ�ȭ
        base.Awake();
        Init();
    }

    private void Init()
    {
        // TurtleShell �ɷ�ġ �ʱ�ȭ
        maxHp = 50;
        curHp = maxHp;
        speed = 1f;
        maxDistance = 5f;
        idleThreshold = 0.1f;
        attackDelay = 2f;
        damage = 5f;
        attackRange = 1.3f;

        // �ʱ���´� Idle
        curState = States.Idle;
        // StateMachine ��ü ����(Idle����)
        stateMachine = new StateMachine<TurtleShell>(new IdleState<TurtleShell>(this));

        Nav.speed = speed;
    }

    private void Update()
    {
        stateMachine.curState.OnStateUpdate();

        DecideState();
    }

    // ���� ����
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


    // ���ǿ����� ������ȯ ����
    private void DecideState()
    {
        // �÷��̾� <-> ���� �Ÿ�
        float distanceToPlayer = Vector3.Distance(transform.position, Target.transform.position);

        // Chase���¿��� �������� ���ͿϷ� -> Idle���� ����
        if (curState == States.Chase && isReset && !isDead)
        {
            ChangeState(States.Idle);
        }

        // ���ݹ����� ���� Attack ���� ����
        if (distanceToPlayer <= attackRange && !isDead)
        {
            ChangeState(States.Attack);
        }
        // ���ݻ��¿��� �÷��̾ �־����� Chase ���� ����
        else if (curState == States.Attack && distanceToPlayer > attackRange && !isDead)
        {
            ChangeState(States.Chase);
        }

        // ü���� 0���Ϸ� �������� ����
        if (curHp <= 0 && !isDead)
        {
            isDead = true;
            AudioManager.Instance.PlaySFX("TurtleShell_Die");
            ChangeState(States.Die);
        }
    }

    // ��������
    public void MeleeAttack()
    {
        // Raycast�� ��ġ, ����
        Vector3 origin = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 direction = transform.forward;

        // Raycast ���
        RaycastHit hit;

        // SphereRayCast�� �÷��̾ ��Ҵ��� Ȯ��
        if(Physics.SphereCast(origin, 0.5f,direction,out hit,1f, LayerMask.GetMask("Player")))
        {
            if(hit.collider.CompareTag("Player"))
            {
                // �÷��̾�� ����������
                PlayerData playerData = DataManager.Instance.GetPlayerData();
                playerData.GetDamaged(damage);
            }
        }
    }

    
}
