using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
                        SkillSlotUI
          
            - �÷��̾� ��ų������ ��ų���� �� ��ų���
             
*/

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private string skillId;                 // ������ ��ų ID
    [SerializeField] private Image skillImage;               // ��ų �̹��� ������
    [SerializeField] private Image highlightImage;           // ���� �̹���

    private Skill skill;
    private float cooldownTime;
    private bool isCooldown;

    private float maxHighlightAlpha = 0.5f;             // ���̶���Ʈ �̹��� �ִ� ���İ�
    private float currentHighlightAlpha = 0f;           // ���� ���̶���Ʈ �̹��� ���İ�
    private float highlightFadeDuration = 0.2f;         // ���̶���Ʈ �ҿ� �ð�

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        SkillData skillData = SkillManager.Instance.GetSkillDataById(skillId);
        skill = SkillManager.Instance.CreateSkillInstance(skillData);
        skill.InitAnimator(GameManager.Instance.player.gameObject);

        cooldownTime = skillData.Cooldown;
    }

    // ��ų ���
    public void UseSkill()
    {
        // ��Ÿ�� �ƴҶ�
        if(!isCooldown)
        {
            // ��ų��� �������ο����� ��Ÿ������
            SkillManager.Instance.ExecuteSkill(skillId, GameManager.Instance.player.gameObject, successed =>
            {
                if (successed)
                    StartCoroutine(Cooldown());
            });
        }  
    }
    
    // ��ų ��Ÿ�� ǥ��
    private IEnumerator Cooldown()
    {
        isCooldown = true;
        float timer = 0f;

        while(timer < cooldownTime)
        {
            timer += Time.deltaTime;
            skillImage.fillAmount = 1f - (timer / cooldownTime);
            yield return null;
        }

        skillImage.fillAmount = 1f;
        isCooldown = false;
        StartCoroutine(Highlight());
    }

    // ���� ����ȿ��
    private IEnumerator Highlight()
    {
        // ���̶���Ʈ �̹��� Ȱ��ȭ
        highlightImage.gameObject.SetActive(true);

        float timer = maxHighlightAlpha / highlightFadeDuration;

        // ���̶���Ʈ �̹��� ���İ��� ������ ������Ű��
        for (; currentHighlightAlpha <= maxHighlightAlpha; currentHighlightAlpha += timer * Time.deltaTime)
        {
            highlightImage.color = new Color(
                     highlightImage.color.r,
                     highlightImage.color.g,
                     highlightImage.color.b,
                     currentHighlightAlpha
                );

            yield return null;
        }

        highlightImage.gameObject.SetActive(false);
    }
}
