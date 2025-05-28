using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
                    Monster - ������ ���� �����͸� ����

        - Monster Status & Flags

        - ������Ʈ �ʱ�ȭ

        - ���� �Լ�
            - ReadyToAttack : ���ݰ��ɿ��θ� ��Ÿ���� isAttackReady �÷��׸� True��
            - DeactiveGameObject : ���� ������Ʈ�� ��Ȱ��ȭ
            - GetDamaged : ���� ��������ŭ Hp ����
 */

public class Monster : MonoBehaviour
{
    [SerializeField] private Transform damageTextPos; // ������ �ؽ�Ʈ ǥ�� ��ġ

    #region ** Monster Status **
    [Header("#Monster Stats")]
    public float maxHp;
    public float curHp;
    public float speed;
    public float maxDistance;                       // �÷��̾���� �Ÿ�(�����ϱ����� �ִ�Ÿ�)
    public float idleThreshold;                     // ������ ó�� ��ġ���� ����
    public float attackDelay;                       // ���ݼӵ�
    public float damage;                            // ���ݷ�
    public float attackRange;                       // ���ݰ����� ����
    #endregion

    #region ** Flags **
    //[HideInInspector]
    public bool isReset;                            // �������� �����ߴ��� ����
    [HideInInspector]
    public bool isAttackReady;                      // ���� ���� ����
    [HideInInspector]
    public bool isDead;                             // �׾����� ����
    #endregion

    #region ** Private Fields **
    private Vector3 startPosition;                  // ������ ù ��ġ
    private PlayerController targetPlayer;          // Ÿ�� �÷��̾�
    private BoxCollider hitBoxCol;                  // ���� ��Ʈ�ڽ�
    private Animator anim;                          // ���� �ִϸ�����
    private NavMeshAgent nav;                       // ���� �׺���̼�
    #endregion

    #region ** Properties **
    public Animator Anim => anim;
    public NavMeshAgent Nav => nav;
    public PlayerController Target => targetPlayer;
    public Vector3 StartPosition => startPosition;
    public BoxCollider HitBox => hitBoxCol;
    #endregion
    
    protected virtual void Awake()
    {
        Init();
    }

    private void Init()
    {
        targetPlayer = GameManager.Instance.player;
        startPosition = transform.position;

        isAttackReady = true;
        isReset = true;

        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        hitBoxCol = GetComponent<BoxCollider>();
    }


    // ���� ���ɻ��·� ��ȯ
    public void ReadyToAttack()
    {
        isAttackReady = true;
    }

    // ����
    public void DeactiveGameObject()
    {
        Destroy(this.gameObject);
    }

    // ���ݹ���
    public void GetDamaged(float damage)
    {
        float minDamage = damage * 0.8f;
        float maxDamage = damage * 1.2f;
        int randomDamage = (int)Random.Range(minDamage, maxDamage);
        DamageTextManager.Instance.ShowDamage(damageTextPos.position, randomDamage);
        curHp -= randomDamage;
    }

}
