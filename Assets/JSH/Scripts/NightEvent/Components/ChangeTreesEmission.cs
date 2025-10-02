using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTreesEmission : MonoBehaviour, IEvent
{
    // Shared tree material
    [SerializeField] Material treeMaterial;

    public void ExecuteDayEvent()
    {
        treeMaterial.DisableKeyword("_EMISSION");
    }

    public void ExecuteNightEvent()
    {
        treeMaterial.EnableKeyword("_EMISSION");
    }
}