using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                    UIManager 
        
        - UI Ȱ��ȭ
        - Escape Ű : ���� �ֱٿ� ���� UI ��Ȱ��ȭ(Stack Ȱ��)
 */
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject menuUI;

    private Stack<GameObject> uiStack = new();
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(IsAnyUIOpen())
            {
                CloseTopUI();
            }
            else
            {
                ToggleMenuUI();
            }
        }
    }

    public void OpenUI(GameObject ui)
    {
        if (ui.activeSelf) return;

        ui.SetActive(true);
        uiStack.Push(ui);
    }

    public void CloseUI(GameObject ui)
    {
        if (!ui.activeSelf) return;

        ui.SetActive(false);
        if(uiStack.Contains(ui))
        {
            // ���ÿ��� �ش� UI ����
            Stack<GameObject> tempStack = new();
            while(uiStack.Count > 0)
            {
                GameObject top = uiStack.Pop();
                if (top == ui) break;
                tempStack.Push(top);
            }
            while(tempStack.Count>0)
            {
                uiStack.Push(tempStack.Pop());
            }
        }
    }

    // UI Ȱ��/��Ȱ��ȭ ���
    public void ToggleUI(GameObject ui)
    {
        if(ui.activeSelf)
        {
            CloseUI(ui);
        }
        else
        {
            OpenUI(ui);
        }
    }

    // ���� �ֱٿ� Ȱ��ȭ�� UI ��Ȱ��ȭ
    public void CloseTopUI()
    {
        if (uiStack.Count == 0) return;

        GameObject topUI = uiStack.Pop();
        topUI.SetActive(false);
    }

    public bool IsAnyUIOpen()
    {
        return uiStack.Count > 0;
    }

    private void ToggleMenuUI()
    {
        menuUI.SetActive(!menuUI.activeSelf);
    }
}
