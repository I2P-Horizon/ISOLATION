using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : GatherableObject
{
    public override void Interact(object context = null)
    {
        base.Interact(context);

        // 효과음, 애니메이션 등 추후 구현 예정
    }
}
