using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UIRaycaster : MonoBehaviour
{
    private GraphicRaycaster gr;
    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();
        ped = new PointerEventData(EventSystem.current);
        rrList = new List<RaycastResult>();
    }

    private void Update()
    {
        ped.position = Input.mousePosition;
    }

    // T 컴포넌트를 가지고있는 첫번째 오브젝트의 T 반환
    public T RaycastAndgetFirstComponent<T>() where T : Component
    {
        // 리스트 초기화
        rrList.Clear();

        // 현재 마우스 위치에서 감지된 UI요소 저장
        gr.Raycast(ped, rrList);

        // 없으면 null
        if (rrList.Count == 0)
            return null;

        for(int i = 0; i< rrList.Count; i++)
        {
            T component = rrList[i].gameObject.GetComponent<T>();
            if(component != null)
            {
                return component;
            }
        }

        return null;
    }
}
