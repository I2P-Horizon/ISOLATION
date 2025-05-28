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
            Debug.Log("�÷��̾� �ǰ�");

            DataManager.Instance.GetPlayerData().GetDamaged(damage);
        }

        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if(rigid != null)
        {
            Debug.Log("�÷��̾� ��Ʈ");
            Vector3 knockbackDir = (other.transform.forward).normalized;
            rigid.AddForce(knockbackDir * knocebackForce, ForceMode.Impulse);
        }
    }
}
