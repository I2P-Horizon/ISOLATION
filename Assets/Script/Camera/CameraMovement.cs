using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ī�޶� �̵� ����
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; // ���� ��� (�÷��̾�)
    /// <summary>
    /// ��� ��ġ. �÷��̾�� ���� ���� ��ġ�ϱ� ���� �Ÿ�
    /// </summary>
    [SerializeField] private Vector3 offset = new Vector3(0f, 3.0f, -10.0f);
    /// <summary>
    /// �ε巯�� ���� (���ڰ� �������� ������ ������)
    /// </summary>
    [SerializeField] private float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero; // ���� �ӵ�

    private void Start()
    {
        // ���� �� ī�޶� ��ġ ����
        transform.position = targetTransform.position + offset;
    }

    private void LateUpdate()
    {
        // ��ǥ ��ġ ���
        Vector3 targetPosition = targetTransform.position + offset;

        // ���� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵�
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
