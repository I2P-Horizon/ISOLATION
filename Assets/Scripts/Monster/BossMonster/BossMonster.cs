using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
                    BossMonster - 보스몬스터의 공통 데이터를 관리

        - Boss Monster 컴포넌트 정보 가져오기

        - 공통함수
            - GetDamaged : 받은 데미지만큼 Hp 감소
 */

public class BossMonster : MonoBehaviour
{
    [SerializeField] private Transform damageTextPos; // 데미지 텍스트 표시 위치

    #region ** Events **
    public event System.Action OnBossDied;

    #endregion
    #region ** Monster Status **
    [Header("#Boss Monster Stats")]
    public float maxHp;
    public float curHp;
    public float speed;
    public float damage;                            // 공격력
    public float attackRange;                       // 공격가능한 범위
    #endregion

    #region ** Private Fields **
    public PlayerController targetPlayer;             // 타깃 플레이어
    protected BoxCollider hitBoxCol;                  // 몬스터 히트박스
    protected Animator anim;                          // 몬스터 애니메이터
    protected NavMeshAgent nav;                       // 몬스터 네비게이션
    #endregion

    #region ** Flags **
    [HideInInspector]
    public bool isAttackReady;                      // 공격 가능 여부
    [HideInInspector]
    protected bool isDead;                             // 죽었는지 여부
    [HideInInspector]
    public bool isAttacking = false;                // 공격중인지 여부
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

    // 공격받음
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
