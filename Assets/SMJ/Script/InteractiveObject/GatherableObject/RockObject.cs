using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockObject : GatherableObject
{
    [SerializeField] private int dropMinAmount = 1;

    public override void Interact(object context = null)
    {
        base.Interact(context);

        // 효과음, 애니메이션 등 추후 구현 예정
    }

    protected override void DropItems()
    {
        if (_dropItem == null) return;

        int amount = Random.Range(dropMinAmount, _dropAmount + 1);

        for (int i = 0; i < amount; i++)
        {
            Vector3 dropPosition = transform.position + Random.insideUnitSphere * 2f;
            dropPosition.y = transform.position.y + 1;

            Quaternion dropRotation = _dropItem.transform.rotation;

            Instantiate(_dropItem, dropPosition, dropRotation);
        }
    }
}
