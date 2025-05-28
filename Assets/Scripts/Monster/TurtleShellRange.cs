using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleShellRange : MonoBehaviour
{
    [SerializeField] private SphereCollider scanRange;              // �� Ž�� ����
    [SerializeField] private  TurtleShell parent;

    // Scan Range�� �÷��̾ ��������� Chase ���� ����
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if(parent.curState == TurtleShell.States.Idle)
            parent.ChangeState(TurtleShell.States.Chase);
    }
}
