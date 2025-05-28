using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary> �巡�� �� ��� UI �̵�/// </summary>
public class MovableHeaderUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private Transform targetUI;    // �̵��� UI

    private Vector2 beginPoint;
    private Vector2 moveBegin;


    private void Awake()
    {
        // ������ Ÿ��UI�� ������� �θ�� �ʱ�ȭ
        if (targetUI == null)
            targetUI = transform.parent;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        targetUI.position = beginPoint + (eventData.position - moveBegin);
    }


    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        beginPoint = targetUI.position;
        moveBegin = eventData.position;
    }

    // Ÿ�� UI ��Ȱ��ȭ
    public void HideUI()
    {
        UIManager.Instance.CloseUI(targetUI.gameObject);
    }
}