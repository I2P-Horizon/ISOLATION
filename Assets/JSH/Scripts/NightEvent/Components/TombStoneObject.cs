using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TombStoneObject : GatherableObject
{
    public GameObject monster;
    public override void Interact(object context = null)
    {
        base.Interact(context);
    }

    protected override void DestroyObject()
    {
        Destroy(gameObject);
        for(int i = 0; i < 5; i++)
        {
            Instantiate(monster).transform.position = gameObject.transform.position;
        }
    }
}
