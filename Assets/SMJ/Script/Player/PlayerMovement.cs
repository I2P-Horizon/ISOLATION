using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 이동/점프 입력 및 처리
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Player _player;

    private float _moveSpeed; // 이동 속도
    private float _jumpHeight; // 점프 높이
    [SerializeField] private float _gravity = -9.81f; // 중력가속도(음수)

    [SerializeField] private float _satietyDecreaseAmount = 0.001f; // 이동에 따른 포만감 감소량

    private CharacterController _characterController; 
    private Vector3 _velocity; // 현재 속도
    private bool _isGrounded; // 땅에 닿아 있는지 여부

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
    /// 플레이어가 땅에 닿아있는지 판단 + 땅에 닿아있고 속도가 아래로 향할 때 속도 리셋
    /// </summary>
    private void GroundCheck()
    {
        _isGrounded = _characterController.isGrounded;

        // 땅에 닿아 있고, y축 속도가 아래로 향하고 있으면
        if (_isGrounded && _velocity.y < 0)
        {
            // y축 속도를 고정하여 플레이어가 바닥에 안정적으로 붙어있게 함
            _velocity.y = -2.0f;
        }
    }

    /// <summary>
    /// 이동 관련 입력을 받아 플레이어의 이동 방향 계산
    /// </summary>
    /// <returns>이동 방향 벡터</returns>
    private Vector3 GetInputMovement()
    {
        float x = Input.GetAxis("Horizontal"); // 좌우 이동 (A/D or ←/→)
        float z = Input.GetAxis("Vertical"); // 앞뒤 이동 (W/S or ↑/↓)

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// 이동, 점프 처리
    /// </summary>
    private void Move()
    {
        Vector3 inputDir = GetInputMovement(); // 입력에 따른 방향 벡터 계산

        IsMoving = inputDir.sqrMagnitude > 0.01f;

        if (IsMoving)
        {
            inputDir = inputDir.normalized;

            _moveSpeed = _player.State.IsSatietyZero ? _player.State.MoveSpeed * 0.5f : _player.State.MoveSpeed;

            // 이동 처리
            _characterController.Move(inputDir * _moveSpeed *  Time.deltaTime); 
            
            // 이동 방향을 바라보도록 회전
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7.0f);

            // 이동에 따른 포만감 감소
            if (!_player.State.IsSatietyZero)
                _player.State.DecreaseSatiety(_satietyDecreaseAmount);
        }
        

        // 스페이스 입력 && 플레이어가 땅에 닿은 상태일 때
        if(_isGrounded && Input.GetButtonDown("Jump"))
        {
            _jumpHeight = _player.State.IsSatietyZero ? _player.State.JumpHeight * 0.5f : _player.State.JumpHeight;

            // 점프 높이와 중력으로 초기 y속도 계산
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
        }

        ApplyGravity();
    }

    /// <summary>
    /// 중력 계산 담당
    /// </summary>
    private void ApplyGravity()
    {
        _velocity.y += _gravity * Time.deltaTime; // 중력을 누적시켜 시간이 지날수록 더 빠르게 떨어지도록 함
        _characterController.Move(_velocity * Time.deltaTime); // 최종 이동 적용
    }
}
