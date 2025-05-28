using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

/// <summary>
///                             SkillManager 
///                     1. ��ų ������ �ε�
///                     2. ��ų ���� 
///                     3. ��ų ������Ʈ Ǯ��
/// </summary>

public class SkillManager : Singleton<SkillManager>
{
    public List<Skill> playerSkills = new();

    private Dictionary<string, SkillData> skillDataDictionary = new();  // ��ų ������ ĳ�� ��ųʸ�
    private Dictionary<string, Queue<Skill>> skillPool = new();         // ��ų Ǯ�� ��ųʸ�

    protected override void Awake()
    {
        base.Awake();
        LoadSkillData();
    }

    
    // ��ų ������ �ҷ�����(ĳ��)
    private void LoadSkillData()
    {
        string path = Path.Combine(Application.persistentDataPath, "SkillData.json");

        if(File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            var skillDict = JsonConvert.DeserializeObject<Dictionary<string, List<SkillDataDTO>>>(jsonData);

            if(skillDict != null)
            {
                foreach(var category in skillDict)
                {
                    foreach(var skillDTO in category.Value)
                    {
                        SkillData skillData = new SkillData(skillDTO);
                        skillDataDictionary[skillData.Id] = skillData;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Json �����͸� �Ľ��� �� ����.");
            }
        }
        else
        {
            Debug.LogWarning("SkillData.json ������ ����.");
        }
    }

    // ��ų �ν��Ͻ� ����
    public Skill CreateSkillInstance(SkillData data)
    {
        switch (data.Id)
        {
            case "buff":
                return new Buff(data);
            case "iceshot":
                return new IceShot(data);
            case "slash":
                return new Slash(data);
            default:
                Debug.Log($"���ǵ��� ���� ��ų ID : {data.Id}");
                return null;
        }
    }

    // ID�� ��ų ������ ��������
    public SkillData GetSkillDataById(string id)
    {
        if(skillDataDictionary != null && skillDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID�� �ش��ϴ� ��ų�����Ͱ� ����.");
            return null;
        }
    }

    // Ǯ���� ��ų ��������
    private Skill GetSkillFromPool(SkillData data)
    {
        // Ǯ�� ��ų�� ������ ��ȯ
        if(skillPool.TryGetValue(data.Id, out var queue) && queue.Count > 0)
        {
            return queue.Dequeue();
        }

        // ������ ��ų �ν��Ͻ� ������ ��ȯ
        return CreateSkillInstance(data);
    }

    //  ��ų ��� �� Ǯ���ֱ�
    private void ReturnSkillToPool(string skillId, Skill skill)
    {
        // ���ο� ��ų�̸� 
        if(!skillPool.ContainsKey(skillId))
        {
            skillPool[skillId] = new Queue<Skill>();
        }

        skillPool[skillId].Enqueue(skill);
    }

    // ��ų ����
    public void ExecuteSkill(string skillId, GameObject user, Action<bool> onSkillExecuted)
    {
        var skillData = GetSkillDataById(skillId);
        if (skillData == null)
        {
            onSkillExecuted?.Invoke(false);
            return;
        }

        // ��ų �ν��Ͻ� ����
        Skill skill = GetSkillFromPool(skillData);

        // ����Ʈ�� �̸� �ε� �Ͽ�
        ResourceManager.Instance.LoadEffectPrefab(skillData.EffectName, prefab =>
        {
            // ��ų ����(�ִϸ�����, ����Ʈ)�� �ߵ�
            if (prefab != null)
            {
                skill.SetEffect(prefab);                        // ����Ʈ ����
                skill.InitAnimator(user.gameObject);            // �ִϸ����� ĳ��
                bool successed = skill.Activate(user);          // ��ų ���� ��������

                if (successed)
                {
                    StartCoroutine(ReturnSkillAfterUse(skillId, skill, skillData.Cooldown));
                }

                onSkillExecuted?.Invoke(successed);             // �������� �ݹ�
            }
            else
            {
                Debug.Log($"Failed to load prefab for item : {prefab}");
                onSkillExecuted?.Invoke(false);                 // ���� �ݹ�
            }
        });
    }

    // ��ų ��� ��
    private IEnumerator ReturnSkillAfterUse(string skillId, Skill skill, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnSkillToPool(skillId, skill);              // Ǯ�� �־����
        if (!(skill is IBuffSkill))
            skill.cachedEffect.SetActive(false);
    }
}
