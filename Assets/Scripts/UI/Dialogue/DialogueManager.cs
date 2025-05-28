using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
                    DialogueManager
        
        - �Ѱܹ��� ��ȭ �����͸� ������ ��ȭ�� ����

        - NPC�� ��ȭ�� "--"�� �������� ������ ������������ ���� ��µǵ�����
        
        - ��ȭ�� Ÿ���� ȿ��   
 */

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialogueUI;             // ��ȭâ ������Ʈ
    [SerializeField] private TMP_Text npcNameText;              // NPC �̸� �ؽ�Ʈ
    [SerializeField] private TMP_Text dialogueText;             // ��ȭ �ؽ�Ʈ

    private Queue<string> pages = new Queue<string>();
    private bool isTypipng = false;                             // ��ȭ Ÿ���� ȿ��
    private float typingSpeed = 0.05f;                          // Ÿ���� �ӵ�

    [HideInInspector]
    public NPC npc;                                             // ���� ��� NPC
    [HideInInspector]                                           // ��ȭ�ϱ� ���ɿ���(�ߺ���ȭ����)
    public bool isReadyToTalk = true;

    private void Update()
    {
        if(dialogueUI.activeSelf && npc != null && Input.GetKeyDown(KeyCode.G))
        {
            DisplayNextPage(npc);
        }
    }

    // ��ȭ����
    public void StartDialogue(DialogueDataSO dialogue, NPC npcData)
    {
        if (!isReadyToTalk)
            return;

        isReadyToTalk = false;

        npc = npcData;

        dialogueUI.SetActive(true);                     // ��ȭâ UI Ȱ��ȭ

        npcNameText.text = dialogue.npcName;
        pages.Clear();                                  // ���� ��ȭ ���� �ʱ�ȭ

        foreach(string sentence in dialogue.sentences)
        {
            SplitSentenceToPages(sentence);
        }

        DisplayNextPage(npc);
    }

    // ���������� ��ȭ���� ���
    public void DisplayNextPage(NPC npc)
    {
        if (isTypipng) return;

        // ���̻� ����� ������ ������
        if(pages.Count == 0)
        {
            EndDialogue();                      // ��ȭâ ��Ȱ��ȭ 
            npc.SetActiveNpcUI();               // ���� ��ȭ���� NPC�� UI ���
            return;
        }

        string page = pages.Dequeue();
        StopAllCoroutines();
        
        StartCoroutine(TypePage(page));
    }

    // ��ȣ "--"�� �������� ��ȭ ������ ������
    private void SplitSentenceToPages(string sentence)
    {
        string[] pagesArray = sentence.Split(new string[] { "--" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string page in pagesArray)
        {
            pages.Enqueue(page.Trim());         // ���� ���� �� ť�� ����
        }
    }
 
    // Ÿ���� ȿ��
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

    // ��ȭâ ��Ȱ��ȭ
    private void EndDialogue()
    {
        dialogueUI.SetActive(false);
    }
}
