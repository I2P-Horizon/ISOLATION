using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
///                             DataManager 
///                     1. 각 데이터 로드 및 세이브
///                     2. 각 데이터에 대한 접근제공
///                     
/// </summary>
public class DataManager : Singleton<DataManager>
{
    private string playerDataPath;          // 플레이어 데이터 저장경로
    private string weaponItemDataPath;      // 무기 데이터 저장경로
    private string portionItemDataPath;     // 포션 데이터 저장경로
    private string armorItemDataPath;       // 방어구 데이터 저장경로

    private Dictionary<int, WeaponItemData> weaponDataDictionary;
    private Dictionary<int, PortionItemData> portionDataDictionary;
    private Dictionary<int, ArmorItemData> armorDataDictionary;
    private PlayerData playerData;

    protected override void Awake()
    {
        base.Awake();
        InitAndLoadData();
    }

    private void Start()
    {
        // 5초마다 자동 저장
        InvokeRepeating("SavePlayerData", 5f, 5f);
    }
    private void InitAndLoadData()
    {
        playerDataPath = Path.Combine(Application.persistentDataPath, "Playerdata.json");
        weaponItemDataPath = Path.Combine(Application.persistentDataPath, "WeaponData.json");
        portionItemDataPath = Path.Combine(Application.persistentDataPath, "PortionData.json");
        armorItemDataPath = Path.Combine(Application.persistentDataPath, "ArmorData.json");

        playerData = LoadPlayerData();
        weaponDataDictionary = LoadWeaponData();
        portionDataDictionary = LoadPortionData();
        armorDataDictionary = LoadArmorData();
    }

    // 데이터 저장
    public void SaveData<T>(T data, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, jsonData);
    }

    // 플레이어 데이터 불러오기
    private PlayerData LoadPlayerData()
    {
        if(File.Exists(playerDataPath))
        {
            string jsonData = File.ReadAllText(playerDataPath);
            var playerDTO = JsonConvert.DeserializeObject<PlayerDataDTO>(jsonData);

            if (playerDTO != null && playerDTO.Status != null && playerDTO.Position != null)
            {
                return new PlayerData(playerDTO);
            }
            else
            {
                Debug.LogWarning("PlayerData.json 파일에 Status 또는 Position 정보가 없음.");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("PlayerData.json 파일이 없음.");
            return null;
        }
    }

    // 무기 데이터 불러오기
    private Dictionary<int, WeaponItemData> LoadWeaponData()
    {
        Dictionary<int, WeaponItemData> dataDictionary = new Dictionary<int, WeaponItemData>();

        if (File.Exists(weaponItemDataPath))
        {
            string jsonData = File.ReadAllText(weaponItemDataPath);
            var weaponDict = JsonConvert.DeserializeObject<Dictionary<string, List<WeaponItemDTO>>>(jsonData);

            // 카테고리별 모든 데이터 저장
            if(weaponDict != null)
            {
                foreach(var category in weaponDict)
                {
                    // DTO를 WeaponItemData로 변환하여 저장
                    foreach (var weaponDTO in category.Value)
                    {
                        WeaponItemData weaponData = new WeaponItemData(weaponDTO);
                        dataDictionary[weaponData.ID] = weaponData;
                    }
                }
                return dataDictionary;
            }
            else
            {
                Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            }
        }
        else
        {
            Debug.LogWarning("WeaponData.json 파일이 없음.");
        }
        return new Dictionary<int, WeaponItemData>();
    }

    // 포션 데이터 불러오기
    private Dictionary<int, PortionItemData> LoadPortionData()
    {
        Dictionary<int, PortionItemData> dataDictionary = new Dictionary<int, PortionItemData>();

        if (File.Exists(portionItemDataPath))
        {
            string jsonData = File.ReadAllText(portionItemDataPath);
            var portionDict = JsonConvert.DeserializeObject<Dictionary<string, List<PortionItemDTO>>>(jsonData);

            // 카테고리별 모든 데이터 저장
            if (portionDict !=  null)
            {
                foreach (var category in portionDict)
                {
                    // DTO를 WeaponItemData로 변환하여 저장
                    foreach (var portionDTO in category.Value)
                    {
                        PortionItemData portionData = new PortionItemData(portionDTO);
                        dataDictionary[portionData.ID] = portionData;
                    }
                }
                return dataDictionary;
            }
            else
            {
                Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            }
        }
        else
        {
            Debug.LogWarning("PortionData.json 파일이 없음.");
        }
        return new Dictionary<int, PortionItemData>();
    }

    // 방어구 데이터 불러오기
    private Dictionary<int, ArmorItemData> LoadArmorData()
    {
        Dictionary<int, ArmorItemData> dataDictionary = new Dictionary<int, ArmorItemData>();

        if (File.Exists(armorItemDataPath))
        {
            string jsonData = File.ReadAllText(armorItemDataPath);
            var armorDict = JsonConvert.DeserializeObject<Dictionary<string, List<ArmorItemDTO>>>(jsonData);

            // 카테고리별 모든 데이터 저장
            if (armorDict != null)
            {
                foreach(var category in armorDict)  // "Top", "Shoes", "Gloves"
                {
                    foreach (var armorDTO in category.Value)
                    {
                        ArmorItemData armorData = new ArmorItemData(armorDTO);
                        dataDictionary[armorData.ID] = armorData;
                    }  
                }
                return dataDictionary;
            }
            else
            {
                Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            }
        }
        else
        {
            Debug.LogWarning("ArmorData.json 파일이 없음.");
        }
        return new Dictionary<int, ArmorItemData>();
    }

    // ID로 무기 데이터 가져오기
    public WeaponItemData GetWeaponDataById(int id)
    {
        if(weaponDataDictionary != null && weaponDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID에 해당하는 무기데이터가 없음.");
            return null;
        }
    }

    // ID로 포션 데이터 가져오기
    public PortionItemData GetPortionDataById(int id)
    {
        if (portionDataDictionary != null && portionDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID에 해당하는 포션데이터가 없음.");
            return null;
        }
    }

    // ID로 방어구 데이터 가져오기
    public ArmorItemData GetArmorDataById(int id)
    {
        if(armorDataDictionary != null && armorDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning(id + "에 해당하는 방어구데이터가 없음.");
            return null;
        }
    }

    // 플레이어 데이터에 대한 접근자 메서드
    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    // 플레이어 데이터 저장
    public void SavePlayerData()
    {
        PlayerDataDTO dto = playerData.ToDTO();
        SaveData(dto, "PlayerData");
        Debug.Log("세이브 완료!!");
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }
}
