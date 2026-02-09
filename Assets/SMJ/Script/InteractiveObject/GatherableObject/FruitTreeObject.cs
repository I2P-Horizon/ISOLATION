using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitTreeObject : TreeObject
{
    [Header("Fruit Settings")]
    [SerializeField] private PickupItem _initialFruit;
    [SerializeField] private PickupItem _fruitPrefab;

    [Header("Drop Settings")]
    [SerializeField] private float _minDropForce = 2.0f;
    [SerializeField] private float _maxDropForce = 5.0f;
    [SerializeField] private float _upwardModifier = 1.0f;

    private bool _isFruitReady = true;
    private PickupItem _currentFruit;
    private Transform _fruitMountPoint;
    private Vector3 _saveFruitScale;

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

            _saveFruitScale = _initialFruit.transform.localScale;

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

            Vector3 randomDir = Random.insideUnitCircle.normalized;
            Vector3 dropDirection = new Vector3(randomDir.x, _upwardModifier, randomDir.y).normalized;

            float force = Random.Range(_minDropForce, _maxDropForce);

            rb.AddForce(dropDirection * force, ForceMode.Impulse);

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

            newFruit.transform.localScale = _saveFruitScale;

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
