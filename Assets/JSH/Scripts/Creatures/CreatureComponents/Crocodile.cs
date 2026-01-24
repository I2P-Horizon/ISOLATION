using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : CreatureBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        InitStats(80.0f, 160.0f, 5.0f, 2.0f, 10.0f, 5.0f, 5.0f, 2.0f);
    }
}
