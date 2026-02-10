using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTreesEmissionEvent : MonoBehaviour, IEvent
{
    // Shared tree material
    [SerializeField] Material tree1Material;
    [SerializeField] Material tree2Material;
    [SerializeField] Material tree3Material;

    public void ExecuteDayEvent()
    {
        tree1Material.DisableKeyword("_EMISSION");
        tree2Material.DisableKeyword("_EMISSION");
        tree3Material.DisableKeyword("_EMISSION");
    }

    public void ExecuteNightEvent()
    {
        tree1Material.EnableKeyword("_EMISSION");
        tree2Material.EnableKeyword("_EMISSION");
        tree3Material.EnableKeyword("_EMISSION");
    }
}