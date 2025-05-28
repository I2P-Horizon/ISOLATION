using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                    Sword : 무기(검) 클래스

            - Collider를 사용해서 공격판정 구현
            - SetHitBox() : Collider On/Off - 애니메이션 이벤트에 사용
 */
public class Sword : Weapon
{
    private BoxCollider hitBox;                 // 공격 판정
    private TrailRenderer effect;               // 공격 이펙트

    private void Awake()
    {
        // 무기 타입 설정
        type = WeaponType.Sword;

        hitBox = GetComponent<BoxCollider>();
        effect = GetComponentInChildren<TrailRenderer>();
        soundId = "Sword";
    }

    public override void Attack()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();
            monster.GetDamaged(DataManager.Instance.GetPlayerData().Damage * Random.Range(0.8f, 1f));
        }
        else if(other.CompareTag("BossMonster"))
        {
            BossMonster boss = other.GetComponent<BossMonster>();
            boss.GetDamaged(DataManager.Instance.GetPlayerData().Damage * Random.Range(0.8f, 1f));
        }
    }

    public override void SetHitBox(bool isEnabled)
    {
        hitBox.enabled = isEnabled;
    }

    public override void SetEffect(bool isEnabled)
    {
        effect.enabled = isEnabled;
    }

    public override void PlayerSfx()
    {
        AudioManager.Instance.PlaySFX(soundId);
    }
}
