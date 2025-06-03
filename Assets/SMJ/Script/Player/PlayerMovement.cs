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
    private Player _player;

    private float _moveSpeed; // �̵� �ӵ�
    private float _jumpHeight; // ���� ����
    [SerializeField] private float _gravity = -9.81f; // �߷°��ӵ�(����)

    [SerializeField] private float _satietyDecreaseAmount = 0.001f; // �̵��� ���� ������ ���ҷ�

    private CharacterController _characterController; 
    private Vector3 _velocity; // ���� �ӵ�
    private bool _isGrounded; // ���� ��� �ִ��� ����

    public bool IsMoving = false; 

    void Start()
    {
        _player = GetComponent<Player>();
        _characterController = GetComponent<CharacterController>();

        _moveSpeed = _player.State.MoveSpeed;
        _jumpHeight = _player.State.JumpHeight;
    }

    void Update()
    {
        if(_player.Interaction.IsInteracting)
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

        IsMoving = inputDir.sqrMagnitude > 0.01f;

        if (IsMoving)
        {
            inputDir = inputDir.normalized;

            _moveSpeed = _player.State.IsSatietyZero ? _player.State.MoveSpeed * 0.5f : _player.State.MoveSpeed;

            // �̵� ó��
            _characterController.Move(inputDir * _moveSpeed *  Time.deltaTime); 
            
            // �̵� ������ �ٶ󺸵��� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7.0f);

            // �̵��� ���� ������ ����
            if (!_player.State.IsSatietyZero)
                _player.State.DecreaseSatiety(_satietyDecreaseAmount);
        }
        

        // �����̽� �Է� && �÷��̾ ���� ���� ������ ��
        if(_isGrounded && Input.GetButtonDown("Jump"))
        {
            _jumpHeight = _player.State.IsSatietyZero ? _player.State.JumpHeight * 0.5f : _player.State.JumpHeight;

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
