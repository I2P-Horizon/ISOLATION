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
    private float _moveSpeed; // �̵� �ӵ�
    [SerializeField] private float _jumpHeight = 2.0f; // ���� ����
    [SerializeField] private float _gravity = -9.81f; // �߷°��ӵ�(����)

    [SerializeField] private float _satietyDecreaseAmount = 0.001f; // �̵��� ���� ������ ���ҷ�

    private CharacterController _characterController; 
    private PlayerInteraction _interaction; 
    private Vector3 _velocity; // ���� �ӵ�
    private bool _isGrounded; // ���� ��� �ִ��� ����

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _interaction = GetComponent<PlayerInteraction>();

        _moveSpeed = PlayerState.Instance.MoveSpeed;
    }

    void Update()
    {
        if(_interaction.IsInteracting)
        {
            GroundCheck();
            ApplyGravity();
            return;
        }

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

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// �̵�, ���� ó��
    /// </summary>
    private void Move()
    {
        Vector3 inputDir = GetInputMovement(); // �Է¿� ���� ���� ���� ���
        
        if (inputDir.sqrMagnitude > 0.01f)
        {
            inputDir = inputDir.normalized;

            // �̵� ó��
            _characterController.Move(inputDir * _moveSpeed *  Time.deltaTime); 
            
            // �̵� ������ �ٶ󺸵��� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7.0f);

            // �̵��� ���� ������ ����
            PlayerState.Instance.DecreaseSatiety(_satietyDecreaseAmount);
        }
        

        // �����̽� �Է� && �÷��̾ ���� ���� ������ ��
        if(_isGrounded && Input.GetButtonDown("Jump"))
        {
            // ���� ���̿� �߷����� �ʱ� y�ӵ� ���
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
        }

        ApplyGravity();
    }

    /// <summary>
    /// �߷� ��� ���
    /// </summary>
    private void ApplyGravity()
    {
        _velocity.y += _gravity * Time.deltaTime; // �߷��� �������� �ð��� �������� �� ������ ���������� ��
        _characterController.Move(_velocity * Time.deltaTime); // ���� �̵� ����
    }
}
