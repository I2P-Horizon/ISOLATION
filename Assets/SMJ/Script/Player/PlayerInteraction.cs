using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 플레이어의 상호작용을 관리하는 스크립트 (채집, 사냥 등).
/// 마우스 좌클릭: 채집/공격
/// F키: 근처 아이템 줍기
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private Player _player;

    enum InteractionState
    {
        None, // 아무 것도 하지 않는 상태
        Gathering, // 채집 중인 상태
        Attacking // 공격 중인 상태
    }

    [SerializeField] private Inventory _inventory; // 플레이어 인벤토리

    [Header("Common")]
    /// <summary>상호작용(채집, 사냥) 가능한 거리</summary>
    [SerializeField] private float _interactionDistance = 2.0f;

    [Header("Gather")]
    /// <summary>채집 시 오브젝트 내구도 감소량</summary>
    [SerializeField] private float _gatherStrength = 5.0f;
    /// <summary>지속 채집 간격</summary>
    [SerializeField] private float _gatherInterval = 1.0f;
    /// <summary>채집 시 포만감 감소량</summary>
    [SerializeField] private float _satietyDecreaseAmount = 0.05f;

    [Header("Attack")]
    /// <summary>공격력</summary>
    [SerializeField] private float _attackPower = 10.0f;
    /// <summary>공격 속도</summary>
    [SerializeField] private float _attackInterval = 0.8f;

    [Header("CraftingTable Interaction")]
    /// <summary>제작대 감지 반경</summary>
    [SerializeField] private float _ctDetectRadius = 1.0f;
    /// <summary>제작대 레이어</summary>
    [SerializeField] private LayerMask _ctLayerMask;

    private List<PickupItem> _ItemsInScope; // 플레이어 주변에 감지된 아이템 목록

    private InteractionState _currentState = InteractionState.None; // 현재 상태
    private MonoBehaviour _currentTarget = null; // 현재 상호작용 중인 대상
    private float _lastInteractionTime = 0; // 마지막으로 상호작용한 시간

    public bool IsInteracting => _currentState != InteractionState.None;

    private void Awake()
    {
        _player = GetComponent<Player>();
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
            if (TryInteractionWithCraftingTable()) return;

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
        if (_currentState == InteractionState.None || _currentTarget == null)
            return;

        // target이 여전히 상호작용 가능한 거리 내에 있는지 확인
        bool isWithinDistance = Vector3.Distance(
            _currentTarget.GetComponent<Collider>().ClosestPoint(transform.position), 
            transform.position) <= _interactionDistance;
        if (!isWithinDistance) // 대상과 멀어졌으면 상호작용 중단
        {
            StopInteraction();
            return;
        }

        if (Time.time - _lastInteractionTime < GetCurrentInterval())
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

        // ray에 걸리지 않는 것: trigger collider, 'Ignore Raycast' Layer로 설정된 객체
        int mask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit mouseHit, 100f, mask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"Camera : {mouseHit.collider.name}");
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green, 1f);
            Vector3 dir = (mouseHit.collider.bounds.center - transform.position).normalized;
            //Debug.Log($"mouseHit.transform.position : {mouseHit.transform.position}");
            //Debug.Log($"mouseHit.transform.localPosition : {mouseHit.transform.localPosition}");
            //Debug.Log($"transform.position : {transform.position}");

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, _interactionDistance, mask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log($"Player : {hit.collider.name}");
                Debug.DrawRay(transform.position, dir * _interactionDistance, Color.green, 1f);
                if (hit.collider.TryGetComponent(out DestructibleObject destructible))
                {
                    Debug.Log("Targetting");
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
        else if (target is CreatureBase)
            _currentState = InteractionState.Attacking;
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
                _player.State.DecreaseSatiety(_satietyDecreaseAmount);
                break;
            case InteractionState.Attacking:
                (_currentTarget as CreatureBase)?.Interact(_attackPower);
                break;
        }
    }

    /// <summary>
    /// 현재 상태에 따른 상호작용 간격 반환
    /// </summary>
    private float GetCurrentInterval()
    {
        float baseInterval = _currentState switch
        {
            InteractionState.Gathering => _gatherInterval,
            InteractionState.Attacking => _attackInterval,
            _ => float.MaxValue
        };

        return _player.State.IsSatietyZero ? baseInterval * 2f : baseInterval;
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
        if (!TryFindNearestItem(out PickupItem nearestItem))
        {
            return;
        }

        if (!CanReachItem(nearestItem))
        {
            return;
        }

        if (nearestItem.CanBePickedUp())
        {
            
            if (_inventory.AddItem(nearestItem.ItemData) == 0)
            {
                nearestItem.Interact();
                _ItemsInScope.Remove(nearestItem);
            }
        }
    }

    /// <summary>
    /// ItemInScope 중에서 플레이어와 가장 가까이 있는 아이템을 찾아 반환
    /// </summary>
    private bool TryFindNearestItem(out PickupItem nearestItem)
    {
        nearestItem = null;

        _ItemsInScope.RemoveAll(item => item == null);

        if (_ItemsInScope.Count == 0)
        {
            return false;
        }

        nearestItem = _ItemsInScope[0];
        for (int i = 0; i < _ItemsInScope.Count; i++)
        {
            if (Vector3.Distance(transform.position, nearestItem.transform.position) >
            Vector3.Distance(transform.position, _ItemsInScope[i].transform.position))
            {
                nearestItem = _ItemsInScope[i];
            }
        }

        return true;
    }

    /// <summary>
    /// 플레이어 -> 주우려는 아이템 방향으로 Ray를 쏴서 둘 사이에 장애물이 없는지 확인
    /// </summary>
    private bool CanReachItem(PickupItem item)
    {
        Vector3 dir = (item.transform.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, item.transform.position);

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dist, ~0, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(transform.position, dir * dist, Color.red, 1f);
            return hit.collider.gameObject == item.gameObject;
        }

        return false;
    }

    private bool TryInteractionWithCraftingTable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _ctDetectRadius, _ctLayerMask);

        CraftingTable nearestCT = null;
        float nearestDist = float.MaxValue;

        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out CraftingTable ct))
            {
                float dist = Vector3.Distance(transform.position, ct.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestCT = ct;
                }
            }
        }

        if (nearestCT != null)
        {
            nearestCT.Interact();
            return true;
        }

        return false;
    }
}
