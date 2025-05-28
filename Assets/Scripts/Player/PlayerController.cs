using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어 제어와 관련된 동작수행 및 애니메이션
/// 1. Move(움직임)
/// 2. Turn(회전)
/// 3. Attack(공격)
/// 4. Dead(죽음)
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] skillSlots;              // 스킬 슬롯

    private PlayerData playerData;

    readonly private int hashSpeed = Animator.StringToHash("Speed");
    readonly private int hashAttackTrigger = Animator.StringToHash("Attack");
    readonly private int hashDeadTrigger = Animator.StringToHash("Dead");
    readonly private int hashComboCount = Animator.StringToHash("ComboCount");

    private float hAxis;
    private float vAxis;
    private float baseSpeed = 3f;

    private bool isAttackKeyDown;                       // 공격키입력여부(C)
    private bool isAttacking = false;                   // 공격중여부
    private bool isDead = false;                        // 생존여부
    private bool isComboAllowed = false;                // 콤보가능여부

    public bool isCutscenePlaying = false;              // 컷신플레잉 여부

    private Vector3 moveVec;
    private Rigidbody rigid;
    private Animator anim;

    public Animator Anim => anim;

    public int CurComboCount
    {
        get => anim.GetInteger(hashComboCount);
        set => anim.SetInteger(hashComboCount, value);
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        // 플레이어 위치 로딩
        Vector3 loadedPosition = new Vector3
        (
            DataManager.Instance.GetPlayerData().PosX,
            DataManager.Instance.GetPlayerData().PosY,
            DataManager.Instance.GetPlayerData().PosZ
        );

        transform.position = loadedPosition;
    }

    private void Update()
    {
        playerData = DataManager.Instance.GetPlayerData();

        GetInput();
        DoSkill();
        Move();
        Turn();
        Attack();
        ComboAttack();
        Dead();

        // 컷씬동안에는 Idle 애니메이션
        if (isCutscenePlaying)
            anim.SetFloat(hashSpeed, 0);
    }

    // 컴포넌트 초기화
    private void Init()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // 키입력
    private void GetInput()
    {
        if (isDead || isCutscenePlaying)
            return;

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        isAttackKeyDown = Input.GetButtonDown("Attack");
    }

 
    // 플레이어 이동로직
    private void Move()
    {
        if (isAttacking || isDead || isCutscenePlaying)
            return;

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        rigid.position += moveVec * (baseSpeed + playerData.Speed) * Time.deltaTime;
        anim.SetFloat(hashSpeed, moveVec == Vector3.zero ? 0 : (baseSpeed + playerData.Speed));
    }
    
    // 플레이어 회전로직
    private void Turn()
    {
        if (isAttacking || moveVec == Vector3.zero || isDead || isCutscenePlaying)
            return;

        Quaternion newRotation = Quaternion.LookRotation(moveVec);
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRotation, playerData.RotateSpeed * Time.deltaTime);
    }

    // 플레이어 공격 
    private void Attack()
    {
        if (isAttackKeyDown && !isAttacking && !isDead && !isCutscenePlaying)
        {
            anim.SetTrigger(hashAttackTrigger);
        }
    }

    // 플레이어 콤보 공격
    private void ComboAttack()
    {
        if(isAttackKeyDown && isAttacking && isComboAllowed)
        {
            anim.SetTrigger(hashAttackTrigger);
        }
    }

    // 스킬사용
    private void DoSkill()
    {
        if (isAttacking || isDead || isCutscenePlaying)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            skillSlots[0].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            skillSlots[1].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            skillSlots[2].UseSkill();
        }
    }

    // 플레이어 죽음
    private void Dead()
    {
        if(playerData.CurHp <= 0 && !isDead)
        {
            isDead = true;
            anim.SetTrigger(hashDeadTrigger);
        }
    }

    
    #region ** Animation Events **
    // 공격상태 돌입
    private void SetIsAttackingTrue() => isAttacking = true;

    // 공격상태 해제
    private void SetIsAttackingFalse() => isAttacking = false;

    // 콤보 가능
    private void SetIsComboAllowedTrue() => isComboAllowed = true;

    // 콤보 불가능
    private void SetIsComboAllowedFalse() => isComboAllowed = false;

    // 공격판정(Collider) On
    private void EnableAttackHitbox() => WeaponManager.Instance.currentWeapon.SetHitBox(true);

    // 공격판정(Collider) Off
    private void DisableAttackHitbox() => WeaponManager.Instance.currentWeapon.SetHitBox(false);

    // 공격판정(Raycast etc..)
    private void TriggerAttack() => WeaponManager.Instance.currentWeapon.Attack();

    // 공격이펙트 On
    private void EnableEffect() => WeaponManager.Instance.currentWeapon.SetEffect(true);

    // 공격이펙트 Off
    private void DisableEffect() => WeaponManager.Instance.currentWeapon.SetEffect(false);

    // 콤보 카운트 리셋
    private void ResetComboCount() => CurComboCount = 0;

    // 무기 공격효과음 실행
    private void PlayWeaponSfx() => WeaponManager.Instance.currentWeapon.PlayerSfx();

    #endregion
}
