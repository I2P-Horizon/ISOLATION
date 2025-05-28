using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleShellRange : MonoBehaviour
{
    [SerializeField] private SphereCollider scanRange;              // 적 탐지 범위
    [SerializeField] private  TurtleShell parent;

    // Scan Range에 플레이어가 들어왔을경우 Chase 상태 돌입
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if(parent.curState == TurtleShell.States.Idle)
            parent.ChangeState(TurtleShell.States.Chase);
    }
}
