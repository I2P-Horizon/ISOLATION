using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 이동 관리
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; // 따라갈 대상 (플레이어)
    /// <summary>
    /// 상대 위치. 플레이어보다 뒤쪽 위에 위치하기 위한 거리
    /// </summary>
    [SerializeField] private Vector3 offset = new Vector3(0f, 3.0f, -10.0f);
    /// <summary>
    /// 부드러움 정도 (숫자가 작을수록 빠르게 움직임)
    /// </summary>
    [SerializeField] private float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero; // 현재 속도

    private void Start()
    {
        // 시작 시 카메라 위치 조정
        transform.position = targetTransform.position + offset;
    }

    private void LateUpdate()
    {
        // 목표 위치 계산
        Vector3 targetPosition = targetTransform.position + offset;

        // 현재 위치에서 목표 위치로 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
