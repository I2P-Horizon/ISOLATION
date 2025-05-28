using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        Slash
          
            - �˽�ų
            - �������� : SphereCast
*/

public class Slash : Skill
{
    public Slash(SkillData data) : base(data) { }
    private float attackRadius = 2f;
    private float attackRange = 1f;
    
    // ��ų ���(��������, ����Ʈ, ����)
    public override bool Activate(GameObject user)
    {
        // �ùٸ� ���⸦ �����ߴ��� ����
        bool hasWeapon = WeaponManager.Instance.currentWeapon.type == WeaponType.Sword;

        if (anim == null)
        {
            Debug.Log($"{user} �� Animator�� �������� ����.");
            return false;
        }
        else if(!hasWeapon)
        {
            Debug.Log($"������ ����δ� ��ų�� ����� �� �����ϴ�.");
            return false;
        }
        else
        { 
            anim.SetTrigger("Skill");
            anim.SetInteger("SkillId", data.AnimId);

            if(cachedEffect == null)
            {
                // ������ ����Ʈ�� ������ ����
                cachedEffect = UnityEngine.Object.Instantiate(effectPrefab,
                user.transform.position + new Vector3(0,1,0),
                user.transform.rotation, SkillManager.Instance.gameObject.transform);
            }
            else
            {
                // ������ ����Ʈ�� ������ ���ο� ��ġ ����
                cachedEffect.transform.position = user.transform.position + new Vector3(0, 1, 0);
                cachedEffect.transform.rotation = user.transform.rotation;
            }

            cachedEffect.SetActive(true);
            SkillManager.Instance.StartCoroutine(Attack());
            AudioManager.Instance.PlaySFX(data.Name, 0.3f);     // ������ ���
            return true;
        }
    }

    // ��������(SphereCast ���)
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
                    // ������
                    monster.GetDamaged((DataManager.Instance.GetPlayerData().Damage + data.Damage) * Random.Range(0.8f, 1f));
                }
            }
            else if(hit.collider.CompareTag("BossMonster"))
            {
                BossMonster boss = hit.collider.GetComponent<BossMonster>();
                if (boss != null)
                {
                    // ������
                    boss.GetDamaged((DataManager.Instance.GetPlayerData().Damage + data.Damage) * Random.Range(0.8f, 1f));
                }
            }
        }
    }
    
    // �ִϸ��̼ǿ� ���� �������� ON
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.5f);

        EnableHitBox();
    }
}
