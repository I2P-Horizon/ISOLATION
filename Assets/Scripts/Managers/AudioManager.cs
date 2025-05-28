using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class BGMData
{
    public string id;
    public string clipName;
    public float volume;
    public bool loop;
}

[System.Serializable]
public class BGMList
{
    public List<BGMData> bgmList;
}

[System.Serializable]
public class SFXData
{
    public string id;
    public string clipName;
    public float volume;
    public bool loop;
}

[System.Serializable]
public class SFXList
{
    public List<SFXData> sfxList;
}


public class AudioManager : Singleton<AudioManager>
{
    #region ** BGM Settings **
    private AudioSource bgmSource;
    #endregion

    #region ** SFX Settings **
    private List<AudioSource> sfxSource;
    private int currentSFXIndex = 0;
    private int sfxPoolSize = 10;                                       // SFX 풀 사이즈
    #endregion

    private Dictionary<string, BGMData> bgmDictionary;                  // BGM 캐싱 데이터
    private Dictionary<string, SFXData> sfxDictionary;                  // SFX 캐싱 데이터
    private Dictionary<string, List<AudioSource>> loopedSFXMap = new(); // 반복재생되는 SFX를 추적하기위한 딕셔너리

    protected override void Awake()
    {
        LoadBGMData();
        LoadSFXData();
        InitAudioSources();
    }

    // 초기화
    private void InitAudioSources()
    {
        // BGM 
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmSource = bgmObject.AddComponent<AudioSource>();

        // SFX
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxSource = new List<AudioSource>();

        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = sfxObject.AddComponent<AudioSource>();
            sfxSource.Add(source);
        }

    }

    // BGM 데이터 불러오기
    private void LoadBGMData()
    {
        string path = Path.Combine(Application.persistentDataPath, "bgmData.json");

        if(File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            BGMList bgmList = JsonUtility.FromJson<BGMList>(jsonData);

            bgmDictionary = new Dictionary<string, BGMData>();
            foreach(var bgm in bgmList.bgmList)
            {
                bgmDictionary[bgm.id] = bgm;
            }
        }
        else
        {
            Debug.LogWarning("bgmData.json 파일이 없음.");
        }
    }

    // BGM 데이터 불러오기
    private void LoadSFXData()
    {
        string path = Path.Combine(Application.persistentDataPath, "sfxData.json");

        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            SFXList list = JsonUtility.FromJson<SFXList>(jsonData);

            sfxDictionary = new Dictionary<string, SFXData>();
            foreach (var sfx in list.sfxList)
            {
                sfxDictionary[sfx.id] = sfx;
            }
        }
        else
        {
            Debug.LogWarning("sfxData.json 파일이 없음.");
        }
    }

    
    // BGM 실행(ID)
    public void PlayBGM(string bgmId)
    {
        // 이전 BGM 정지
        if(bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        // BGM ID로 BGM 실행
        if(bgmDictionary.TryGetValue(bgmId, out BGMData bgmData))
        {
            ResourceManager.Instance.LoadSound(bgmId, bgm =>
            {
                // 성공
                if (bgm != null)
                {
                    bgmSource.clip = bgm;
                    bgmSource.volume = bgmData.volume;
                    bgmSource.loop = bgmData.loop;
                    bgmSource.Play();
                }
                else
                {
                    Debug.Log($"오디오 클립을 찾을 수 없음 : {bgmData.clipName}");
                }
            });
        }
        else
        {
            Debug.Log($"BGM ID를 찾을 수 없음 : {bgmId}");
        }
    }

    // SFX 실행(ID)
    public void PlaySFX(string sfxId, float delay = 0f)
    {
        // BGM ID로 BGM 실행
        if (sfxDictionary.TryGetValue(sfxId, out SFXData sfxData))
        {
            ResourceManager.Instance.LoadSound(sfxId, sfx =>
            {
                // 성공
                if (sfx != null)
                {
                    for(int index = 0; index < sfxPoolSize;index++)
                    {
                        int loopIndex = (index + currentSFXIndex) % sfxPoolSize;

                        if (sfxSource[loopIndex].isPlaying)
                            continue;

                        currentSFXIndex = loopIndex;
                        sfxSource[currentSFXIndex].clip = sfx;
                        sfxSource[currentSFXIndex].volume = sfxData.volume;
                        sfxSource[currentSFXIndex].loop = sfxData.loop;
                        sfxSource[currentSFXIndex].PlayDelayed(delay);

                        // 반복재생되는 SFX 는 추적대상
                        if(sfxSource[currentSFXIndex].loop)
                        {
                            // 추적 등록
                            if (!loopedSFXMap.ContainsKey(sfxId))
                                loopedSFXMap[sfxId] = new List<AudioSource>();

                            loopedSFXMap[sfxId].Add(sfxSource[currentSFXIndex]);
                        }
                        else
                        {
                            StartCoroutine(CleanupClipAfterPlay(sfxSource[currentSFXIndex]));
                        }

                        break;
                    }
                }
                else
                {
                    Debug.Log($"오디오 클립을 찾을 수 없음 : {sfxData.clipName}");
                }
            });
        }
        else
        {
            Debug.Log($"BGM ID를 찾을 수 없음 : {sfxId}");
        }
    }

    // SFX 중지(ID)
    public void StopSFX(string sfxId)
    {
        // ID로 중지할 SFX 탐색
        if(loopedSFXMap.TryGetValue(sfxId, out var sources))
        {
            // ID 키로 추적된 모든 AudioSource 중지
            foreach(var src in sources)
            {
                if (src.isPlaying)
                    src.Stop();
                src.clip = null;
            }
            
            // 추적 제거
            loopedSFXMap.Remove(sfxId);
        }
    }

    // 사운드 출력 후 AudioSource 정리
    private IEnumerator CleanupClipAfterPlay(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);

        if(!source.loop)
        {
            source.clip = null;
        }
    }
}
