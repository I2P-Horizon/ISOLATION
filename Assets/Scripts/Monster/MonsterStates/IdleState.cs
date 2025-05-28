using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                Monster State - Idle (기본상태)
        
           - 기본상태 애니메이션 실행
 */
public class IdleState<T> : BaseState<T> where T : Monster
{
    public IdleState(T monster) : base(monster) { }

    public override void OnStateEnter()
    {
        monster.Anim.SetBool("Walk", false);
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }
}
