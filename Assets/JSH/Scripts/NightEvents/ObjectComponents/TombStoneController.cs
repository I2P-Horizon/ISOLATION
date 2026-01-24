using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class TombStoneController : GatherableObject
{
    [SerializeField] private GameObject golemPrefab;

    private Material material;
    private float startEmissionIntensity = 0.01f;
    private float endEmissionIntensity = 5.0f;
    private float endTime = 150.0f;

    void Start()
    {
        material = GetComponentInChildren<Renderer>().material;
        StartCoroutine(CountDownStart());
    }
    public override void Interact(object context = null)
    {
        base.Interact(context);
    }

    protected override void DestroyObject()
    {
        Destroy(gameObject);
        Instantiate(golemPrefab).transform.position = gameObject.transform.position;
    }
    IEnumerator CountDownStart()
    {
        float timer = 0.0f;
        Color initEmissionColor = material.GetColor("_EmissionColor");

        while (timer <= endTime)
        {

            material.SetColor("_EmissionColor", (initEmissionColor * Mathf.Lerp(startEmissionIntensity, endEmissionIntensity, timer / endTime)));

            timer += Time.deltaTime;
            yield return null;
        }

        DestroyObject();
    }
}