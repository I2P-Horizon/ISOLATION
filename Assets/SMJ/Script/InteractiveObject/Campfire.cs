using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    [Header("Cooking Settings")]
    [SerializeField] private int _rawMeatID = 40001;
    [SerializeField] private GameObject _cookedMeatPrefab;
    [SerializeField] private float _cookingTime = 20.0f;
    [SerializeField] private GameObject _rawMeatVisual;

    private bool _isCooking = false;

    private void Start()
    {
        if (_rawMeatVisual != null)
            _rawMeatVisual.SetActive(false);
    }

    public bool TryCook(Inventory inventory)
    {
        if (_isCooking)
        {
            Debug.Log("Campfire is already cooking.");
            return false;
        }

        if (inventory.GetTotalAmountOfItem(_rawMeatID) >= 1)
        {
            inventory.ConsumeItem(_rawMeatID, 1);
            StartCoroutine(CookMeat());
            return true;
        }
        else
        {
            Debug.Log("Not enough raw meat to cook.");
            return false;
        }
    }

    private IEnumerator CookMeat()
    {
        _isCooking = true;

        if (_rawMeatVisual != null) _rawMeatVisual.SetActive(true);

        yield return new WaitForSeconds(_cookingTime);

        if (_rawMeatVisual != null) _rawMeatVisual.SetActive(false);

        dropCookedMeat();
        _isCooking = false;
    }

    private void dropCookedMeat()
    {
        if (_cookedMeatPrefab != null)
        {
            Vector3 dropPos = transform.position - transform.right * 2.0f + Vector3.up * 0.5f;

            Instantiate(_cookedMeatPrefab, dropPos, Quaternion.identity);
        }
    }
}
