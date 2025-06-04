using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineappleBushObject : BushObject
{
    [SerializeField] PickupItem _pineapple;

    private bool _pineappleDropped = false;

    public override void Interact(object context = null)
    {
        if (!_pineappleDropped)
        {
            DropPineapple();
            _pineappleDropped = true;
        }

        base.Interact(context);
    }

    private void DropPineapple()
    {
        _pineapple.transform.SetParent(null);

        if (_pineapple.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            Vector3 launchDir = (Vector3.up + Vector3.right).normalized;
            rb.AddForce(launchDir * 2f, ForceMode.Impulse);
        }
    }
}
