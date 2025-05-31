using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class CreatureBase : DestructibleObject, ICycleListener//�߻� Ŭ���� DestructibleObject���� ��� �޾� ���
{
    [Header("AnimalState")]
    [SerializeField] protected bool AnimalState;
    [SerializeField] private GameObject animalShape;


    [Header("MonsterState")]
    [SerializeField] protected bool MonsterState;
    [SerializeField] private GameObject monsterShape;

    [Header("State")]
    [SerializeField] protected bool isPatrolling;
    [SerializeField] protected bool isChasing;
    [SerializeField] protected bool isAttacking;

    [Header("Stats")]
    [SerializeField] protected float moveSpeed = 5.0f;
    [SerializeField] protected float defense;
    [SerializeField] protected float detectionRadius = 5.0f;
    [SerializeField] protected float attackableRadius = 2.0f;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackSpeed;

    [SerializeField] protected float distanceToPlayer;

    private Vector3 playerPos;

    Rigidbody rb;

    protected virtual void Awake()
    {
        if (!TimeManager.instance) return;

        if (!TimeManager.instance.IsNight) AnimalState = true;
        else if (TimeManager.instance.IsNight) MonsterState = true;

        TimeManager.instance.Register(this);
        //Debug.Log("Creature Register Complete");

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        isPatrolling = true;
        isChasing = false;
        isAttacking = false;

        //SetNextPatrolPoint();

        rb = GetComponent<Rigidbody>();

        //MJ
        if (_hpUI != null)
        {
            _hpCanvas = _hpUI.GetComponentInChildren<Canvas>();
            _hpSlider = _hpUI.GetComponentInChildren<Slider>();

            _hpSlider.maxValue = _hp;
            _hpSlider.value = _hp;

            _hpSlider.gameObject.SetActive(false);
        }
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

    protected virtual void AnimalAi() // https://docs.unity3d.com/kr/2022.3/Manual/nav-AgentPatrol.html
    {
        Patrol();
    }
    protected virtual void MonsterAi()
    {
        if (!isAttacking)
        {
            if (isPatrolling)
            {
                Patrol();
            }
            else if (isChasing)
            {
                Chase();
            }
        }
    }

    // Patrol()
    protected NavMeshAgent agent;
    [SerializeField] protected float patrolRadius = 5f;
    protected virtual void Patrol()
    {
        agent.speed = 5.0f;
        agent.acceleration = 1.0f;
        agent.angularSpeed = 120.0f;

        Debug.Log("Patrol");
        if (!agent.pathPending && (agent.remainingDistance <= 0.5f))
            SetNextPatrolPoint();

        if (distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
            isPatrolling = false;
        }
    }
    protected virtual void SetNextPatrolPoint()
    {
        //Debug.Log("New Destination Setting");
        Vector3 randomDirection = Random.insideUnitCircle * patrolRadius;

        randomDirection += new Vector3(transform.position.x, 0, transform.position.z);

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    // Chase()
    protected virtual void Chase()
    {

        if (distanceToPlayer > detectionRadius)
        {
            isPatrolling = true;
            isChasing = false;
        }

        if (distanceToPlayer <= attackableRadius)
        {
            StartCoroutine(Attack());
        }
        //agent.speed = 7.0f;
        //agent.angularSpeed = 180f;
        Debug.Log("Chase");

        agent.speed = 5.0f;
        agent.acceleration = 1000.0f;
        agent.angularSpeed = 720.0f;
        //agent.stoppingDistance = 0.0f;

        agent.SetDestination(playerPos);

    }

    // Attack()
    /*
    protected virtual void Attack()
    {
        agent.enabled = false;
        rb.isKinematic = true;
        Debug.Log("Attack");
        transform.Addforce()
    }
    */

    private IEnumerator Attack()
    {
        isAttacking = true;

        agent.enabled = false;
        rb.isKinematic = false;

        Vector3 dashDir = (playerPos - transform.position).normalized;
        rb.AddForce(dashDir * 50.0f, ForceMode.Impulse);

        yield return new WaitForSeconds(1.5f);

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        agent.enabled = true;

        isAttacking = false;
    }

    // + MJ

    /// <summary>
    /// ������ ǥ�� UI
    /// </summary>
    [SerializeField] private GameObject _hpUI;

    private Slider _hpSlider;
    private Canvas _hpCanvas;

    /// <summary>
    /// �÷��̾�� ��ȣ�ۿ��ϰ� ���ʰ� ���� �� UI�� �������
    /// </summary>
    [SerializeField] private float _UIHideTime = 3.0f;
    private Coroutine _hideUICoroutine;


    // AI �⺻ State 
    // Animal Patrol(), RunAway()
    // Monster Patrol(), Chase(), Attack()
    
    // State Stats DetectionRadius, AttackRadius

    protected virtual void Update()
    {
        playerPos = GameObject.FindWithTag("Player").transform.position;
        //Debug.Log(playerPos);

        distanceToPlayer = (playerPos-transform.position).magnitude;
        //Debug.Log(distanceToPlayer);

        if (AnimalState) AnimalAi();
        else if (MonsterState) MonsterAi();


        if (_hpCanvas != null)
        {
            // UI�� ī�޶� �ٶ󺸵��� ���� ��ȯ
            _hpCanvas.transform.rotation = Quaternion.LookRotation(_hpCanvas.transform.position - Camera.main.transform.position);
        }
    }

    /// <summary>
    /// ä�� ��ȣ�ۿ� �� �������� ��� �Լ� (�÷��̾ ��ȣ�ۿ� �� ȣ��)
    /// </summary>
    /// <param name="amount">���� ������ ��ġ</param>
    public override void Interact(object context = null)
    {
        if (context is float amount)
        {
            _hp -= amount;

            if (_hp > 0)
            {
                if (_hpUI != null)
                {
                    _hpSlider.value = _hp;

                    // ��ȣ�ۿ� �ϸ� UI Ȱ��ȭ
                    _hpSlider.gameObject.SetActive(true);

                    if (_hideUICoroutine != null)
                    {
                        StopCoroutine(_hideUICoroutine);
                    }

                    _hideUICoroutine = StartCoroutine(HideUIAfterDelay(_UIHideTime));
                }
            }
            else
            {
                DestroyObject();
            }
        }
    }

    private IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_hpCanvas != null)
        {
            _hpSlider.gameObject.SetActive(false);
        }
    }
}
