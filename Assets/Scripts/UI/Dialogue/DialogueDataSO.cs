using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Datas/DialogueData")]
public class DialogueDataSO : ScriptableObject
{
    public string npcName;              // NPC 이름
    [TextArea(2, 10)]
    public string[] sentences;          // 대화 내용
}
