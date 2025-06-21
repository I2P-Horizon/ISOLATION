using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : MonoBehaviour
{
    [SerializeField] private GameObject lgihtEffect;

    private void Update()
    {
        lgihtEffect.gameObject.transform.Rotate(0.01f, 0, 0);
    }
}