using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class CreatureBase : DestructibleObject, ICycleListener
{
    private Rigidbody rb;

    [Header("AnimalState")]
    [SerializeField] private bool _animalState = false;
    [SerializeField] private GameObject _animalShape;

    [Header("MonsterState")]
    [SerializeField] private bool _monsterState = false;
    [SerializeField] private GameObject _monsterShape;   

    [Header("Stats")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _defense;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _attackableRadius;
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackSpeed;

    [SerializeField] protected float _distanceToPlayer;

    [SerializeField] private GameObject _animalStateDropItem;
    [SerializeField] private GameObject _monsterStateDropItem;

    [SerializeField] private float _animalStateHp;
    [SerializeField] private float _monsterStateHp;

    // AI
    private NavMeshAgent _agent;
    private const float patrolRadius = 10.0f;

    private Transform _playerTransform;
    private Vector3 _playerPos;

    [Header("State")]
    [SerializeField] private bool _isPatrolling;
    [SerializeField] private bool _isChasing;
    [SerializeField] private bool _isAttacking;
    


    private void Awake()
    {
        InitAIConfig();
        InitHpUI();

        _playerTransform = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        MeshCollider collider = GetComponentInChildren<MeshCollider>();
    }

    protected virtual void OnEnable()
    {
        if (!TimeManager.Instance) return;
        TimeManager.Instance.Register(this);

        if (!TimeManager.Instance.IsNight) _animalState = true;
        else if (TimeManager.Instance.IsNight) _monsterState = true;

        if (!_playerTransform) 
            _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        UpdateAILogic();
        UpdateHpUI();
    }

    private void OnDisable()
    {
        if (!TimeManager.Instance) return;
        TimeManager.Instance.UnRegister(this);
    }



    public void OnCycleChanged()
    {
        ChangeCreatureState();
    }

    protected void InitStats(float animalStateHp, float monsterStateHp, float moveSpeed, float defense, 
        float detectionRadius, float attackableRadius, float attackDamage, float attackSpeed)
    {
        _animalStateHp = animalStateHp;
        _monsterStateHp = monsterStateHp;
        _moveSpeed = moveSpeed;
        _defense = defense;
        _detectionRadius = detectionRadius;
        _attackableRadius = attackableRadius;
        _attackDamage = attackDamage;
        _attackSpeed = attackSpeed;
    }

    private void InitAIConfig()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;

        _isPatrolling = true;
        _isChasing = false;
        _isAttacking = false;
    }

    private void UpdateAILogic()
    {
        _playerPos = _playerTransform.transform.position;
        //Debug.Log(playerPos);

        _distanceToPlayer = (_playerPos - transform.position).magnitude;
        //Debug.Log(distanceToPlayer);

        if (_animalState) AnimalAi();
        else if (_monsterState) MonsterAi();
    }
    private void UpdateHpUI()
    {
        if (_hpCanvas != null)
        {
            _hpCanvas.transform.rotation = Quaternion.LookRotation(_hpCanvas.transform.position - Camera.main.transform.position);
        }
    }

    private void ChangeCreatureState()
    {
        if (!TimeManager.Instance.IsNight)
            ToAnimalState();

        else if (TimeManager.Instance.IsNight)
            ToMonsterState();
    }

    private void ToAnimalState()
    {
        _animalState = true;
        _monsterState = false;

        _animalShape.SetActive(true);
        _monsterShape.SetActive(false);
        _dropItem = _animalStateDropItem;
        _hp = _animalStateHp;
    }
    private void ToMonsterState()
    {
        _animalState = false;
        _monsterState = true;

        _animalShape.SetActive(false);
        _monsterShape.SetActive(true);
        _dropItem = _monsterStateDropItem;
        _hp = _monsterStateHp;
    }

    // JSH TODO AI Culling
    private void AnimalAi()
    {
        Patrol();
    }
    private void MonsterAi()
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
    protected virtual void Patrol()
    {
        _agent.speed = _moveSpeed;
        _agent.acceleration = 1.0f;
        _agent.angularSpeed = 120.0f;

        //Debug.Log("Patrol");
        if (_agent.isOnNavMesh && !_agent.pathPending && (_agent.remainingDistance <= 0.5f))
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

    protected virtual void Chase()
    {
        //Debug.Log("Chase");
        if (_distanceToPlayer > _detectionRadius)
        {
            _isPatrolling = true;
            _isChasing = false;
        }

        if (_distanceToPlayer <= _attackableRadius) 
            Attack();

        if (_isAttacking == false)
        {
            //_agent.speed = 5.0f;
            _agent.acceleration = 1000.0f;
            _agent.angularSpeed = 720.0f;

            _playerPos.y = transform.position.y;
            _agent.SetDestination(_playerPos);
        }
    }

    private void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
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




    // SMJ
    [SerializeField] private GameObject _hpUI;

    private Slider _hpSlider;
    private Canvas _hpCanvas;
    private float _UIHideTime = 3.0f;
    private Coroutine _hideUICoroutine;
    private void InitHpUI()
    {
        // SMJ
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
        if (!TimeManager.Instance || !CreaturePoolsManager.Instance) return;

        TimeManager.Instance.UnRegister(this);
        CreaturePoolsManager.Instance.OnCreatureDie(gameObject);

        Debug.Log("Creature UnRegister & Back to pool Complete");
    }

    // JSH TODO: Add condition based on player's equipped equipment
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
