using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemStoneObject : RockObject
{
    [SerializeField] private GameObject[] dropGems;
    [SerializeField] private float dropRate = 0.5f;

    public override void Interact(object context = null)
    {
        base.Interact(context);

        // 효과음, 애니메이션 등 추후 구현 예정
    }

    protected override void DropItems()
    {
        if (dropGems.Length == 0) return;

        if (Random.value > dropRate) return;

        int index = Random.Range(0, dropGems.Length);

        for (int i = 0; i < _dropAmount; i++)
        {
            Vector3 dropPosition = transform.position + Random.insideUnitSphere * 2f;
            dropPosition.y = transform.position.y + 1;

            Quaternion dropRotation = dropGems[index].transform.rotation;

            Instantiate(dropGems[index], dropPosition, dropRotation);
        }
    }
}
