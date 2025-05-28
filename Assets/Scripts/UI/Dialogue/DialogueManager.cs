using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
                    DialogueManager
        
        - 넘겨받은 대화 데이터를 가지고 대화를 시작

        - NPC의 대화를 "--"를 기준으로 나누어 여러페이지에 걸쳐 출력되도록함
        
        - 대화시 타이핑 효과   
 */

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialogueUI;             // 대화창 오브젝트
    [SerializeField] private TMP_Text npcNameText;              // NPC 이름 텍스트
    [SerializeField] private TMP_Text dialogueText;             // 대화 텍스트

    private Queue<string> pages = new Queue<string>();
    private bool isTypipng = false;                             // 대화 타이핑 효과
    private float typingSpeed = 0.05f;                          // 타이핑 속도

    [HideInInspector]
    public NPC npc;                                             // 현재 대상 NPC
    [HideInInspector]                                           // 대화하기 가능여부(중복대화방지)
    public bool isReadyToTalk = true;

    private void Update()
    {
        if(dialogueUI.activeSelf && npc != null && Input.GetKeyDown(KeyCode.G))
        {
            DisplayNextPage(npc);
        }
    }

    // 대화시작
    public void StartDialogue(DialogueDataSO dialogue, NPC npcData)
    {
        if (!isReadyToTalk)
            return;

        isReadyToTalk = false;

        npc = npcData;

        dialogueUI.SetActive(true);                     // 대화창 UI 활성화

        npcNameText.text = dialogue.npcName;
        pages.Clear();                                  // 이전 대화 내용 초기화

        foreach(string sentence in dialogue.sentences)
        {
            SplitSentenceToPages(sentence);
        }

        DisplayNextPage(npc);
    }

    // 다음페이지 대화내용 출력
    public void DisplayNextPage(NPC npc)
    {
        if (isTypipng) return;

        // 더이상 출력할 내용이 없으면
        if(pages.Count == 0)
        {
            EndDialogue();                      // 대화창 비활성화 
            npc.SetActiveNpcUI();               // 현재 대화중인 NPC의 UI 출력
            return;
        }

        string page = pages.Dequeue();
        StopAllCoroutines();
        
        StartCoroutine(TypePage(page));
    }

    // 기호 "--"를 기준으로 대화 페이지 나누기
    private void SplitSentenceToPages(string sentence)
    {
        string[] pagesArray = sentence.Split(new string[] { "--" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string page in pagesArray)
        {
            pages.Enqueue(page.Trim());         // 공백 제거 후 큐에 저장
        }
    }
 
    // 타이핑 효과
    private IEnumerator TypePage(string page)
    {
        isTypipng = true;
        dialogueText.text = "";

        AudioManager.Instance.PlaySFX("DialogueEffect");

        foreach (char letter in page.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypipng = false;
        AudioManager.Instance.StopSFX("DialogueEffect");

    }

    // 대화창 비활성화
    private void EndDialogue()
    {
        dialogueUI.SetActive(false);
    }
}
