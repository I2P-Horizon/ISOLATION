using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        Skill
          
            - 스킬이 공통적으로 가지는 데이터 관리
            
            - InitAnimator() : 스킬사용주체의 애니메이터 캐싱
            
            - SetEffect : 이펙트 프리팹 캐싱

            - Activate() : 스킬 사용
                - 개별 스킬클래스에서 구현
*/
public abstract class Skill
{
    protected SkillData data;                       
    protected Animator anim;                        
    protected GameObject effectPrefab;               

    public GameObject cachedEffect;                 // 캐싱된 이펙트 오브젝트

    public Skill(SkillData data)
    {
        this.data = data;
    }

    // 애니메이션 캐싱
    public void InitAnimator(GameObject user)
    {
        anim = user.GetComponent<Animator>();
    }

    // 이펙트 프리팹 저장
    public void SetEffect(GameObject effect)
    {
        effectPrefab = effect;
    }

    // 스킬 사용
    public abstract bool Activate(GameObject user);
}