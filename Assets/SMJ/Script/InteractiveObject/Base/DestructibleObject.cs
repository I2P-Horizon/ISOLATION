using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와 상호작용하여 파괴될 수 있는 오브젝트의 공통 기반 클래스.
/// ex) 나무, 돌, 몬스터, 동물 등.
/// 체력(hp)가 존재하며, 체력이 모두 소모되면 파괴되면서 아이템을 드랍할 수 있음.
/// </summary>
public abstract class DestructibleObject : MonoBehaviour, IInteractable
{
    /// <summary>
    /// 오브젝트의 체력 (0 이하가 되면 파괴됨)
    /// </summary>
    [SerializeField] protected float _hp;
    /// <summary>
    /// 파괴 시 드랍될 아이템 프리팹
    /// </summary>
    [SerializeField] protected GameObject _dropItem;
    /// <summary>
    /// 드랍할 아이템의 수량
    /// </summary>
    [SerializeField] protected int _dropAmount;

    /// <summary>
    /// 플레이어와 상호작용할 때 호출되는 함수.
    /// 기본적으로 amount만큼 체력을 감소시키고, 체력이 0 이하가 되면 파괴시킴.
    /// 자식 클래스에서 오버라이드하여 사용할 수 있음.
    /// </summary>
    /// <param name="context">줄어들 체력 값(float)</param>
    public virtual void Interact(object context = null)
    {
        if (context is float amount)
        {
            _hp -= amount;

            if (_hp <= 0)
            {
                DestroyObject();
                return;
            }
        }
    }

    /// <summary>
    /// 오브젝트가 파괴될 때 호출되는 함수.
    /// 아이템 드랍 후, 자기 자신을 파괴시킴.
    /// 자식 클래스에서 오버라이드하여 사용할 수 있음.
    /// </summary>
    protected virtual void DestroyObject()
    {
        DropItems();
        Destroy(gameObject);

        Debug.Log($"{_dropAmount}개의 아이템이 드랍됨"); // 디버그용 로그. 추후 삭제 
    }

    /// <summary>
    /// 아이템을 드랍하는 로직.
    /// 지정된 수량만큼 아이템을 오브젝트 주변에 생성함.
    /// 생성 위치는 오브젝트 포지션 기준 반지름 2m 이내의 랜덤한 위치로 설정.
    /// 자식 클래스에서 오버라이드하여 사용할 수 있음.
    /// </summary>
    protected virtual void DropItems()
    {
        if (_dropItem == null) return;

        for (int i = 0; i < _dropAmount; i++)
        {
            Vector3 dropPosition = transform.position + Random.insideUnitSphere * 2f;
            dropPosition.y = transform.position.y;

            Quaternion dropRotation = _dropItem.transform.rotation;

            Instantiate(_dropItem, dropPosition, dropRotation);
        }
    }
}
