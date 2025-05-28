using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

/// <summary>
///                             SkillManager 
///                     1. 스킬 데이터 로드
///                     2. 스킬 실행 
///                     3. 스킬 오브젝트 풀링
/// </summary>

public class SkillManager : Singleton<SkillManager>
{
    public List<Skill> playerSkills = new();

    private Dictionary<string, SkillData> skillDataDictionary = new();  // 스킬 데이터 캐싱 딕셔너리
    private Dictionary<string, Queue<Skill>> skillPool = new();         // 스킬 풀링 딕셔너리

    protected override void Awake()
    {
        base.Awake();
        LoadSkillData();
    }

    
    // 스킬 데이터 불러오기(캐싱)
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
                Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            }
        }
        else
        {
            Debug.LogWarning("SkillData.json 파일이 없음.");
        }
    }

    // 스킬 인스턴스 생성
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
                Debug.Log($"정의되지 않은 스킬 ID : {data.Id}");
                return null;
        }
    }

    // ID로 스킬 데이터 가져오기
    public SkillData GetSkillDataById(string id)
    {
        if(skillDataDictionary != null && skillDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID에 해당하는 스킬데이터가 없음.");
            return null;
        }
    }

    // 풀에서 스킬 가져오기
    private Skill GetSkillFromPool(SkillData data)
    {
        // 풀에 스킬이 있으면 반환
        if(skillPool.TryGetValue(data.Id, out var queue) && queue.Count > 0)
        {
            return queue.Dequeue();
        }

        // 없으면 스킬 인스턴스 생성후 반환
        return CreateSkillInstance(data);
    }

    //  스킬 사용 후 풀에넣기
    private void ReturnSkillToPool(string skillId, Skill skill)
    {
        // 새로운 스킬이면 
        if(!skillPool.ContainsKey(skillId))
        {
            skillPool[skillId] = new Queue<Skill>();
        }

        skillPool[skillId].Enqueue(skill);
    }

    // 스킬 실행
    public void ExecuteSkill(string skillId, GameObject user, Action<bool> onSkillExecuted)
    {
        var skillData = GetSkillDataById(skillId);
        if (skillData == null)
        {
            onSkillExecuted?.Invoke(false);
            return;
        }

        // 스킬 인스턴스 생성
        Skill skill = GetSkillFromPool(skillData);

        // 이펙트를 미리 로딩 하여
        ResourceManager.Instance.LoadEffectPrefab(skillData.EffectName, prefab =>
        {
            // 스킬 세팅(애니메이터, 이펙트)후 발동
            if (prefab != null)
            {
                skill.SetEffect(prefab);                        // 이펙트 설정
                skill.InitAnimator(user.gameObject);            // 애니메이터 캐싱
                bool successed = skill.Activate(user);          // 스킬 실행 성공여부

                if (successed)
                {
                    StartCoroutine(ReturnSkillAfterUse(skillId, skill, skillData.Cooldown));
                }

                onSkillExecuted?.Invoke(successed);             // 성공여부 콜백
            }
            else
            {
                Debug.Log($"Failed to load prefab for item : {prefab}");
                onSkillExecuted?.Invoke(false);                 // 실패 콜백
            }
        });
    }

    // 스킬 사용 후
    private IEnumerator ReturnSkillAfterUse(string skillId, Skill skill, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnSkillToPool(skillId, skill);              // 풀에 넣어놓기
        if (!(skill is IBuffSkill))
            skill.cachedEffect.SetActive(false);
    }
}
