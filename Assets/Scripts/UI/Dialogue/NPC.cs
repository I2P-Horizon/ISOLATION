using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                    NPC

        - 플레이어와 상호작용(G키)
            ㄴ BoxCollider

        - DialogueManager 에서 이 NPC의 대화를 시작할 수 있도록 데이터를 넘김

 */
public class NPC : MonoBehaviour
{
    [SerializeField] protected GameObject dialogueUI;
    
    [HideInInspector]
    public GameObject npcUI;                                     // 개별 NPC의 UI

    protected bool isPlayerInRange = false;                      // 플레이어가 범위안에 있는지 여부

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

    // NPC UI 활성/비활성화
    public void SetActiveNpcUI()
    {
        npcUI.SetActive(!npcUI.activeSelf);
        DialogueManager.Instance.isReadyToTalk = !npcUI.activeSelf;
        DialogueManager.Instance.npc = null;
    }
}
