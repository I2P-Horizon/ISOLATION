using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour, ICycleListener
{
    private Transform playerTransform;

    private MeshRenderer meshRenderer;

    // Life cycle functions //
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        StartCoroutine(Blink());
    }

    void Update()
    {
        if (playerTransform == null) return;
        transform.LookAt(playerTransform.position);
        transform.Rotate(-90.0f, 0.0f, 0.0f);
    }



    // Interface function implementation //
    public void OnCycleChanged()    
    {
        if(!TimeManager.Instance.IsNight) Destroy(gameObject);
    }



    // Coroutine //
    IEnumerator Blink()
    {
        while(true)
        {
            meshRenderer.enabled = false;
            yield return new WaitForSeconds(1.5f);

            meshRenderer.enabled = true;
            yield return new WaitForSeconds(1.5f);
        }
    }
}