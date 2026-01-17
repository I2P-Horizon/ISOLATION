using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armadillo : CreatureBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        InitStats(50.0f, 100.0f, 5.0f, 2.0f, 10.0f, 5.0f, 5.0f, 2.0f);
    }
}
