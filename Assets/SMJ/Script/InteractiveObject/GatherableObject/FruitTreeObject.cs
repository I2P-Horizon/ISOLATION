using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitTreeObject : TreeObject
{
    [SerializeField] private PickupItem _fruit;

    [Header("Drop Settings")]
    [SerializeField] private float _minDropForce = 2.0f;
    [SerializeField] private float _maxDropForce = 5.0f;
    [SerializeField] private float _upwardModifier = 1.0f;

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

            Vector3 randomDir = Random.insideUnitCircle.normalized;
            Vector3 dropDirection = new Vector3(randomDir.x, _upwardModifier, randomDir.y).normalized;

            float force = Random.Range(_minDropForce, _maxDropForce);

            rb.AddForce(dropDirection * force, ForceMode.Impulse);

            _fruit.SetPickupState(true);
        }
    }
}
