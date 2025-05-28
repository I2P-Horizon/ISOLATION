using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
                    Staff : 무기(스태프) 클래스

            - RayCast를 사용해서 공격판정 구현
 */

public class Staff : Weapon
{
    private float attackRange = 3f;             // 공격 범위
    private float attackRadius = 3f;            // 공격 반경 

    private Vector3 attackOrigin;               // 공격 시작위치
    private Vector3 attackDir;                  // 공격 방향

   

    private void Awake()
    {
        type = WeaponType.Staff;

        soundId = "Staff";
    }

    // 공격 구현
    public override void Attack()
    {
        // 공격시작 위치 : 플레이어약간앞, 공격방향 : 플레이어 정면
        attackOrigin = GameManager.Instance.player.transform.position + GameManager.Instance.player.transform.forward * 2f;
        attackDir = GameManager.Instance.player.transform.forward;
        
        // 공격
        RaycastHit[] hits = Physics.SphereCastAll(
            attackOrigin,
            attackRadius,
            attackDir,
            attackRange,
            LayerMask.GetMask("Monster", "BossMonster")
        );

        // 공격범위에 몬스터가 있을경우
        foreach(RaycastHit hit in hits)
        {
            if(hit.collider.CompareTag("Monster"))
            {
                Monster monster = hit.collider.GetComponent<Monster>();
                if (monster != null)
                {
                    // .. 몬스터에게 데미지
                    Debug.Log("몬스터를 맞추었음.");
                    monster.GetDamaged(DataManager.Instance.GetPlayerData().Damage * Random.Range(0.8f, 1f));
                }
            }
            else if(hit.collider.CompareTag("BossMonster"))
            {
                BossMonster boss = hit.collider.GetComponent<BossMonster>();
                if (boss != null)
                {
                    // .. 몬스터에게 데미지
                    Debug.Log("몬스터를 맞추었음.");
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

    // 공격범위 시각화
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
