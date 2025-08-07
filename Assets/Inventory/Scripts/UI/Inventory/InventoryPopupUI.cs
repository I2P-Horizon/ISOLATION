using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryPopupUI : MonoBehaviour
{
    [Header("# 확인 팝업창")]
    [SerializeField] private GameObject confirmationPopupUI;    // 확인 팝업창
    [SerializeField] private Text confirmaitionItemNameText;    // 아이템 이름 텍스트
    [SerializeField] private Button confirmationOkButton;       // 확인버튼
    [SerializeField] private Button confirmationCancelButton;   // 취소버튼

    // 확인 버튼을 눌렀을 때 실행될 이벤트
    private event Action OnConfirmationOK;

    private void Awake()
    {
        InitUIEvents();
        HidePanel();
        HideConfirmationPopupUI();
    }

    private void InitUIEvents()
    {
        // 확인 버튼을 눌렀을 때 이벤트 추가
        confirmationOkButton.onClick.AddListener(HidePanel);
        confirmationOkButton.onClick.AddListener(HideConfirmationPopupUI);
        confirmationOkButton.onClick.AddListener(() => OnConfirmationOK?.Invoke());

        // 취소 버튼을 눌렀을 때 이벤트 추가
        confirmationCancelButton.onClick.AddListener(HidePanel);
        confirmationCancelButton.onClick.AddListener(HideConfirmationPopupUI);
    }

    // 판넬 활성화(Raycast Target 활성화)
    private void ShowPanel() => gameObject.SetActive(true);
    // 판넬 비활성화
    private void HidePanel() => gameObject.SetActive(false);


    // 확인 팝업 활성화
    private void ShowConfirmationPopupUI(string itemName)
    {
        confirmaitionItemNameText.text = itemName;
        confirmationPopupUI.SetActive(true);
    }

    // 확인 팝업 비활성화
    private void HideConfirmationPopupUI() => confirmationPopupUI.SetActive(false);

    // 이벤트 설정
    private void SetConfirmationOkEvent(Action handler) => OnConfirmationOK = handler;

    // 버리기확인 팝업 띄우기
    public void OpenConfirmationPopupUI(Action okCallback, string itemName)
    {
        ShowPanel();
        ShowConfirmationPopupUI(itemName);
        SetConfirmationOkEvent(okCallback);
    }
   
}
