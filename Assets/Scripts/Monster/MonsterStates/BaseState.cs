using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                BaseState : 몬스터가 가지는 상태 구현을 위한 추상클래스

        * 개별 몬스터는 Monster 클래스를 상속받아 구현
            => 제네릭으로 구현하여 여러 몬스터들을 받을 수 있게 구현

        - OnStateEnter() : 상태에 처음 진입했을 때 한 번만 호출(초기설정)
        - OnStateUpdate() : 매 프레임마다 호출
        - OnStateExit() : 상태 변경시 호출(마무리작업)
 */

public abstract class BaseState<T> where T : Monster
{
    protected T monster;
   
    protected BaseState(T monster)
    {
        this.monster = monster;
    }
    
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
