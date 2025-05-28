using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        Skill
          
            - ��ų�� ���������� ������ ������ ����
            
            - InitAnimator() : ��ų�����ü�� �ִϸ����� ĳ��
            
            - SetEffect : ����Ʈ ������ ĳ��

            - Activate() : ��ų ���
                - ���� ��ųŬ�������� ����
*/
public abstract class Skill
{
    protected SkillData data;                       
    protected Animator anim;                        
    protected GameObject effectPrefab;               

    public GameObject cachedEffect;                 // ĳ�̵� ����Ʈ ������Ʈ

    public Skill(SkillData data)
    {
        this.data = data;
    }

    // �ִϸ��̼� ĳ��
    public void InitAnimator(GameObject user)
    {
        anim = user.GetComponent<Animator>();
    }

    // ����Ʈ ������ ����
    public void SetEffect(GameObject effect)
    {
        effectPrefab = effect;
    }

    // ��ų ���
    public abstract bool Activate(GameObject user);
}