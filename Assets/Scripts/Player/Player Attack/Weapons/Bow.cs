using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                    Bow : 무기(활) 클래스

            - Projectile를 사용해서 공격판정 구현
            
 */
public class Bow : Weapon
{
    private void Awake()
    {
        type = WeaponType.Bow;
    }

    public override void Attack()
    {
        // 화살 생성
    }

    public override void SetHitBox(bool isEnabled)
    {
        
    }

    public override void SetEffect(bool isEnabled)
    {
        
    }

    public override void PlayerSfx()
    {
        
    }
}
