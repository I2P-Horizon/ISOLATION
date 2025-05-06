using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ �������� �� ��ȣ�ۿ� ������ ������Ʈ�� �����ؾ� �� ���� �������̽�
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// �÷��̾�� ��ȣ�ۿ� ����.
    /// context �Ķ���͸� ���� �پ��� ������ ���� ����.
    /// </summary>
    /// <param name="context">
    /// ��ȣ�ۿ� �� �߰� ������ ��� �Ű�����. 
    /// �ʿ� ���� ��� ���� ����.
    /// ��� �� null���� �� �����Ƿ�, ����ü���� Ÿ�� Ȯ�� �ʿ�
    /// </param>
    void Interact(object context = null);
}
