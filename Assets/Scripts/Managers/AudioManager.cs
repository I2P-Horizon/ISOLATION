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
    private int sfxPoolSize = 10;                                       // SFX Ǯ ������
    #endregion

    private Dictionary<string, BGMData> bgmDictionary;                  // BGM ĳ�� ������
    private Dictionary<string, SFXData> sfxDictionary;                  // SFX ĳ�� ������
    private Dictionary<string, List<AudioSource>> loopedSFXMap = new(); // �ݺ�����Ǵ� SFX�� �����ϱ����� ��ųʸ�

    protected override void Awake()
    {
        LoadBGMData();
        LoadSFXData();
        InitAudioSources();
    }

    // �ʱ�ȭ
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

    // BGM ������ �ҷ�����
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
            Debug.LogWarning("bgmData.json ������ ����.");
        }
    }

    // BGM ������ �ҷ�����
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
            Debug.LogWarning("sfxData.json ������ ����.");
        }
    }

    
    // BGM ����(ID)
    public void PlayBGM(string bgmId)
    {
        // ���� BGM ����
        if(bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        // BGM ID�� BGM ����
        if(bgmDictionary.TryGetValue(bgmId, out BGMData bgmData))
        {
            ResourceManager.Instance.LoadSound(bgmId, bgm =>
            {
                // ����
                if (bgm != null)
                {
                    bgmSource.clip = bgm;
                    bgmSource.volume = bgmData.volume;
                    bgmSource.loop = bgmData.loop;
                    bgmSource.Play();
                }
                else
                {
                    Debug.Log($"����� Ŭ���� ã�� �� ���� : {bgmData.clipName}");
                }
            });
        }
        else
        {
            Debug.Log($"BGM ID�� ã�� �� ���� : {bgmId}");
        }
    }

    // SFX ����(ID)
    public void PlaySFX(string sfxId, float delay = 0f)
    {
        // BGM ID�� BGM ����
        if (sfxDictionary.TryGetValue(sfxId, out SFXData sfxData))
        {
            ResourceManager.Instance.LoadSound(sfxId, sfx =>
            {
                // ����
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

                        // �ݺ�����Ǵ� SFX �� �������
                        if(sfxSource[currentSFXIndex].loop)
                        {
                            // ���� ���
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
                    Debug.Log($"����� Ŭ���� ã�� �� ���� : {sfxData.clipName}");
                }
            });
        }
        else
        {
            Debug.Log($"BGM ID�� ã�� �� ���� : {sfxId}");
        }
    }

    // SFX ����(ID)
    public void StopSFX(string sfxId)
    {
        // ID�� ������ SFX Ž��
        if(loopedSFXMap.TryGetValue(sfxId, out var sources))
        {
            // ID Ű�� ������ ��� AudioSource ����
            foreach(var src in sources)
            {
                if (src.isPlaying)
                    src.Stop();
                src.clip = null;
            }
            
            // ���� ����
            loopedSFXMap.Remove(sfxId);
        }
    }

    // ���� ��� �� AudioSource ����
    private IEnumerator CleanupClipAfterPlay(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);

        if(!source.loop)
        {
            source.clip = null;
        }
    }
}
