using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾�� ��ȣ�ۿ��Ͽ� �ı��� �� �ִ� ������Ʈ�� ���� ��� Ŭ����.
/// ex) ����, ��, ����, ���� ��.
/// ü��(hp)�� �����ϸ�, ü���� ��� �Ҹ�Ǹ� �ı��Ǹ鼭 �������� ����� �� ����.
/// </summary>
public abstract class DestructibleObject : MonoBehaviour, IInteractable
{
    /// <summary>
    /// ������Ʈ�� ü�� (0 ���ϰ� �Ǹ� �ı���)
    /// </summary>
    [SerializeField] protected float _hp;
    /// <summary>
    /// �ı� �� ����� ������ ������
    /// </summary>
    [SerializeField] protected GameObject _dropItem;
    /// <summary>
    /// ����� �������� ����
    /// </summary>
    [SerializeField] protected int _dropAmount;

    /// <summary>
    /// �÷��̾�� ��ȣ�ۿ��� �� ȣ��Ǵ� �Լ�.
    /// �⺻������ amount��ŭ ü���� ���ҽ�Ű��, ü���� 0 ���ϰ� �Ǹ� �ı���Ŵ.
    /// �ڽ� Ŭ�������� �������̵��Ͽ� ����� �� ����.
    /// </summary>
    /// <param name="context">�پ�� ü�� ��(float)</param>
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
    /// ������Ʈ�� �ı��� �� ȣ��Ǵ� �Լ�.
    /// ������ ��� ��, �ڱ� �ڽ��� �ı���Ŵ.
    /// �ڽ� Ŭ�������� �������̵��Ͽ� ����� �� ����.
    /// </summary>
    protected virtual void DestroyObject()
    {
        DropItems();
        Destroy(gameObject);

        Debug.Log($"{_dropAmount}���� �������� �����"); // ����׿� �α�. ���� ���� 
    }

    /// <summary>
    /// �������� ����ϴ� ����.
    /// ������ ������ŭ �������� ������Ʈ �ֺ��� ������.
    /// ���� ��ġ�� ������Ʈ ������ ���� ������ 2m �̳��� ������ ��ġ�� ����.
    /// �ڽ� Ŭ�������� �������̵��Ͽ� ����� �� ����.
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
