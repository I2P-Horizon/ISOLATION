using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾�� ��ȣ�ۿ� ������ ������Ʈ�� ���� ��� Ŭ����.
/// ex) ����, ��, ����, ���� ��.
/// ü��(hp)�� �����ϸ�, ü���� ��� �Ҹ�Ǹ� �ı��Ǹ鼭 �������� ����� �� ����.
/// </summary>
public abstract class InteractiveObject : MonoBehaviour
{
    /// <summary>
    /// ������Ʈ�� ü�� (0 ���ϰ� �Ǹ� �ı���)
    /// </summary>
    [SerializeField] protected float hp;
    /// <summary>
    /// �ı� �� ����� ������ ������
    /// </summary>
    [SerializeField] protected GameObject dropItem;
    /// <summary>
    /// ����� �������� ����
    /// </summary>
    [SerializeField] protected int dropAmount;

    /// <summary>
    /// �÷��̾�� ��ȣ�ۿ��� �� ȣ��Ǵ� �Լ�.
    /// �⺻������ amount��ŭ ü���� ���ҽ�Ű��, ü���� 0 ���ϰ� �Ǹ� �ı���Ŵ.
    /// �ڽ� Ŭ�������� �������̵��Ͽ� ����� �� ����.
    /// </summary>
    /// <param name="amount">�پ�� ü�� ��</param>
    public virtual void Interact(float amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            DestroyObject();
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

        Debug.Log($"{dropAmount}���� �������� �����"); // ����׿� �α�. ���� ���� 
    }

    /// <summary>
    /// �������� ����ϴ� ����.
    /// ������ ������ŭ �������� ������Ʈ �ֺ��� ������.
    /// ���� ��ġ�� ������Ʈ ������ ���� ������ 2m �̳��� ������ ��ġ�� ����.
    /// �ڽ� Ŭ�������� �������̵��Ͽ� ����� �� ����.
    /// </summary>
    protected virtual void DropItems()
    {
        if (dropItem == null) return;

        for (int i = 0; i < dropAmount; i++)
        {
            Vector3 dropPosition = transform.position + Random.insideUnitSphere * 2f;
            dropPosition.y = transform.position.y;

            Instantiate(dropItem, dropPosition, Quaternion.identity);
        }
    }
}
