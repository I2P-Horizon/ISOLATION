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
    [SerializeField] private float interactionDistance = 5.0f;
    /// <summary>
    /// ä�� �� ������ ���ҷ�
    /// </summary>
    [SerializeField] private float gatherStrength = 5.0f;
    /// <summary>
    /// ���� ä�� ����
    /// </summary>
    [SerializeField] private float gatherInterval = 1.0f;

    private InteractionState currentState = InteractionState.None; // ���� ����
    private MonoBehaviour currentTarget = null; // ���� ��ȣ�ۿ� ���� ���

    private float holdTime = 0; // ��Ŭ�� ���� �ð�

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
    /// ���� ���¿� ���� ������ ��ȣ�ۿ� ����
    /// </summary>
    private void ProcessInteraction()
    {
        switch (currentState)
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
        currentState = InteractionState.None;
        holdTime = 0;
    }

    /// <summary>
    /// ä�� ���� �Լ�
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
