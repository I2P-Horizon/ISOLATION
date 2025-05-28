using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                ���Ӱ����� ��ų

            - ���� ���ݸ��� ������ �߻�
 */
public class StayHitbox : MonoBehaviour
{
    public float damage;
    public float attackInterval = 0.5f;             // ���� ����(0.5��)

    private float timer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();

            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                monster.GetDamaged(damage);
                timer = 0;
            }
        }
        else if(other.CompareTag("BossMonster"))
        {
            BossMonster boss = other.GetComponent<BossMonster>();

            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                boss.GetDamaged(damage);
                timer = 0;
            }
        }
    }
}
