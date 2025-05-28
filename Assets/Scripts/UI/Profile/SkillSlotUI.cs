using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
                        SkillSlotUI
          
            - 플레이어 스킬슬롯의 스킬설정 및 스킬사용
             
*/

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private string skillId;                 // 슬롯의 스킬 ID
    [SerializeField] private Image skillImage;               // 스킬 이미지 아이콘
    [SerializeField] private Image highlightImage;           // 강조 이미지

    private Skill skill;
    private float cooldownTime;
    private bool isCooldown;

    private float maxHighlightAlpha = 0.5f;             // 하이라이트 이미지 최대 알파값
    private float currentHighlightAlpha = 0f;           // 현재 하이라이트 이미지 알파값
    private float highlightFadeDuration = 0.2f;         // 하이라이트 소요 시간

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

    // 스킬 사용
    public void UseSkill()
    {
        // 쿨타임 아닐때
        if(!isCooldown)
        {
            // 스킬사용 성공여부에따라 쿨타임적용
            SkillManager.Instance.ExecuteSkill(skillId, GameManager.Instance.player.gameObject, successed =>
            {
                if (successed)
                    StartCoroutine(Cooldown());
            });
        }  
    }
    
    // 스킬 쿨타임 표시
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

    // 슬롯 강조효과
    private IEnumerator Highlight()
    {
        // 하이라이트 이미지 활성화
        highlightImage.gameObject.SetActive(true);

        float timer = maxHighlightAlpha / highlightFadeDuration;

        // 하이라이트 이미지 알파값을 서서히 증가시키기
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
