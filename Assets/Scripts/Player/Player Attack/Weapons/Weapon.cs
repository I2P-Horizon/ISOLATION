using UnityEngine;

/*                  
                    Weapon : ���� ���̽� Ŭ����
            - Attack() : ���� ����� ����
 */
public abstract class Weapon : MonoBehaviour
{
    public WeaponType type;
    protected string soundId;

    // ���� ���� (����ĳ��Ʈ �� ���)
    public abstract void Attack();
  
    // ���� ���� ON/OFF (Collider ���)
    public abstract void SetHitBox(bool isEnabled);

    // ����Ʈ On/Off
    public abstract void SetEffect(bool isEnabled);


    // ���� ȿ���� ����
    public abstract void PlayerSfx();

}
