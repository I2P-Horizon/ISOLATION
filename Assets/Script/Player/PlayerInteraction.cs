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
    [SerializeField] private float interactionDistance = 5.0f;
    /// <summary>
    /// 채집 시 내구도 감소량
    /// </summary>
    [SerializeField] private float gatherStrength = 5.0f;
    /// <summary>
    /// 지속 채집 간격
    /// </summary>
    [SerializeField] private float gatherInterval = 1.0f;

    private InteractionState currentState = InteractionState.None; // 현제 상태
    private MonoBehaviour currentTarget = null; // 현재 상호작용 중인 대상

    private float holdTime = 0; // 좌클릭 유지 시간

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Mouse Click");
            TryInteraction();
            holdTime = 0;
        }

        if (Input.GetMouseButton(0) && currentState != InteractionState.None)
        {
            holdTime += Time.deltaTime;

            if (currentState == InteractionState.Gathering)
            {
                if(holdTime >= gatherInterval)
                {
                    Debug.Log("Holding Left Mouse Button");
                    TryInteraction();
                    holdTime = 0;
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

            if (Physics.Raycast(from, direction, out RaycastHit hit, interactionDistance))
            {
                GatherableObject gatherable = hit.collider.GetComponent<GatherableObject>();
                if (gatherable != null)
                {
                    currentState = InteractionState.Gathering;
                    currentTarget = gatherable;
                    ProcessInteraction();
                    return;
                }
            }
        }

        currentState = InteractionState.None;
        currentTarget = null;
    }

    /// <summary>
    /// 현재 상태에 따라 정해진 상호작용 수행
    /// </summary>
    private void ProcessInteraction()
    {
        switch (currentState)
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
        currentState = InteractionState.None;
        holdTime = 0;
    }

    /// <summary>
    /// 채집 수행 함수
    /// </summary>
    private void Gathering()
    {
        if (currentTarget == null)
        {
            StopInteraction();
            return;
        }

        GatherableObject gatherable = currentTarget as GatherableObject;
        gatherable.Interact(gatherStrength);
    }
}
