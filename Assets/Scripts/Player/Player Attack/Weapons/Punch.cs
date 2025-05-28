using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/*
                        Punch : ����(�ָ�) Ŭ����

                - RayCast�� ����ؼ� �������� ����
*/
public class Punch : Weapon
{
    private float attackRange = 2f;                         // ���� ��Ÿ�
    private int maxComboCount = 1;

    private Vector3 boxSize = new Vector3(0.8f, 2f, 0.8f);
    private Vector3 attackOrigin;                             
    private Vector3 attackDir;


    private void Awake()
    {
        // ���� Ÿ�� ����
        type = WeaponType.None;
        soundId = "Punch";
    }

    // ���� ����(���� ĳ��Ʈ)
    public override void Attack()
    {
        SetComboCount();

        attackOrigin = GameManager.Instance.player.transform.position + GameManager.Instance.player.transform.up;
        attackDir = GameManager.Instance.player.transform.forward;

        RaycastHit[] hits = Physics.BoxCastAll(
            attackOrigin,                       // �߽���ġ : �÷��̾�
            boxSize,                            // �ڽ�ũ��
            attackDir,                          // ���ݹ���     
            Quaternion.identity,                // ȸ��X
            attackRange,                        // �����ִ�Ÿ�
            LayerMask.GetMask("Monster", "BossMonster")
            );

        foreach(RaycastHit hit in hits)
        {
            if(hit.collider.CompareTag("Monster"))
            {
                Monster monster = hit.collider.GetComponent<Monster>();
                if(monster != null)
                {
                    monster.GetDamaged(DataManager.Instance.GetPlayerData().Damage * Random.Range(0.8f, 1f));
                    return;
                }
            }
            else if(hit.collider.CompareTag("BossMonster"))
            {
                BossMonster boss = hit.collider.GetComponent<BossMonster>();
                if(boss!=null)
                {
                    boss.GetDamaged(DataManager.Instance.GetPlayerData().Damage * Random.Range(0.8f, 1f));
                    return;
                }
            }
        }
    }


    private  void OnTriggerEnter(Collider other)
    {
        
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

    // ComboCount ����
    private void SetComboCount()
    {
        if (GameManager.Instance.player.CurComboCount < maxComboCount)
            GameManager.Instance.player.CurComboCount++;
        else
            GameManager.Instance.player.CurComboCount = 0;
    }

    // ���ݹ��� �ð�ȭ
    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackOrigin, boxSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(attackOrigin, attackOrigin + attackDir * attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackOrigin + attackDir * attackRange, boxSize);
    }
}
