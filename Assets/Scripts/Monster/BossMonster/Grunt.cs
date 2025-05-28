using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Grunt : BossMonster
{
    [SerializeField] private GameObject thunderboltEffect;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject explosionAttacker;

    readonly private int hashAttackTrigger = Animator.StringToHash("Attack");
    readonly private int hashAttackType = Animator.StringToHash("AttackType");
    readonly private int hashDeadTrigger = Animator.StringToHash("Dead");
    readonly private int hashSpeed = Animator.StringToHash("Speed");

    private int prevAttack = 0;
    private CinemachineBasicMultiChannelPerlin noise;           // ī�޶� ������(��鸲)

    protected override void Awake()
    {
        // �θ�(Monster)�� �ʱ�ȭ
        base.Awake();
        InitData();
    }


    private void Update()
    {
        if (targetPlayer == null || isDead)
            return;

        Move();
        Attack();
        Die();
    }

    // ���� ������ �ʱ�ȭ
    private void InitData()
    {
        maxHp = 250;
        curHp = maxHp;
        speed = 1.5f;
        damage = 15f;
        attackRange = 3f;

        nav.speed = speed;

        noise = GameManager.Instance.virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // �̵�
    private void Move()
    {
        if (isAttacking)
            return;

        nav.SetDestination(targetPlayer.transform.position);
        anim.SetFloat(hashSpeed, speed);
    }

    // ����
    private void Attack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
        Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;

        // ���ݹ����ȿ� ������ ��
        if (!isAttacking && distanceToPlayer <= attackRange)
        {
            transform.LookAt(targetPlayer.transform);
            StartCoroutine(DecideNextAttack());
        }
    }

    // ����
    public override void Die()
    {
        if(!isDead && curHp <= 0)
        {
            base.Die();
            isDead = true;
            anim.SetTrigger(hashDeadTrigger);
            PlaySFX("Grunt_Die");
            hitBoxCol.enabled = false;
            nav.isStopped = true;
        }
    }

    #region ** Coroutines **
    // ���� ���� ���ϱ�
    private IEnumerator DecideNextAttack()
    {
        nav.isStopped = true;
        isAttacking = true;
        anim.SetFloat(hashSpeed, 0);

        int randomAct = Random.Range(0, 4);

        // ���ݻ��� ����
        yield return new WaitForSeconds(1f);

        // ���� ������ �����ϰ� ����
        switch(randomAct)
        {
            case 0:
                PlaySFX("Grunt_Attack01");
                anim.SetInteger(hashAttackType, randomAct);
                anim.SetTrigger(hashAttackTrigger);
                prevAttack = 0;
                break;
            case 1:
                anim.SetInteger(hashAttackType, randomAct);
                anim.SetTrigger(hashAttackTrigger);
                prevAttack = 1;
                break;
            case 2:
                if(prevAttack == 2)
                {
                    StopCoroutine(DecideNextAttack());
                    StartCoroutine(DecideNextAttack());
                }
                else
                {
                    PlaySFX("Grunt_Attack03");
                    anim.SetInteger(hashAttackType, randomAct);
                    anim.SetTrigger(hashAttackTrigger);
                    StartCoroutine(Thunderbolt());
                    prevAttack = 2;
                }
                break;
            case 3:
                if(prevAttack == 3)
                {
                    StopCoroutine(DecideNextAttack());
                    StartCoroutine(DecideNextAttack());
                }
                else
                {
                    PlaySFX("Grunt_Attack04");
                    anim.SetInteger(hashAttackType, randomAct);
                    anim.SetTrigger(hashAttackTrigger);
                    StartCoroutine(Explosion());
                    prevAttack = 3;
                }
                break;
        }

        yield return null;
    }

    // ��ų ����1
    private IEnumerator Thunderbolt()
    {
        thunderboltEffect.TryGetComponent<GruntThunderbolt>(out GruntThunderbolt hitbox);
        if (hitbox == null)
        {
            hitbox = thunderboltEffect.AddComponent<GruntThunderbolt>();
            hitbox.damage = damage;
        }
        noise.m_AmplitudeGain = 1f;             // ī�޶� ��鸲 ON
        thunderboltEffect.SetActive(true);

        yield return new WaitForSeconds(3f);
        noise.m_AmplitudeGain = 0f;             // ī�޶� ��鸲 Off
        thunderboltEffect.SetActive(false);
    }
    
    // ��ų ����2
    private IEnumerator Explosion()
    {
        explosionAttacker.TryGetComponent<GruntExplosion>(out GruntExplosion hitbox);
        if(hitbox == null)
        {
            hitbox = explosionAttacker.AddComponent<GruntExplosion>();
            hitbox.damage = damage * Random.Range(0.3f, 0.7f);
        }
        explosionEffect.transform.position = GameManager.Instance.player.transform.position;
        explosionEffect.SetActive(true);

        yield return new WaitForSeconds(1f);
        explosionAttacker.transform.position = GameManager.Instance.player.transform.position + GameManager.Instance.player.transform.up * 2f;
        explosionAttacker.SetActive(true);

        yield return new WaitForSeconds(1f);
        explosionAttacker.SetActive(false);
        explosionEffect.SetActive(false);

        EndAttack();
    }

    // ���� ���� ����(�ִϸ��̼� �̺�Ʈ)
    private void MeleeAttack()
    {
        // Raycast�� ��ġ, ����
        Vector3 origin = transform.position + new Vector3(0, 1f, 0);
        Vector3 direction = transform.forward;

        RaycastHit hit;

        if(Physics.SphereCast(origin, 1f, direction, out hit, 2f, LayerMask.GetMask("Player")))
        {
            if(hit.collider.CompareTag("Player"))
            {
                // �÷��̾� ������
                PlayerData playerData = DataManager.Instance.GetPlayerData();
                playerData.GetDamaged(Random.Range(damage * 0.8f, damage * 1.2f));
            }
        }
    }
    #endregion

    #region ** Animation Events **
    // ���� ��(�ִϸ��̼� �̺�Ʈ)
    private void EndAttack()
    {
        isAttacking = !isAttacking;
        nav.isStopped = false;
    }

    // ȿ���� ���
    private void PlaySFX(string soundId)
    {
        AudioManager.Instance.PlaySFX(soundId);
    }
    #endregion
}
