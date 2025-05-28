using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
            StateMachine�� Monster�� AI ���¸� ����
 */

public class StateMachine<T> where T : Monster
{
    public BaseState<T> curState { get; set; }

    public StateMachine(BaseState<T> init)
    {
        // ������� �ʱ�ȭ
        curState = init;
        curState.OnStateEnter();
    }

    // ���� ����
    public void ChangeState(BaseState<T> nextState)
    {
        // �ٸ����·θ� ����
        if (nextState == curState)
            return;

        // ������� Ż��
        if (curState != null)
            curState.OnStateExit();

        // ������¸� nextState�� ����
        curState = nextState;
        curState.OnStateEnter();
    }

    public void UpdateState()
    {
        if (curState != null)
            curState.OnStateUpdate();
    }
}
