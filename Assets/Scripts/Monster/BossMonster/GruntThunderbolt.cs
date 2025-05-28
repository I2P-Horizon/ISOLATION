using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                GruntThunderbolt

            - 보스몬스터 Grunt 스킬 공격
                - 장판형 지속스킬
 */

public class GruntThunderbolt : MonoBehaviour
{
    public float damage;
    public float attackInterval = 0.5f;

    private float timer = 0f;

    private void OnTriggerStay(Collider ohter)
    {
        if (ohter.CompareTag("Player"))
        {
            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                DataManager.Instance.GetPlayerData().GetDamaged(damage * Random.Range(0.1f, 0.4f));
                timer = 0;
            } 
        }
    }
}
