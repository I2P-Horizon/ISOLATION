using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoconutTreeObject : TreeObject
{
    [SerializeField] private PickupItem _coconut;

    private bool _coconutDropped = false;

    public override void Interact(object context = null)
    {
        if (!_coconutDropped)
        {
            DropCoconut();
            _coconutDropped = true;
        }

        base.Interact(context);
    }

    private void DropCoconut()
    {
        _coconut.transform.SetParent(null);

        if (_coconut.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            _coconut.SetPickupState(true);
        }
    }
}
