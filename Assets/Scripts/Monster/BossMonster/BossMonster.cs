using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
                    BossMonster - ���������� ���� �����͸� ����

        - Boss Monster ������Ʈ ���� ��������

        - �����Լ�
            - GetDamaged : ���� ��������ŭ Hp ����
 */

public class BossMonster : MonoBehaviour
{
    [SerializeField] private Transform damageTextPos; // ������ �ؽ�Ʈ ǥ�� ��ġ

    #region ** Events **
    public event System.Action OnBossDied;

    #endregion
    #region ** Monster Status **
    [Header("#Boss Monster Stats")]
    public float maxHp;
    public float curHp;
    public float speed;
    public float damage;                            // ���ݷ�
    public float attackRange;                       // ���ݰ����� ����
    #endregion

    #region ** Private Fields **
    public PlayerController targetPlayer;             // Ÿ�� �÷��̾�
    protected BoxCollider hitBoxCol;                  // ���� ��Ʈ�ڽ�
    protected Animator anim;                          // ���� �ִϸ�����
    protected NavMeshAgent nav;                       // ���� �׺���̼�
    #endregion

    #region ** Flags **
    [HideInInspector]
    public bool isAttackReady;                      // ���� ���� ����
    [HideInInspector]
    protected bool isDead;                             // �׾����� ����
    [HideInInspector]
    public bool isAttacking = false;                // ���������� ����
    #endregion

    #region ** Properties **
    public NavMeshAgent Nav => nav;
    #endregion

    protected virtual void Awake()
    {
        Init();
    }

    private void Init()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        hitBoxCol = GetComponent<BoxCollider>();
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

    public virtual void Die()
    {
        OnBossDied?.Invoke();
    }
}
