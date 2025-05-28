using UnityEngine;

/*                  
                    Weapon : 무기 베이스 클래스
            - Attack() : 현재 무기로 공격
 */
public abstract class Weapon : MonoBehaviour
{
    public WeaponType type;
    protected string soundId;

    // 공격 판정 (레이캐스트 등 방식)
    public abstract void Attack();
  
    // 공격 판정 ON/OFF (Collider 방식)
    public abstract void SetHitBox(bool isEnabled);

    // 이펙트 On/Off
    public abstract void SetEffect(bool isEnabled);


    // 공격 효과음 실행
    public abstract void PlayerSfx();

}
