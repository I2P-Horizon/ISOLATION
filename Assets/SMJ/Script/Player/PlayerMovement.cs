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

    private CharacterController _characterController;
    private Animator _animator;

    [SerializeField] private float _moveSpeed; // 이동 속도
    private float _jumpHeight; // 점프 높이
    [SerializeField] private float _gravity = -9.81f; // 중력가속도(음수)

    private Vector3 _velocity; // 현재 속도
    private bool _isGrounded; // 땅에 닿아 있는지 여부

    public bool IsMoving = false;

    public bool CanMove = true;

    void Start()
    {
        _player = GetComponent<Player>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _moveSpeed = _player.State.MoveSpeed;
        _jumpHeight = _player.State.JumpHeight;

        CanMove = false;
        IsMoving = false;

        IslandManager.OnGenerationComplete += startWakeUpSequence;
    }

    private void OnDestroy()
    {
        IslandManager.OnGenerationComplete -= startWakeUpSequence;
    }

    void Update()
    {
        if (CanMove == false || _player.State.Die || _player.State.IsDeadSequenceStarted) return;

        _moveSpeed = _player.State.MoveSpeed;

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
        float x = Input.GetAxisRaw("Horizontal"); // 좌우 이동 (A/D or ←/→)
        float z = Input.GetAxisRaw("Vertical"); // 앞뒤 이동 (W/S or ↑/↓)

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// 이동, 점프 처리
    /// </summary>
    private void Move()
    {
        Vector3 inputDir = GetInputMovement(); // 입력에 따른 방향 벡터 계산

        // 혼란 상태일 때 이동 방향 반전
        if (_player != null && _player.Condition.IsConfused)
        {
            inputDir *= -1;
        }

        IsMoving = inputDir.sqrMagnitude != 0f;

        _velocity.x = 0f; // x축 속도 초기화
        _velocity.z = 0f; // z축 속도 초기화

        if (IsMoving)
        {
            inputDir = inputDir.normalized;

            _moveSpeed = _player.State.IsSatietyZero ? _player.State.MoveSpeed * 0.5f : _player.State.MoveSpeed;

            #region KSW: 카메라 방향 기준 이동 처리

            /* 카메라 기준 방향 벡터 가져오기 */
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            /* 수평 이동만 사용하기 위해 y값 제거 */
            camForward.y = 0f;
            camRight.y = 0f;

            /* y값 제거로 벡터 길이가 달라지므로 정규화 (이동 속도 일정하게 유지) */
            camForward.Normalize();
            camRight.Normalize();

            /*
             * 입력 방향을 카메라 기준으로 변환.
             * inputDir.z -> camForward 방향(앞/뒤)
             * inputDir.x -> camRight 방향(좌/우)
             * 두 방향을 더해 최종 이동 방향(moveDir)을 계산한다.
            */
            Vector3 moveDir = (camForward * inputDir.z) + (camRight * inputDir.x);
            #endregion

            // 이동 처리
            _characterController.Move(moveDir * _moveSpeed * Time.deltaTime);

            // 이동 방향을 바라보도록 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
        }

        // 스페이스 입력 && 플레이어가 땅에 닿은 상태일 때
        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            _jumpHeight = _player.State.IsSatietyZero ? _player.State.JumpHeight * 0.5f : _player.State.JumpHeight;

            // 점프 높이와 중력으로 초기 y속도 계산
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
            _animator.SetTrigger("Jump");
        }

        #region KSW: 애니메이션 적용
        /* 이동 중인지 여부에 따라 isMoving 조건 변경 */
        bool isMovingAir = inputDir.sqrMagnitude != 0f;
        _animator.SetBool("isMoving", isMovingAir);

        _animator.SetFloat("Horizontal", inputDir.x, 0.05f, Time.deltaTime);
        _animator.SetFloat("Vertical", inputDir.z, 0.05f, Time.deltaTime);
        #endregion

        ApplyGravity();

        //_animator.SetBool("isMoving", IsMoving); // 애니메이션 상태 업데이트
    }

    /// <summary>
    /// 중력 계산 담당
    /// </summary>
    private void ApplyGravity()
    {
        _velocity.y += _gravity * Time.deltaTime; // 중력을 누적시켜 시간이 지날수록 더 빠르게 떨어지도록 함
        _characterController.Move(_velocity * Time.deltaTime); // 최종 이동 적용
    }

    private void startWakeUpSequence()
    {
        StartCoroutine(WakeUpSequence());
    }

    private IEnumerator WakeUpSequence()
    {
        _animator.SetTrigger("WakeUp");

        yield return new WaitForSeconds(0.1f);

        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("GettingUp") ||
               _animator.GetCurrentAnimatorStateInfo(0).IsName("Laying"))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("GettingUp") &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }

        CanMove = true;
    }
}