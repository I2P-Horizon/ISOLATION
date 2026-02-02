using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBushObject : BushObject
{
    [SerializeField] PickupItem _fruit;

    private bool _fruitDropped = false;

    public override void Interact(object context = null)
    {
        if (!_fruitDropped)
        {
            DropFruit();
            _fruitDropped = true;
        }

        base.Interact(context);
    }

    private void DropFruit()
    {
        _fruit.transform.SetParent(null);

        if (_fruit.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            Vector3 launchDir = (Vector3.up + Vector3.right).normalized;
            rb.AddForce(launchDir * 2f, ForceMode.Impulse);
        }
    }
}
