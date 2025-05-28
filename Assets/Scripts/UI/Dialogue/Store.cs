using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : NPC
{
    [SerializeField] private DialogueDataSO dialogueData;               // ���� NPC ��ȭ ������
    [SerializeField] private GameObject storeUI;                        // ���� UI

    private void Start()
    {
        npcUI = storeUI;
    }

    private void Update()
    {
        // ��ȭ ����
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.G) && !dialogueUI.activeSelf)
        {
            DialogueManager.Instance.StartDialogue(dialogueData, this);
        }
    }  
}
