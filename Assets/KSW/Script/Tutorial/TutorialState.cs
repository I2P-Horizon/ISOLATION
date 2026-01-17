using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialState
{
    protected TutorialManager manager;

    protected bool isCompleted = false;

    public TutorialState(TutorialManager manager)
    {
        this.manager = manager;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}