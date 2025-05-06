using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 플레이어의 상호작용을 관리하는 스크립트 (채집, 사냥 등).
/// 마우스 좌클릭: 채집/공격
/// F키: 근처 아이템 줍기
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    enum InteractionState
    {
        None, // 아무 것도 하지 않는 상태
        Gathering, // 채집 중인 상태
        Attacking // 공격 중인 상태
    }

    [SerializeField] private Inventory _inventory; // 플레이어 인벤토리

    [Header("Common")]
    /// <summary>상호작용(채집, 사냥) 가능한 거리</summary>
    [SerializeField] private float _interactionDistance = 5.0f;

    [Header("Gather")]
    /// <summary>채집 시 내구도 감소량</summary>
    [SerializeField] private float _gatherStrength = 5.0f;
    /// <summary>지속 채집 간격</summary>
    [SerializeField] private float _gatherInterval = 1.0f;

    [Header("Attack")]
    /// <summary>공격력</summary>
    [SerializeField] private float _attackPower = 10.0f;
    /// <summary>공격 속도</summary>
    [SerializeField] private float _attackInterval = 0.8f;

    private List<PickupItem> _ItemsInScope; // 플레이어 주변에 감지된 아이템 목록

    private InteractionState _currentState = InteractionState.None; // 현제 상태
    private MonoBehaviour _currentTarget = null; // 현재 상호작용 중인 대상
    private float _lastInteractionTime = 0; // 마지막으로 상호작용한 시간

    public bool IsInteracting => _currentState != InteractionState.None;

    private void Awake()
    {
        _ItemsInScope = new List<PickupItem>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnInteractionPressed();
        }

        if (Input.GetMouseButton(0))
        {
            OnInteractionHeld();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopInteraction();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryPickupItem();
        }
    }

    /// <summary>
    /// 좌클릭 시 상호작용 대상 탐색 및 첫 상호작용 수행
    /// </summary>
    private void OnInteractionPressed()
    {
        if (TryFindTarget(out _currentTarget))
        {
            UpdateInteractionState(_currentTarget);

            if (Time.time - _lastInteractionTime < GetCurrentInterval()) { return; }

            ProcessInteraction();
            _lastInteractionTime = Time.time;
        }
        else
        {
            StopInteraction();
        }
    }

    /// <summary>
    /// 좌클릭 유지 시 Interval마다 지속적으로 상호작용 수행
    /// </summary>
    private void OnInteractionHeld()
    {
        if (_currentState == InteractionState.None || Time.time - _lastInteractionTime < GetCurrentInterval())
            return;

        ProcessInteraction();
        _lastInteractionTime = Time.time;
    }

    /// <summary>
    /// 상호작용 중단
    /// </summary>
    private void StopInteraction()
    {
        _currentState = InteractionState.None;
        _currentTarget = null;
    }

    /// <summary>
    /// 상호작용 가능 거리 내에 있는 상호작용 가능한 대상 탐색
    /// </summary>
    private bool TryFindTarget(out MonoBehaviour target)
    {
        target = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit mouseHit, 100f, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 dir = (mouseHit.point - transform.position).normalized;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, _interactionDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawRay(transform.position, dir * _interactionDistance, Color.green, 1f);
                if (hit.collider.TryGetComponent(out DestructibleObject destructible))
                {
                    target = destructible;
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 상호작용 대상에 따라 상호작용 상태 결정
    /// </summary>
    private void UpdateInteractionState(MonoBehaviour target)
    {
        if (target is GatherableObject)
            _currentState = InteractionState.Gathering;
        //else if (target is CreatureBase)
        //    _currentState = InteractionState.Attacking;
        else
            _currentState = InteractionState.None;
    }

    /// <summary>
    /// 현재 상태에 따라 상호작용 수행
    /// </summary>
    private void ProcessInteraction()
    {
        switch (_currentState)
        {
            case InteractionState.Gathering:
                (_currentTarget as GatherableObject)?.Interact(_gatherStrength);
                break;
            //case InteractionState.Attacking:
            //    (_currentTarget as CreatureBase)?.Interact(_attackPower);
            //    break;
        }
    }

    /// <summary>
    /// 현재 상태에 따른 상호작용 간격 반환
    /// </summary>
    private float GetCurrentInterval()
    {
        return _currentState switch
        {
            InteractionState.Gathering => _gatherInterval,
            InteractionState.Attacking => _attackInterval,
            _ => float.MaxValue
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        PickupItem item = other.GetComponent<PickupItem>();
        if (item != null)
            _ItemsInScope.Add(item);
    }

    private void OnTriggerExit(Collider other)
    {
        PickupItem item = other.GetComponent<PickupItem>();
        if (item != null)
            _ItemsInScope.Remove(item);
    }

    /// <summary>
    /// 가장 가까운 아이템을 주우려 시도.
    /// 인벤토리에 공간이 있는 경우에만 아이템을 주움.
    /// </summary>
    private void TryPickupItem()
    {
        if (_ItemsInScope.Count == 0) return;

        PickupItem nearestItem = _ItemsInScope[0];
        for (int i = 0; i < _ItemsInScope.Count; i++)
        {
            if (Vector3.Distance(transform.position, nearestItem.transform.position) >
                Vector3.Distance(transform.position, _ItemsInScope[i].transform.position))
            {
                nearestItem = _ItemsInScope[i];
            }
        }

        // <인벤토리 연결 전 확인용 코드>
        nearestItem.Interact();
        _ItemsInScope.Remove(nearestItem);

        // <인벤토리 연결 시 아래 코드 주석 제거 후 사용>
        //if (_inventory.Add(nearestItem.ItemData) == 0)
        //{
        //    nearestItem.Interact();
        //    _ItemsInScope.Remove(nearestItem);
        //}
    }
}
