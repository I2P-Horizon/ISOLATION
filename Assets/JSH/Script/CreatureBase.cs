using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class CreatureBase : DestructibleObject, ICycleListener// Creture�� ���ΰ�, �߻� Ŭ���� Object���� ��� �޾� ���
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
        //Debug.Log("Creature Register Complete");


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
    protected NavMeshAgent agent;
    [SerializeField] protected float patrolRadius = 5f;

    protected virtual void SetNextPatrolPoint() 
    {
        //Debug.Log("New Destination Setting");
        Vector3 randomDirection = Random.insideUnitCircle * patrolRadius;

        randomDirection += new Vector3 (transform.position.x, 0, transform.position.z);

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    protected virtual void AnimalAi() { }
    protected virtual void MonsterAi() { }

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

    protected virtual void Update()
    {
        if (_hpCanvas != null)
       {
            //Debug.Log("��ư ��");
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
