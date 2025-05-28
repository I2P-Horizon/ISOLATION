using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                Monster State - Chase (추격상태)

        - 추격 애니메이션 실행

        - Chase 상태진입시, 플레이어를 쫓음
            - 플레이어와 일정거리(maxDistance)이상 멀어질경우 원래자리(startPosition)로 되돌아감
            - 되돌아가는 위치가 원점과 근접할경우 isReset = true로 복귀했음을 알림
 */
public class ChaseState<T> : BaseState<T> where T : Monster
{
    public ChaseState(T monster) : base(monster) { }

    public override void OnStateEnter()
    {
        monster.isReset = false;
        monster.Anim.SetBool("Walk", true);
    }

    public override void OnStateUpdate()
    {
        // Target 추격
        monster.Nav.SetDestination(monster.Target.transform.position);

        float distanceToPlayer = Vector3.Distance(monster.transform.position, monster.Target.transform.position);

        // 일정거리이상 벌어지면 원래자리로 복귀
        if (distanceToPlayer > monster.maxDistance)
        {
            monster.Nav.SetDestination(monster.StartPosition);

            if(Vector3.Distance(monster.transform.position, monster.StartPosition) <= monster.idleThreshold)
            {
                monster.isReset = true;
            }
        }

    }

    public override void OnStateExit()
    {
        monster.Anim.SetBool("Walk", false);
    }
}
