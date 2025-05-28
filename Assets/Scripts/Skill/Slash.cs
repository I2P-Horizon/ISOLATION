using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        Slash
          
            - 검스킬
            - 공격판정 : SphereCast
*/

public class Slash : Skill
{
    public Slash(SkillData data) : base(data) { }
    private float attackRadius = 2f;
    private float attackRange = 1f;
    
    // 스킬 사용(공격판정, 이펙트, 사운드)
    public override bool Activate(GameObject user)
    {
        // 올바른 무기를 장착했는지 여부
        bool hasWeapon = WeaponManager.Instance.currentWeapon.type == WeaponType.Sword;

        if (anim == null)
        {
            Debug.Log($"{user} 의 Animator가 존재하지 않음.");
            return false;
        }
        else if(!hasWeapon)
        {
            Debug.Log($"장착한 무기로는 스킬을 사용할 수 없습니다.");
            return false;
        }
        else
        { 
            anim.SetTrigger("Skill");
            anim.SetInteger("SkillId", data.AnimId);

            if(cachedEffect == null)
            {
                // 생성된 이펙트가 없으면 생성
                cachedEffect = UnityEngine.Object.Instantiate(effectPrefab,
                user.transform.position + new Vector3(0,1,0),
                user.transform.rotation, SkillManager.Instance.gameObject.transform);
            }
            else
            {
                // 생성된 이펙트가 있으면 새로운 위치 지정
                cachedEffect.transform.position = user.transform.position + new Vector3(0, 1, 0);
                cachedEffect.transform.rotation = user.transform.rotation;
            }

            cachedEffect.SetActive(true);
            SkillManager.Instance.StartCoroutine(Attack());
            AudioManager.Instance.PlaySFX(data.Name, 0.3f);     // 공격음 재생
            return true;
        }
    }

    // 공격판정(SphereCast 사용)
    private void EnableHitBox()
    {
        Vector3 origin = GameManager.Instance.player.transform.position + GameManager.Instance.player.transform.forward * 2f;
        Vector3 dir = GameManager.Instance.player.transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            attackRadius,
            dir,
            attackRange,
            LayerMask.GetMask("Monster", "BossMonster")
        );

        foreach(RaycastHit hit in hits)
        {
            if(hit.collider.CompareTag("Monster"))
            {
                Monster monster = hit.collider.GetComponent<Monster>();
                if (monster != null)
                {
                    // 데미지
                    monster.GetDamaged((DataManager.Instance.GetPlayerData().Damage + data.Damage) * Random.Range(0.8f, 1f));
                }
            }
            else if(hit.collider.CompareTag("BossMonster"))
            {
                BossMonster boss = hit.collider.GetComponent<BossMonster>();
                if (boss != null)
                {
                    // 데미지
                    boss.GetDamaged((DataManager.Instance.GetPlayerData().Damage + data.Damage) * Random.Range(0.8f, 1f));
                }
            }
        }
    }
    
    // 애니메이션에 맞춰 공격판정 ON
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.5f);

        EnableHitBox();
    }
}
