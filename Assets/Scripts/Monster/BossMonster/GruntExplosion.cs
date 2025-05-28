using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntExplosion : MonoBehaviour
{
    public float damage;
    public float knocebackForce = 7f;
    public float duration = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("플레이어 피격");

            DataManager.Instance.GetPlayerData().GetDamaged(damage);
        }

        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if(rigid != null)
        {
            Debug.Log("플레이어 히트");
            Vector3 knockbackDir = (other.transform.forward).normalized;
            rigid.AddForce(knockbackDir * knocebackForce, ForceMode.Impulse);
        }
    }
}
