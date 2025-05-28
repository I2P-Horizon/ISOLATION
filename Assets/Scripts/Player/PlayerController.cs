using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// �÷��̾� ����� ���õ� ���ۼ��� �� �ִϸ��̼�
/// 1. Move(������)
/// 2. Turn(ȸ��)
/// 3. Attack(����)
/// 4. Dead(����)
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] skillSlots;              // ��ų ����

    private PlayerData playerData;

    readonly private int hashSpeed = Animator.StringToHash("Speed");
    readonly private int hashAttackTrigger = Animator.StringToHash("Attack");
    readonly private int hashDeadTrigger = Animator.StringToHash("Dead");
    readonly private int hashComboCount = Animator.StringToHash("ComboCount");

    private float hAxis;
    private float vAxis;
    private float baseSpeed = 3f;

    private bool isAttackKeyDown;                       // ����Ű�Է¿���(C)
    private bool isAttacking = false;                   // �����߿���
    private bool isDead = false;                        // ��������
    private bool isComboAllowed = false;                // �޺����ɿ���

    public bool isCutscenePlaying = false;              // �ƽ��÷��� ����

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
        // �÷��̾� ��ġ �ε�
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

        // �ƾ����ȿ��� Idle �ִϸ��̼�
        if (isCutscenePlaying)
            anim.SetFloat(hashSpeed, 0);
    }

    // ������Ʈ �ʱ�ȭ
    private void Init()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Ű�Է�
    private void GetInput()
    {
        if (isDead || isCutscenePlaying)
            return;

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        isAttackKeyDown = Input.GetButtonDown("Attack");
    }

 
    // �÷��̾� �̵�����
    private void Move()
    {
        if (isAttacking || isDead || isCutscenePlaying)
            return;

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        rigid.position += moveVec * (baseSpeed + playerData.Speed) * Time.deltaTime;
        anim.SetFloat(hashSpeed, moveVec == Vector3.zero ? 0 : (baseSpeed + playerData.Speed));
    }
    
    // �÷��̾� ȸ������
    private void Turn()
    {
        if (isAttacking || moveVec == Vector3.zero || isDead || isCutscenePlaying)
            return;

        Quaternion newRotation = Quaternion.LookRotation(moveVec);
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRotation, playerData.RotateSpeed * Time.deltaTime);
    }

    // �÷��̾� ���� 
    private void Attack()
    {
        if (isAttackKeyDown && !isAttacking && !isDead && !isCutscenePlaying)
        {
            anim.SetTrigger(hashAttackTrigger);
        }
    }

    // �÷��̾� �޺� ����
    private void ComboAttack()
    {
        if(isAttackKeyDown && isAttacking && isComboAllowed)
        {
            anim.SetTrigger(hashAttackTrigger);
        }
    }

    // ��ų���
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

    // �÷��̾� ����
    private void Dead()
    {
        if(playerData.CurHp <= 0 && !isDead)
        {
            isDead = true;
            anim.SetTrigger(hashDeadTrigger);
        }
    }

    
    #region ** Animation Events **
    // ���ݻ��� ����
    private void SetIsAttackingTrue() => isAttacking = true;

    // ���ݻ��� ����
    private void SetIsAttackingFalse() => isAttacking = false;

    // �޺� ����
    private void SetIsComboAllowedTrue() => isComboAllowed = true;

    // �޺� �Ұ���
    private void SetIsComboAllowedFalse() => isComboAllowed = false;

    // ��������(Collider) On
    private void EnableAttackHitbox() => WeaponManager.Instance.currentWeapon.SetHitBox(true);

    // ��������(Collider) Off
    private void DisableAttackHitbox() => WeaponManager.Instance.currentWeapon.SetHitBox(false);

    // ��������(Raycast etc..)
    private void TriggerAttack() => WeaponManager.Instance.currentWeapon.Attack();

    // ��������Ʈ On
    private void EnableEffect() => WeaponManager.Instance.currentWeapon.SetEffect(true);

    // ��������Ʈ Off
    private void DisableEffect() => WeaponManager.Instance.currentWeapon.SetEffect(false);

    // �޺� ī��Ʈ ����
    private void ResetComboCount() => CurComboCount = 0;

    // ���� ����ȿ���� ����
    private void PlayWeaponSfx() => WeaponManager.Instance.currentWeapon.PlayerSfx();

    #endregion
}
