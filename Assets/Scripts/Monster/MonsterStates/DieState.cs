using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                Monster State - Die (��������)

        - Die �ִϸ��̼� ����

        - ��Ʈ�ڽ� ���� => �÷��̾ ���� �������

        - �����ð��� ���� ��Ȱ��ȭ
 */

public class DieState<T> : BaseState<T> where T : Monster
{
    public DieState(T monster) : base(monster) { }

    public override void OnStateEnter()
    {
        // ���� Die �ִϸ��̼�
        monster.Anim.SetTrigger("Die");
        monster.Anim.SetBool("Walk", false);

        // ���� ��Ʈ�ڽ� ����
        monster.HitBox.enabled = false;

        // ���� ��Ȱ��ȭ
        monster.Invoke(nameof(monster.DeactiveGameObject), 3);
    }

    public override void OnStateUpdate()
    {
        monster.Nav.SetDestination(monster.transform.position);
    }

    public override void OnStateExit()
    {

    }
}
