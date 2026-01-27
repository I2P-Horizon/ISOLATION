using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapUI : Overlay
{
    [SerializeField] private GameObject _worldMap;

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!_worldMap.activeSelf) Show();
            else Close();
        }
    }

    protected override void Awake()
    {
        _animator = _worldMap.GetComponent<UIAnimator>();
    }
}