using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
                    Staff : ����(������) Ŭ����

            - RayCast�� ����ؼ� �������� ����
 */

public class Staff : Weapon
{
    private float attackRange = 3f;             // ���� ����
    private float attackRadius = 3f;            // ���� �ݰ� 

    private Vector3 attackOrigin;               // ���� ������ġ
    private Vector3 attackDir;                  // ���� ����

   

    private void Awake()
    {
        type = WeaponType.Staff;

        soundId = "Staff";
    }

    // ���� ����
    public override void Attack()
    {
        // ���ݽ��� ��ġ : �÷��̾�ణ��, ���ݹ��� : �÷��̾� ����
        attackOrigin = GameManager.Instance.player.transform.position + GameManager.Instance.player.transform.forward * 2f;
        attackDir = GameManager.Instance.player.transform.forward;
        
        // ����
        RaycastHit[] hits = Physics.SphereCastAll(
            attackOrigin,
            attackRadius,
            attackDir,
            attackRange,
            LayerMask.GetMask("Monster", "BossMonster")
        );

        // ���ݹ����� ���Ͱ� �������
        foreach(RaycastHit hit in hits)
        {
            if(hit.collider.CompareTag("Monster"))
            {
                Monster monster = hit.collider.GetComponent<Monster>();
                if (monster != null)
                {
                    // .. ���Ϳ��� ������
                    Debug.Log("���͸� ���߾���.");
                    monster.GetDamaged(DataManager.Instance.GetPlayerData().Damage * Random.Range(0.8f, 1f));
                }
            }
            else if(hit.collider.CompareTag("BossMonster"))
            {
                BossMonster boss = hit.collider.GetComponent<BossMonster>();
                if (boss != null)
                {
                    // .. ���Ϳ��� ������
                    Debug.Log("���͸� ���߾���.");
                    boss.GetDamaged(DataManager.Instance.GetPlayerData().Damage * Random.Range(0.8f, 1f));
                }
            }
        }
    }

    public override void SetHitBox(bool isEnabled)
    {
        
    }

    public override void SetEffect(bool isEnabled)
    {
        
    }

    public override void PlayerSfx()
    {
        AudioManager.Instance.PlaySFX(soundId);
    }

    // ���ݹ��� �ð�ȭ
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin, attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(attackOrigin, attackOrigin + attackDir * attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackOrigin + attackDir * attackRange, attackRadius);
    }

    
}
