using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorilla : CreatureBase
{
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        base.OnEnable();
        InitStats(100.0f, 200.0f, 5.0f, 2.0f, 10.0f, 5.0f, 5.0f, 2.0f);
    }
}
