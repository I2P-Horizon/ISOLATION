using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                    NPC

        - �÷��̾�� ��ȣ�ۿ�(GŰ)
            �� BoxCollider

        - DialogueManager ���� �� NPC�� ��ȭ�� ������ �� �ֵ��� �����͸� �ѱ�

 */
public class NPC : MonoBehaviour
{
    [SerializeField] protected GameObject dialogueUI;
    
    [HideInInspector]
    public GameObject npcUI;                                     // ���� NPC�� UI

    protected bool isPlayerInRange = false;                      // �÷��̾ �����ȿ� �ִ��� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // NPC UI Ȱ��/��Ȱ��ȭ
    public void SetActiveNpcUI()
    {
        npcUI.SetActive(!npcUI.activeSelf);
        DialogueManager.Instance.isReadyToTalk = !npcUI.activeSelf;
        DialogueManager.Instance.npc = null;
    }
}
