using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                Monster State - Attack (���ݻ���)

        - Attack ���¿��� ����� �������� ���� �ۼ�

        - ������ ��ġ������ �÷��̾ �ٶ󺸰� ���ݽ���
 */

public class AttackState<T> : BaseState<T> where T : Monster
{
    public AttackState(T monster) : base(monster) { }

    private Vector3 startPos;

    public override void OnStateEnter()
    {
        monster.Anim.SetBool("Walk", false);
        startPos = monster.transform.position;
    }

    public override void OnStateUpdate()
    {
        // ��ġ ����
        monster.Nav.SetDestination(startPos);

        // �÷��̾� �ٶ󺸱�
        monster.transform.LookAt(monster.Target.transform);

        // �����غ� �Ǿ��� ��
        if(monster.isAttackReady)
        {
            monster.Anim.SetTrigger("Attack");

            // ���ݼӵ��� ���� ���ݰ��ɿ��� ����
            monster.isAttackReady = false;
            monster.Invoke(nameof(monster.ReadyToAttack), monster.attackDelay);
        }
    }

    public override void OnStateExit()
    {

    }
}
