using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCameraMovement : CameraMovement
{
    protected override void Start()
    {
        _targetTransform = FindFirstObjectByType<Player>().transform;
        base.Start();
    }
}