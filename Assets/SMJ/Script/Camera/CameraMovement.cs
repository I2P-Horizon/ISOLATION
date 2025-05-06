using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ī�޶� �̵� ����
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform; // ���� ��� (�÷��̾�)
    /// <summary>
    /// ��� ��ġ. �÷��̾�� ���� ���� ��ġ�ϱ� ���� �Ÿ�
    /// </summary>
    [SerializeField] private Vector3 _offset = new Vector3(0f, 3.0f, -10.0f);
    /// <summary>
    /// �ε巯�� ���� (���ڰ� �������� ������ ������)
    /// </summary>
    [SerializeField] private float _smoothTime = 0.1f;

    private Vector3 _velocity = Vector3.zero; // ���� �ӵ�

    private void Start()
    {
        // ���� �� ī�޶� ��ġ ����
        transform.position = _targetTransform.position + _offset;
    }

    private void LateUpdate()
    {
        // ��ǥ ��ġ ���
        Vector3 targetPosition = _targetTransform.position + _offset;

        // ���� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵�
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
    }
}
