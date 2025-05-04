using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 상호작용을 관리하는 스크립트 (채집, 사냥 등)
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    enum InteractionState
    {
        None, // 아무 것도 하지 않는 상태
        Gathering, // 채집 중인 상태
        Attacking // 공격 중인 상태
    }

    /// <summary>
    /// 상호작용 가능한 거리
    /// </summary>
    [SerializeField] private float _interactionDistance = 5.0f;
    /// <summary>
    /// 채집 시 내구도 감소량
    /// </summary>
    [SerializeField] private float _gatherStrength = 5.0f;
    /// <summary>
    /// 지속 채집 간격
    /// </summary>
    [SerializeField] private float _gatherInterval = 1.0f;

    private InteractionState _currentState = InteractionState.None; // 현제 상태
    private MonoBehaviour _currentTarget = null; // 현재 상호작용 중인 대상

    private float _holdTime = 0; // 좌클릭 유지 시간

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Mouse Click");
            TryInteraction();
            _holdTime = 0;
        }

        if (Input.GetMouseButton(0) && _currentState != InteractionState.None)
        {
            _holdTime += Time.deltaTime;

            if (_currentState == InteractionState.Gathering)
            {
                if(_holdTime >= _gatherInterval)
                {
                    Debug.Log("Holding Left Mouse Button");
                    TryInteraction();
                    _holdTime = 0;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopInteraction();
        }
    }

    /// <summary>
    /// 상호작용 가능한 대상이 있는지 확인하고, 대상의 종류에 맞게 상호작용 상태 설정
    /// </summary>
    private void TryInteraction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 마우스 방향으로 쏜 ray가 닿는 점을 얻음
        if (Physics.Raycast(ray, out RaycastHit mouseHitInfo, 100f))
        {
            // 플레이어 위치에서 마우스가 가리키는 world point 방향으로 ray 쏘기
            Vector3 from = transform.position;
            Vector3 to = mouseHitInfo.point;
            Vector3 direction = (to - from).normalized;

            if (Physics.Raycast(from, direction, out RaycastHit hit, _interactionDistance))
            {
                GatherableObject gatherable = hit.collider.GetComponent<GatherableObject>();
                if (gatherable != null)
                {
                    _currentState = InteractionState.Gathering;
                    _currentTarget = gatherable;
                    ProcessInteraction();
                    return;
                }
            }
        }

        _currentState = InteractionState.None;
        _currentTarget = null;
    }

    /// <summary>
    /// 현재 상태에 따라 정해진 상호작용 수행
    /// </summary>
    private void ProcessInteraction()
    {
        switch (_currentState)
        {
            case InteractionState.Gathering:
                Gathering();
                break;
            case InteractionState.Attacking:
                // 공격 함수 호출
                break;
        }
    }

    /// <summary>
    /// 현재 진행중인 상호작용을 종료시킴
    /// </summary>
    private void StopInteraction()
    {
        _currentState = InteractionState.None;
        _holdTime = 0;
    }

    /// <summary>
    /// 채집 수행 함수
    /// </summary>
    private void Gathering()
    {
        if (_currentTarget == null)
        {
            StopInteraction();
            return;
        }

        GatherableObject gatherable = _currentTarget as GatherableObject;
        gatherable.Interact(_gatherStrength);
    }
}
