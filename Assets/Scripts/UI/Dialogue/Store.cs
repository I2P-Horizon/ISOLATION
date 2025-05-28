using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : NPC
{
    [SerializeField] private DialogueDataSO dialogueData;               // 상점 NPC 대화 데이터
    [SerializeField] private GameObject storeUI;                        // 상점 UI

    private void Start()
    {
        npcUI = storeUI;
    }

    private void Update()
    {
        // 대화 시작
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.G) && !dialogueUI.activeSelf)
        {
            DialogueManager.Instance.StartDialogue(dialogueData, this);
        }
    }  
}
