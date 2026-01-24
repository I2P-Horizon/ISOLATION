using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PredictEyesRadiusController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            foreach (Renderer renderer in other.gameObject.GetComponents<Renderer>())
                renderer.enabled = true;
            foreach (Renderer renderer in other.gameObject.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            foreach (Renderer renderer in other.gameObject.GetComponents<Renderer>())
                renderer.enabled = false;
            foreach (Renderer renderer in other.gameObject.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
        }
    }
}
