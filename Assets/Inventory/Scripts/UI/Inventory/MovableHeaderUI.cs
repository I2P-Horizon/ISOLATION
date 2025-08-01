using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary> 드래그 앤 드롭 UI 이동/// </summary>
public class MovableHeaderUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private Transform targetUI;    // 이동될 UI

    private Vector2 beginPoint;
    private Vector2 moveBegin;


    private void Awake()
    {
        // 지정된 타깃UI가 없을경우 부모로 초기화
        if (targetUI == null)
            targetUI = transform.parent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HideUI();
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


    public void HideUI()
    {
        targetUI.gameObject.SetActive(false);
    }
}