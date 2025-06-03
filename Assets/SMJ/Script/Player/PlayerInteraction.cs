using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �÷��̾��� ��ȣ�ۿ��� �����ϴ� ��ũ��Ʈ (ä��, ��� ��).
/// ���콺 ��Ŭ��: ä��/����
/// FŰ: ��ó ������ �ݱ�
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private Player _player;

    enum InteractionState
    {
        None, // �ƹ� �͵� ���� �ʴ� ����
        Gathering, // ä�� ���� ����
        Attacking // ���� ���� ����
    }

    [SerializeField] private Inventory _inventory; // �÷��̾� �κ��丮

    [Header("Common")]
    /// <summary>��ȣ�ۿ�(ä��, ���) ������ �Ÿ�</summary>
    [SerializeField] private float _interactionDistance = 5.0f;

    [Header("Gather")]
    /// <summary>ä�� �� ������Ʈ ������ ���ҷ�</summary>
    [SerializeField] private float _gatherStrength = 5.0f;
    /// <summary>���� ä�� ����</summary>
    [SerializeField] private float _gatherInterval = 1.0f;
    /// <summary>ä�� �� ������ ���ҷ�</summary>
    [SerializeField] private float _satietyDecreaseAmount = 0.05f;

    [Header("Attack")]
    /// <summary>���ݷ�</summary>
    [SerializeField] private float _attackPower = 10.0f;
    /// <summary>���� �ӵ�</summary>
    [SerializeField] private float _attackInterval = 0.8f;

    private List<PickupItem> _ItemsInScope; // �÷��̾� �ֺ��� ������ ������ ���

    private InteractionState _currentState = InteractionState.None; // ���� ����
    private MonoBehaviour _currentTarget = null; // ���� ��ȣ�ۿ� ���� ���
    private float _lastInteractionTime = 0; // ���������� ��ȣ�ۿ��� �ð�

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
            //Debug.Log("F");
            TryPickupItem();
        }
    }

    /// <summary>
    /// ��Ŭ�� �� ��ȣ�ۿ� ��� Ž�� �� ù ��ȣ�ۿ� ����
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
    /// ��Ŭ�� ���� �� Interval���� ���������� ��ȣ�ۿ� ����
    /// </summary>
    private void OnInteractionHeld()
    {
        if (_currentState == InteractionState.None || Time.time - _lastInteractionTime < GetCurrentInterval())
            return;

        ProcessInteraction();
        _lastInteractionTime = Time.time;
    }

    /// <summary>
    /// ��ȣ�ۿ� �ߴ�
    /// </summary>
    private void StopInteraction()
    {
        _currentState = InteractionState.None;
        _currentTarget = null;
    }

    /// <summary>
    /// ��ȣ�ۿ� ���� �Ÿ� ���� �ִ� ��ȣ�ۿ� ������ ��� Ž��
    /// </summary>
    private bool TryFindTarget(out MonoBehaviour target)
    {
        target = null;

        // ray�� �ɸ��� �ʴ� ��: trigger collider, 'Ignore Raycast' Layer�� ������ ��ü
        int mask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit mouseHit, 100f, mask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"Camera : {mouseHit.collider.name}");
            Vector3 dir = (mouseHit.point - transform.position);

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, _interactionDistance, mask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log($"Player {hit.collider.name}");
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
    /// ��ȣ�ۿ� ��� ���� ��ȣ�ۿ� ���� ����
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
    /// ���� ���¿� ���� ��ȣ�ۿ� ����
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
    /// ���� ���¿� ���� ��ȣ�ۿ� ���� ��ȯ
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
    /// ���� ����� �������� �ֿ�� �õ�.
    /// �κ��丮�� ������ �ִ� ��쿡�� �������� �ֿ�.
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

        if (_inventory.Add(nearestItem.ItemData) == 0)
        {
            nearestItem.Interact();
            _ItemsInScope.Remove(nearestItem);
        }
    }

    /// <summary>
    /// ItemInScope �߿��� �÷��̾�� ���� ������ �ִ� �������� ã�� ��ȯ
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
    /// �÷��̾� -> �ֿ���� ������ �������� Ray�� ���� �� ���̿� ��ֹ��� ������ Ȯ��
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
}
