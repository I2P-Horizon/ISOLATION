using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 이동 및 회전 관리
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField] protected Transform _targetTransform; // 따라갈 대상 (플레이어)
    /// <summary>
    /// 상대 위치. 플레이어보다 뒤쪽 위에 위치하기 위한 거리
    /// </summary>
    [SerializeField] protected Vector3 _offset = new Vector3(0f, 3.0f, -10.0f);
    /// <summary>
    /// 부드러움 정도 (숫자가 작을수록 빠르게 움직임)
    /// </summary>
    [SerializeField] protected float _smoothTime = 0.1f;

    /// <summary>
    /// 한 번에 회전할 각도
    /// </summary>
    [SerializeField] protected float _rotationAngle = 30f;
    /// <summary>
    /// 카메라가 바라봐야 하는 목표 x축 회전 각도
    /// </summary>
    protected float _currentXRotation;
    /// <summary>
    /// 카메라가 바라봐야 하는 목표 y축 회전 각도
    /// </summary>
    protected float _currentYRotation;

    protected Vector3 _velocity = Vector3.zero; // 현재 속도
    protected Quaternion _targetRotation; // 목표 회전 값
    protected float _initialXRotation; // 기존 x회전 값 저장용

    /// <summary>
    /// 신호에 따라 HpUI 회전
    /// </summary>
    public static event Action OnRotate;

    protected virtual void Start()
    {
        // 시작 시 카메라 위치 조정
        transform.position = _targetTransform.position + _offset;

        // 초기 회전 값 저장
        _targetRotation = transform.rotation;

        // 카메라 초기 회전 값 저장
        _currentXRotation = transform.eulerAngles.x;
        _currentYRotation = transform.eulerAngles.y;

        // 기존 x 회전 값(기울기) 저장
        _initialXRotation = transform.eulerAngles.x;
    }

    protected virtual void LateUpdate()
    {
        // 목표 위치 계산
        Vector3 targetPosition = _targetTransform.position + _offset;

        // 현재 위치에서 목표 위치로 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);

        // KSW: 카메라 회전
        rotation();
    }

    #region 카메라 회전
    /// <summary>
    /// 카메라 회전
    /// </summary>
    protected virtual void rotation()
    {
        /* 수직 회전(R 키) */
        /* R을 누르면 카메라를 _rotationAngle 만큼 기울이고, R을 떼면 원래 X각도로 복귀 */
        if (Input.GetKey(KeyCode.R)) _currentXRotation = _initialXRotation - _rotationAngle;
        else _currentXRotation = _initialXRotation;

        /* 수평 회전(Q/E 키) */
        float angle = 0f;

        /* Q: 왼쪽으로 회전 */
        /* E: 오른쪽으로 회전 */
        if (Input.GetKeyDown(KeyCode.Q))
        {
            angle = _rotationAngle;

            /* 회전 시 신호 발생 */
            OnRotate?.Invoke();
        }

        else if (Input.GetKeyDown(KeyCode.E))
        {
            angle = -_rotationAngle;
            OnRotate?.Invoke();
        }

        /* 회전 입력이 들어오면 */
        if (angle != 0f)
        {
            /* offset을 회전시켜서 카메라의 상대 위치를 변경 */
            _offset = Quaternion.Euler(0, angle, 0) * _offset;

            /* 목표 Y 각도 누적 */
            _currentYRotation += angle;
        }

        /* 회전 적용 */
        /* 목표 회전값(X: 위아래 / Y: 좌우) */
        _targetRotation = Quaternion.Euler(_currentXRotation, _currentYRotation, 0);

        /* 부드럽게 회전 */
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime / _smoothTime);
    }
    #endregion
}