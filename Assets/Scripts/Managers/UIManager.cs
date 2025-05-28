using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                    UIManager 
        
        - UI 활성화
        - Escape 키 : 가장 최근에 열린 UI 비활성화(Stack 활용)
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
            // 스택에서 해당 UI 제거
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

    // UI 활성/비활성화 토글
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

    // 가장 최근에 활성화된 UI 비활성화
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
