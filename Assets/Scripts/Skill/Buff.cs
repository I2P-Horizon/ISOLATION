using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        Buff
          
            - 버프스킬
            - Buff 스킬 인터페이스 상속
                - FollowingEffect : 버프 이펙트가 플레이어를 따라다님
*/

public class Buff : Skill, IBuffSkill
{
    public Buff(SkillData data) : base(data) { }

    // 스킬 사용
    public override bool Activate(GameObject user)
    {
        // 무기를 장착했는지 여부
        bool hasWeapon = WeaponManager.Instance.currentWeapon != null;

        if (anim == null)
        {
            Debug.Log($"{user} 의 Animator가 존재하지 않음.");
            return false;
        }
        else if (!hasWeapon)
        {
            Debug.Log($"장착한 무기가 없습니다.");
            return false;
        }
        else
        {
            // 애니메이션 설정
            anim.SetTrigger("Skill");
            anim.SetInteger("SkillId", data.AnimId);

            if(cachedEffect == null)
            {
                cachedEffect = UnityEngine.Object.Instantiate(effectPrefab, SkillManager.Instance.gameObject.transform);

                FollowingEffect(user);
            }
            else
            {
                // 생성된 이펙트가 있으면 새로운 위치 지정
                cachedEffect.transform.position = user.transform.position;
                cachedEffect.transform.rotation = user.transform.rotation;
            }

            cachedEffect.SetActive(true);

            // 버프 효과 적용
            SkillManager.Instance.StartCoroutine(EnhanceStatus());
            // 사운드 출력
            AudioManager.Instance.PlaySFX(data.Name, 0.3f);             

            return true;
        }
    }

    // 버프 이펙트가 플레이어를 따라다니도록
    public void FollowingEffect(GameObject user)
    {
        FollowTarget follow = cachedEffect.AddComponent<FollowTarget>();
        follow.target = user.transform;
        follow.duration = data.Cooldown / 2;
    }

    // 버프 : 능력치 상승
    private IEnumerator EnhanceStatus()
    {
        // 공격력, 방어력 증가
        DataManager.Instance.GetPlayerData().Defense += data.Damage;
        DataManager.Instance.GetPlayerData().Damage += data.Damage;

        // 지속시간
        yield return new WaitForSeconds(data.Cooldown / 2);

        // 공격력, 방어력 복구
        DataManager.Instance.GetPlayerData().Defense -= data.Damage;
        DataManager.Instance.GetPlayerData().Damage -= data.Damage;
    }
}
