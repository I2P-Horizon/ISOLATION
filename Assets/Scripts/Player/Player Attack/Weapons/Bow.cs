using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                    Bow : ����(Ȱ) Ŭ����

            - Projectile�� ����ؼ� �������� ����
            
 */
public class Bow : Weapon
{
    private void Awake()
    {
        type = WeaponType.Bow;
    }

    public override void Attack()
    {
        // ȭ�� ����
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
