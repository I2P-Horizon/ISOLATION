using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                BaseState : ���Ͱ� ������ ���� ������ ���� �߻�Ŭ����

        * ���� ���ʹ� Monster Ŭ������ ��ӹ޾� ����
            => ���׸����� �����Ͽ� ���� ���͵��� ���� �� �ְ� ����

        - OnStateEnter() : ���¿� ó�� �������� �� �� ���� ȣ��(�ʱ⼳��)
        - OnStateUpdate() : �� �����Ӹ��� ȣ��
        - OnStateExit() : ���� ����� ȣ��(�������۾�)
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
