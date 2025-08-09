using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class CreatureBase : DestructibleObject, ICycleListener//추상 클래스 DestructibleObject에서 상속 받아 사용
{
    [Header("AnimalState")]
    [SerializeField] protected bool _animalState;
    [SerializeField] private GameObject _animalShape;


    [Header("MonsterState")]
    [SerializeField] protected bool _monsterState;
    [SerializeField] private GameObject _monsterShape;

    [Header("State")]
    [SerializeField] protected bool _isPatrolling;
    [SerializeField] protected bool _isChasing;
    [SerializeField] protected bool _isAttacking;

    [Header("Stats")]
    [SerializeField] protected float _moveSpeed = 5.0f;
    [SerializeField] protected float _defense;
    [SerializeField] protected float _detectionRadius = 5.0f;
    [SerializeField] protected float _attackableRadius = 2.0f;
    [SerializeField] protected float _attackDamage;
    [SerializeField] protected float _attackSpeed;

    [SerializeField] protected float _distanceToPlayer;

    [SerializeField] protected GameObject _animalStateDropItem;
    [SerializeField] protected GameObject _monsterStateDropItem;

    [SerializeField] protected float _animalHp;
    [SerializeField] protected float _monsterHp;

    Transform _playerTransform;
    Vector3 _playerPos;

    Rigidbody rb;

    void Awake()
    {

        //Debug.Log("Creature Register Complete");

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;

        _isPatrolling = true;
        _isChasing = false;
        _isAttacking = false;

        //SetNextPatrolPoint();

        rb = GetComponent<Rigidbody>();
        MeshCollider collider = GetComponentInChildren<MeshCollider>();

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
    private void Start()
    {
        if (!TimeManager.instance) return;
        TimeManager.instance.Register(this);

        if (!TimeManager.instance.IsNight) _animalState = true;
        else if (TimeManager.instance.IsNight) _monsterState = true;
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        _agent = GetComponent<NavMeshAgent>();
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
            _animalState = true;
            _monsterState = false;
            ToAnimalState();
        }

        else if(isNight)
        {
            //Debug.Log("Monster");
            _animalState = false;
            _monsterState = true;
            ToMonsterState();
        }
    }

    private void ToAnimalState()
    {
        _animalShape.SetActive(true);
        _monsterShape.SetActive(false);
        _dropItem = _animalStateDropItem;
        _hp = _animalHp;
    }
    private void ToMonsterState()
    {
        _animalShape.SetActive(false);
        _monsterShape.SetActive(true);
        _dropItem = _monsterStateDropItem;
        _hp = _monsterHp;
    }

    

    // AI

    protected virtual void AnimalAi() // https://docs.unity3d.com/kr/2022.3/Manual/nav-AgentPatrol.html
    {
        Patrol();
    }
    protected virtual void MonsterAi()
    {
        if (!_isAttacking)
        {
            if (_isPatrolling)
            {
                Patrol();
            }
            else if (_isChasing)
            {
                Chase();
            }
        }
    }

    // Patrol()
    protected NavMeshAgent _agent;
    [SerializeField] protected float patrolRadius = 5f;
    protected virtual void Patrol()
    {
        _agent.speed = 5.0f;
        _agent.acceleration = 1.0f;
        _agent.angularSpeed = 120.0f;

        //Debug.Log("Patrol");
        if (!_agent.pathPending && (_agent.remainingDistance <= 0.5f))
            SetNextPatrolPoint();

        if (_distanceToPlayer <= _detectionRadius)
        {
            _isChasing = true;
            _isPatrolling = false;
        }
    }
    protected virtual void SetNextPatrolPoint()
    {
        //Debug.Log("New Destination Setting");
        int randomHeight = Random.Range(1, 7);
        Vector2 randomRadius = Random.insideUnitCircle * patrolRadius;
        Vector3 randomDirection = new Vector3(transform.position.x + randomRadius.x, randomHeight, transform.position.z + randomRadius.y);
        //Debug.Log($"{randomDirection} + {randomDirection / patrolRadius}");

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }

    // Chase()
    protected virtual void Chase()
    {

        if (_distanceToPlayer > _detectionRadius)
        {
            _isPatrolling = true;
            _isChasing = false;
        }

        if (_distanceToPlayer <= _attackableRadius)
        {
            StartCoroutine(Attack());
        }
        //agent.speed = 7.0f;
        //agent.angularSpeed = 180f;
        //Debug.Log("Chase");

        if (_isAttacking == false)
        {
            _agent.speed = 5.0f;
            _agent.acceleration = 1000.0f;
            _agent.angularSpeed = 720.0f;
            //agent.stoppingDistance = 0.0f;

            _playerPos.y = transform.position.y;
            _agent.SetDestination(_playerPos);
        }
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
        _isAttacking = true;

        _agent.enabled = false;
        rb.isKinematic = false;

        Vector3 dashDir = (_playerPos - transform.position).normalized;
        dashDir.y = 0.3f;
        rb.AddForce((dashDir) * 15.0f, ForceMode.Impulse);

        yield return new WaitForSeconds(2.5f);

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        _agent.enabled = true;

        _isAttacking = false;
    }

    // + MJ

    /// <summary>
    /// 내구도 표시 UI
    /// </summary>
    [SerializeField] private GameObject _hpUI;

    private Slider _hpSlider;
    private Canvas _hpCanvas;

    /// <summary>
    /// 플레이어와 상호작용하고 몇초가 지난 후 UI를 숨길건지
    /// </summary>
    [SerializeField] private float _UIHideTime = 3.0f;
    private Coroutine _hideUICoroutine;


    // AI 기본 State 
    // Animal Patrol(), RunAway()
    // Monster Patrol(), Chase(), Attack()
    
    // State Stats DetectionRadius, AttackRadius

    void Update()
    {
        _playerPos = _playerTransform.transform.position;
        //Debug.Log(playerPos);

        _distanceToPlayer = (_playerPos-transform.position).magnitude;
        //Debug.Log(distanceToPlayer);

        if (_animalState) AnimalAi();
        else if (_monsterState) MonsterAi();


        if (_hpCanvas != null)
        {
            // UI가 카메라를 바라보도록 방향 전환
            _hpCanvas.transform.rotation = Quaternion.LookRotation(_hpCanvas.transform.position - Camera.main.transform.position);
        }
    }

    /// <summary>
    /// 채집 상호작용 시 내구도를 깎는 함수 (플레이어가 상호작용 시 호출)
    /// </summary>
    /// <param name="amount">줄일 내구도 수치</param>
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

                    // 상호작용 하면 UI 활성화
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
