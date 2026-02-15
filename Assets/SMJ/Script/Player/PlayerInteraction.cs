using InventorySystem;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 플레이어의 상호작용을 관리하는 스크립트 (채집, 사냥 등).
/// 마우스 좌클릭: 채집/공격
/// F키: 근처 아이템 줍기
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private Player _player;
    private Animator _animator;

    enum InteractionState
    {
        None, // 아무 것도 하지 않는 상태
        Gathering, // 채집 중인 상태
        Attacking // 공격 중인 상태
    }

    [SerializeField] private Inventory _inventory; // 플레이어 인벤토리

    [Header("Interaction Settings")]
    /// <summary>상호작용(채집, 사냥) 가능한 거리</summary>
    [SerializeField] private float _interactionDistance = 1.0f;
    [SerializeField] private float _detectRadius = 0.5f;
    [SerializeField, Range(0, 180)] private float _detectAngle = 60.0f;
    [SerializeField] private LayerMask _waterLayer;

    [Header("CraftingTable Interaction")]
    /// <summary>제작대 감지 반경</summary>
    [SerializeField] private float _ctDetectRadius = 1.0f;
    /// <summary>제작대 레이어</summary>
    [SerializeField] private LayerMask _ctLayerMask;

    [Header("Campfire Interaction")]
    [SerializeField] private float _campfireDetectRadius = 1.0f;
    [SerializeField] private LayerMask _campfireLayerMask;

    private List<PickupItem> _ItemsInScope; // 플레이어 주변에 감지된 아이템 목록

    private InteractionState _currentState = InteractionState.None; // 현재 상태
    private MonoBehaviour _currentTarget = null; // 현재 상호작용 중인 대상
    private float _lastInteractionTime = 0; // 마지막으로 상호작용한 시간

    private const float BASE_DURABILITY_COST = 2.0f; // 기본 내구도 소모량

    public bool IsInteracting => _currentState != InteractionState.None;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _ItemsInScope = new List<PickupItem>();
    }

    private void Update()
    {
        // UI 위에서의 입력 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()
            || InventoryUI.IsDraggingItem)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnInteractionPressed();
        }

        if (Input.GetMouseButton(0))
        {
            OnInteractionHeld();
        }

        if (Input.GetMouseButtonDown(1))
        {
            tryFillBucket();
        }    

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (TryInteractionWithCraftingTable()) return;
            if (TryInteractionWithCampfire()) return;

            TryPickupItem();
        }
    }

    /// <summary>
    /// 좌클릭 시 상호작용 대상 탐색 및 첫 상호작용 수행
    /// </summary>
    private void OnInteractionPressed()
    {
        if (Time.time - _lastInteractionTime < GetCurrentInterval())
            return;

        if (TryFindTarget(out MonoBehaviour target))
        {
            _currentTarget = target;
            UpdateInteractionState(target);
        }
        else
        {
            _currentTarget = null;
            _currentState = InteractionState.Attacking;
        }

        startInteractionAnimation();
        _lastInteractionTime = Time.time;
    }

    /// <summary>
    /// 좌클릭 유지 시 Interval마다 지속적으로 상호작용 수행
    /// </summary>
    private void OnInteractionHeld()
    {
        if (Time.time - _lastInteractionTime < GetCurrentInterval())
            return;

        if (TryFindTarget(out MonoBehaviour target))
        {
            _currentTarget = target;
            UpdateInteractionState(target);
        }
        else
        {
            _currentTarget = null;
            _currentState = InteractionState.Attacking;
        }

        startInteractionAnimation();
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

    private bool isTargetInSight(MonoBehaviour target)
    {
        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
        dirToTarget.y = 0;
        Vector3 fwd = transform.forward;
        fwd.y = 0;

        float angle = Vector3.Angle(fwd, dirToTarget);
        return angle <= _detectAngle;
    }

    /// <summary>
    /// 상호작용 가능 거리 내에 있는 상호작용 가능한 대상 탐색
    /// </summary>
    private bool TryFindTarget(out MonoBehaviour target)
    {
        target = null;
        int mask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, _detectRadius, direction, out RaycastHit hit, _interactionDistance, mask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent(out DestructibleObject destructible))
            {
                if (isTargetInSight(destructible))
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
        else if (target is CreatureBase)
            _currentState = InteractionState.Attacking;
        else
            _currentState = InteractionState.Attacking;
    }

    private void startInteractionAnimation()
    {
        float attackSpeed = _player.State.AttackSpeed;

        _animator.SetFloat("AttackSpeed", attackSpeed);

        _animator.SetTrigger("Gathering");
    }

    public void OnAnimationHit()
    {
        if (_currentState == InteractionState.None)
            return;

        EquipmentItem currentEquip = _player.Equipment.GetEquippedItem(EquipmentType.RightHand);
        WeaponItem weapon = currentEquip as WeaponItem;

        string weaponType = weapon != null ? weapon.WeaponData.Type : "None";

        float finalDamage = _player.State.AttackPower;
        float durabilityCost = BASE_DURABILITY_COST;

        if (_currentState == InteractionState.Gathering && _currentTarget is GatherableObject gatherable)
        {
            calculateGatheringStats(weaponType, gatherable, ref finalDamage, ref durabilityCost);

            gatherable.Interact(finalDamage);
            if (weapon != null) applyDurabilityLoss(weapon, durabilityCost);
        }
        else if (_currentState == InteractionState.Attacking && _currentTarget is CreatureBase creature)
        {
            creature.Interact(finalDamage);
            if (weapon != null) applyDurabilityLoss(weapon, durabilityCost);
        }

        if (!Input.GetMouseButton(0))
        {
            StopInteraction();
        }
    }

    public void OnAnimationWatering()
    {
        ItemData data = DataManager.Instance.GetItemDataByID(50009);
        bool result = _inventory.AddItem(data) == 0;

        if (result)
        {
            _player.Equipment.UnEquipBucket();
        }
    }

    private void calculateGatheringStats(string weaponType, GatherableObject gatherable, ref float damage, ref float durability)
    {
        switch (weaponType)
        {
            case "Axe":
                if (gatherable is RockObject || gatherable is GemStoneObject)
                {
                    damage *= 0.6f;
                    durability *= 1.4f;
                }
                break;
            case "Pickax":
                if (gatherable is TreeObject || gatherable is FruitTreeObject)
                {
                    damage *= 0.5f;
                    durability *= 1.3f;
                }
                break;
            case "Sword":
                if (gatherable is TreeObject || gatherable is FruitTreeObject
                    || gatherable is RockObject || gatherable is GemStoneObject)
                {
                    damage *= 0.4f;
                    durability *= 1.4f;
                }
                break;
        }
    }

    private void applyDurabilityLoss(WeaponItem weapon, float durabilityCost)
    {
        bool isBroken = weapon.DecreaseDurability(durabilityCost);

        if (isBroken)
        {
            _player.Equipment.UnEquip(EquipmentType.RightHand);
        }
    }

    /// <summary>
    /// 현재 상태에 따른 상호작용 간격 반환
    /// </summary>
    private float GetCurrentInterval()
    {
        float currentAttackSpeed = _player.State.AttackSpeed;

        if (currentAttackSpeed <= 0) currentAttackSpeed = 1.0f;

        float interval = 1.0f / currentAttackSpeed;

        return _player.State.IsSatietyZero ? interval * 2.0f : interval;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(origin + transform.forward * _interactionDistance, _detectRadius);
        Gizmos.DrawLine(origin, origin + transform.forward * _interactionDistance);
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

    private void tryFillBucket()
    {
        EquipmentItem currentItem = _player.Equipment.GetEquippedItem(EquipmentType.RightHand);
        WeaponItem currentWeapon = currentItem as WeaponItem;

        if (currentWeapon == null || currentWeapon.WeaponData.Type != "Bucket")
        {
            return;
        }

        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 direction = (transform.forward + Vector3.down * 0.5f).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, 2.0f, _waterLayer, QueryTriggerInteraction.Collide))
        {
            Debug.DrawRay(origin, direction * 2.0f, Color.blue, 1f);
            tryBucketFill();
        }
    }

    private void tryBucketFill()
    {
        _animator.SetTrigger("Watering");
    }

    private bool TryInteractionWithCampfire()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _campfireDetectRadius, _campfireLayerMask);

        Campfire nearestCampfire = null;
        float nearestDist = float.MaxValue;

        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out Campfire campfire))
            {
                float dist = Vector3.Distance(transform.position, campfire.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestCampfire = campfire;
                }
            }
        }

        if (nearestCampfire != null)
        {
            nearestCampfire.TryCook(_inventory);

            return true;
        }

        return false;
    }
}
