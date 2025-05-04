using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �̵�/���� �Է� �� ó��
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f; // �̵� �ӵ�
    [SerializeField] private float _jumpHeight = 2.0f; // ���� ����
    [SerializeField] private float _gravity = -9.81f; // �߷°��ӵ�(����)

    private CharacterController _characterController; // ĳ���� ��Ʈ�ѷ�
    private Vector3 _velocity; // ���� �ӵ�
    private bool _isGrounded; // ���� ��� �ִ��� ����

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        GroundCheck();
        Move();
    }

    /// <summary>
    /// �÷��̾ ���� ����ִ��� �Ǵ� + ���� ����ְ� �ӵ��� �Ʒ��� ���� �� �ӵ� ����
    /// </summary>
    private void GroundCheck()
    {
        _isGrounded = _characterController.isGrounded;

        // ���� ��� �ְ�, y�� �ӵ��� �Ʒ��� ���ϰ� ������
        if (_isGrounded && _velocity.y < 0)
        {
            // y�� �ӵ��� �����Ͽ� �÷��̾ �ٴڿ� ���������� �پ��ְ� ��
            _velocity.y = -2.0f;
        }
    }

    /// <summary>
    /// �̵� ���� �Է��� �޾� �÷��̾��� �̵� ���� ���
    /// </summary>
    /// <returns>�̵� ���� ����</returns>
    private Vector3 GetInputMovement()
    {
        float x = Input.GetAxis("Horizontal"); // �¿� �̵� (A/D or ��/��)
        float z = Input.GetAxis("Vertical"); // �յ� �̵� (W/S or ��/��)

        return transform.right * x + transform.forward * z;
    }

    /// <summary>
    /// �̵�, ����, �߷� ó�� ���
    /// </summary>
    private void Move()
    {
        Vector3 move = GetInputMovement(); // �Է¿� ���� ���� ���� ���
        _characterController.Move(move * _moveSpeed * Time.deltaTime); // �̵� ó��

        // �����̽� �Է� && �÷��̾ ���� ���� ������ ��
        if(_isGrounded && Input.GetButtonDown("Jump"))
        {
            // ���� ���̿� �߷����� �ʱ� y�ӵ� ���
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
        }

        _velocity.y += _gravity * Time.deltaTime; // �߷��� �������� �ð��� �������� �� ������ ���������� ��
        _characterController.Move(_velocity * Time.deltaTime); // ���� �̵� ����
    }
}
