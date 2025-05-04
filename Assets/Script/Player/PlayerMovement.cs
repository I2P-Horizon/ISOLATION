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
    [SerializeField] private float moveSpeed = 5.0f; // �̵� �ӵ�
    [SerializeField] private float jumpHeight = 2.0f; // ���� ����
    [SerializeField] private float gravity = -9.81f; // �߷°��ӵ�(����)

    private CharacterController characterController; // ĳ���� ��Ʈ�ѷ�
    private Vector3 velocity; // ���� �ӵ�
    private bool isGrounded; // ���� ��� �ִ��� ����

    void Start()
    {
        characterController = GetComponent<CharacterController>();
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
        isGrounded = characterController.isGrounded;

        // ���� ��� �ְ�, y�� �ӵ��� �Ʒ��� ���ϰ� ������
        if (isGrounded && velocity.y < 0)
        {
            // y�� �ӵ��� �����Ͽ� �÷��̾ �ٴڿ� ���������� �پ��ְ� ��
            velocity.y = -2.0f;
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
        characterController.Move(move * moveSpeed * Time.deltaTime); // �̵� ó��

        // �����̽� �Է� && �÷��̾ ���� ���� ������ ��
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            // ���� ���̿� �߷����� �ʱ� y�ӵ� ���
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        velocity.y += gravity * Time.deltaTime; // �߷��� �������� �ð��� �������� �� ������ ���������� ��
        characterController.Move(velocity * Time.deltaTime); // ���� �̵� ����
    }
}
