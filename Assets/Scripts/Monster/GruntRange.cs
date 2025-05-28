using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntRange : MonoBehaviour
{
    [SerializeField] private BoxCollider scanRange;         // 적 탐지 범위
    [SerializeField] private Grunt parent;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        parent.targetPlayer = GameManager.Instance.player;
    }
}
