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

    // T ������Ʈ�� �������ִ� ù��° ������Ʈ�� T ��ȯ
    public T RaycastAndgetFirstComponent<T>() where T : Component
    {
        // ����Ʈ �ʱ�ȭ
        rrList.Clear();

        // ���� ���콺 ��ġ���� ������ UI��� ����
        gr.Raycast(ped, rrList);

        // ������ null
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
