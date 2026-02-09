using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBushObject : BushObject
{
    [Header("Fruit Settings")]
    [SerializeField] private PickupItem _initialFruit;
    [SerializeField] private PickupItem _fruitPrefab;

    private bool _isFruitReady = true;
    private PickupItem _currentFruit;
    private Transform _fruitMountPoint;
    private Vector3 _savedFruitScale;

    protected override void Awake()
    {
        base.Awake();

        if (_initialFruit != null)
        {
            _currentFruit = _initialFruit;

            GameObject mountPoint = new GameObject("FruitMountPoint");
            mountPoint.transform.SetParent(transform);
            mountPoint.transform.position = _initialFruit.transform.position;
            mountPoint.transform.rotation = _initialFruit.transform.rotation;

            _savedFruitScale = _initialFruit.transform.localScale;

            _fruitMountPoint = mountPoint.transform;
        }
    }

    public override void Interact(object context = null)
    {
        if (_isFruitReady && _currentFruit != null)
        {
            DropFruit();
        }

        base.Interact(context);
    }

    private void DropFruit()
    {
        _isFruitReady = false;

        _currentFruit.transform.SetParent(null);

        if (_currentFruit.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            Vector3 launchDir = (Vector3.up + Vector3.right).normalized;
            rb.AddForce(launchDir * 2f, ForceMode.Impulse);

            _currentFruit.SetPickupState(true);
        }

        _currentFruit = null;
    }

    protected override void OnRespawn()
    {
        base.OnRespawn();

        if (_fruitPrefab != null && _fruitMountPoint != null)
        {
            PickupItem newFruit = Instantiate(_fruitPrefab, _fruitMountPoint.position, _fruitMountPoint.rotation, transform);

            newFruit.transform.localScale = _savedFruitScale;

            if (newFruit.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            newFruit.SetPickupState(false);

            _currentFruit = newFruit;
            _isFruitReady = true;
        }
    }
}
