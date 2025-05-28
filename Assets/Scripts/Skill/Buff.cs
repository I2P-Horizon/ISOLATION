using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        Buff
          
            - ������ų
            - Buff ��ų �������̽� ���
                - FollowingEffect : ���� ����Ʈ�� �÷��̾ ����ٴ�
*/

public class Buff : Skill, IBuffSkill
{
    public Buff(SkillData data) : base(data) { }

    // ��ų ���
    public override bool Activate(GameObject user)
    {
        // ���⸦ �����ߴ��� ����
        bool hasWeapon = WeaponManager.Instance.currentWeapon != null;

        if (anim == null)
        {
            Debug.Log($"{user} �� Animator�� �������� ����.");
            return false;
        }
        else if (!hasWeapon)
        {
            Debug.Log($"������ ���Ⱑ �����ϴ�.");
            return false;
        }
        else
        {
            // �ִϸ��̼� ����
            anim.SetTrigger("Skill");
            anim.SetInteger("SkillId", data.AnimId);

            if(cachedEffect == null)
            {
                cachedEffect = UnityEngine.Object.Instantiate(effectPrefab, SkillManager.Instance.gameObject.transform);

                FollowingEffect(user);
            }
            else
            {
                // ������ ����Ʈ�� ������ ���ο� ��ġ ����
                cachedEffect.transform.position = user.transform.position;
                cachedEffect.transform.rotation = user.transform.rotation;
            }

            cachedEffect.SetActive(true);

            // ���� ȿ�� ����
            SkillManager.Instance.StartCoroutine(EnhanceStatus());
            // ���� ���
            AudioManager.Instance.PlaySFX(data.Name, 0.3f);             

            return true;
        }
    }

    // ���� ����Ʈ�� �÷��̾ ����ٴϵ���
    public void FollowingEffect(GameObject user)
    {
        FollowTarget follow = cachedEffect.AddComponent<FollowTarget>();
        follow.target = user.transform;
        follow.duration = data.Cooldown / 2;
    }

    // ���� : �ɷ�ġ ���
    private IEnumerator EnhanceStatus()
    {
        // ���ݷ�, ���� ����
        DataManager.Instance.GetPlayerData().Defense += data.Damage;
        DataManager.Instance.GetPlayerData().Damage += data.Damage;

        // ���ӽð�
        yield return new WaitForSeconds(data.Cooldown / 2);

        // ���ݷ�, ���� ����
        DataManager.Instance.GetPlayerData().Defense -= data.Damage;
        DataManager.Instance.GetPlayerData().Damage -= data.Damage;
    }
}
