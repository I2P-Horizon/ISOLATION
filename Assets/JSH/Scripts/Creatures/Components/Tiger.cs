using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : CreatureBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        InitStats(90.0f, 180.0f, 5.0f, 2.0f, 10.0f, 5.0f, 5.0f, 2.0f);
    }
}
