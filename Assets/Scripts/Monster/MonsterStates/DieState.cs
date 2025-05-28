using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                Monster State - Die (죽음상태)

        - Die 애니메이션 실행

        - 히트박스 제거 => 플레이어가 몬스터 통과가능

        - 일정시간뒤 몬스터 비활성화
 */

public class DieState<T> : BaseState<T> where T : Monster
{
    public DieState(T monster) : base(monster) { }

    public override void OnStateEnter()
    {
        // 몬스터 Die 애니메이션
        monster.Anim.SetTrigger("Die");
        monster.Anim.SetBool("Walk", false);

        // 몬스터 히트박스 제거
        monster.HitBox.enabled = false;

        // 몬스터 비활성화
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
