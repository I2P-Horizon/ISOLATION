using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� ��ȣ�ۿ��� �����ϴ� ��ũ��Ʈ (ä��, ��� ��)
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    enum InteractionState
    {
        None, // �ƹ� �͵� ���� �ʴ� ����
        Gathering, // ä�� ���� ����
        Attacking // ���� ���� ����
    }

    /// <summary>
    /// ��ȣ�ۿ� ������ �Ÿ�
    /// </summary>
    [SerializeField] private float _interactionDistance = 5.0f;
    /// <summary>
    /// ä�� �� ������ ���ҷ�
    /// </summary>
    [SerializeField] private float _gatherStrength = 5.0f;
    /// <summary>
    /// ���� ä�� ����
    /// </summary>
    [SerializeField] private float _gatherInterval = 1.0f;

    private InteractionState _currentState = InteractionState.None; // ���� ����
    private MonoBehaviour _currentTarget = null; // ���� ��ȣ�ۿ� ���� ���

    private float _holdTime = 0; // ��Ŭ�� ���� �ð�

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
    /// ��ȣ�ۿ� ������ ����� �ִ��� Ȯ���ϰ�, ����� ������ �°� ��ȣ�ۿ� ���� ����
    /// </summary>
    private void TryInteraction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ���콺 �������� �� ray�� ��� ���� ����
        if (Physics.Raycast(ray, out RaycastHit mouseHitInfo, 100f))
        {
            // �÷��̾� ��ġ���� ���콺�� ����Ű�� world point �������� ray ���
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
    /// ���� ���¿� ���� ������ ��ȣ�ۿ� ����
    /// </summary>
    private void ProcessInteraction()
    {
        switch (_currentState)
        {
            case InteractionState.Gathering:
                Gathering();
                break;
            case InteractionState.Attacking:
                // ���� �Լ� ȣ��
                break;
        }
    }

    /// <summary>
    /// ���� �������� ��ȣ�ۿ��� �����Ŵ
    /// </summary>
    private void StopInteraction()
    {
        _currentState = InteractionState.None;
        _holdTime = 0;
    }

    /// <summary>
    /// ä�� ���� �Լ�
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
