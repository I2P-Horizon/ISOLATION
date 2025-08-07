using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryPopupUI : MonoBehaviour
{
    [Header("# Ȯ�� �˾�â")]
    [SerializeField] private GameObject confirmationPopupUI;    // Ȯ�� �˾�â
    [SerializeField] private Text confirmaitionItemNameText;    // ������ �̸� �ؽ�Ʈ
    [SerializeField] private Button confirmationOkButton;       // Ȯ�ι�ư
    [SerializeField] private Button confirmationCancelButton;   // ��ҹ�ư

    // Ȯ�� ��ư�� ������ �� ����� �̺�Ʈ
    private event Action OnConfirmationOK;

    private void Awake()
    {
        InitUIEvents();
        HidePanel();
        HideConfirmationPopupUI();
    }

    private void InitUIEvents()
    {
        // Ȯ�� ��ư�� ������ �� �̺�Ʈ �߰�
        confirmationOkButton.onClick.AddListener(HidePanel);
        confirmationOkButton.onClick.AddListener(HideConfirmationPopupUI);
        confirmationOkButton.onClick.AddListener(() => OnConfirmationOK?.Invoke());

        // ��� ��ư�� ������ �� �̺�Ʈ �߰�
        confirmationCancelButton.onClick.AddListener(HidePanel);
        confirmationCancelButton.onClick.AddListener(HideConfirmationPopupUI);
    }

    // �ǳ� Ȱ��ȭ(Raycast Target Ȱ��ȭ)
    private void ShowPanel() => gameObject.SetActive(true);
    // �ǳ� ��Ȱ��ȭ
    private void HidePanel() => gameObject.SetActive(false);


    // Ȯ�� �˾� Ȱ��ȭ
    private void ShowConfirmationPopupUI(string itemName)
    {
        confirmaitionItemNameText.text = itemName;
        confirmationPopupUI.SetActive(true);
    }

    // Ȯ�� �˾� ��Ȱ��ȭ
    private void HideConfirmationPopupUI() => confirmationPopupUI.SetActive(false);

    // �̺�Ʈ ����
    private void SetConfirmationOkEvent(Action handler) => OnConfirmationOK = handler;

    // ������Ȯ�� �˾� ����
    public void OpenConfirmationPopupUI(Action okCallback, string itemName)
    {
        ShowPanel();
        ShowConfirmationPopupUI(itemName);
        SetConfirmationOkEvent(okCallback);
    }
   
}
