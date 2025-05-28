using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
///                             DataManager 
///                     1. �� ������ �ε� �� ���̺�
///                     2. �� �����Ϳ� ���� ��������
///                     
/// </summary>
public class DataManager : Singleton<DataManager>
{
    private string playerDataPath;          // �÷��̾� ������ ������
    private string weaponItemDataPath;      // ���� ������ ������
    private string portionItemDataPath;     // ���� ������ ������
    private string armorItemDataPath;       // �� ������ ������

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
        // 5�ʸ��� �ڵ� ����
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

    // ������ ����
    public void SaveData<T>(T data, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, jsonData);
    }

    // �÷��̾� ������ �ҷ�����
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
                Debug.LogWarning("PlayerData.json ���Ͽ� Status �Ǵ� Position ������ ����.");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("PlayerData.json ������ ����.");
            return null;
        }
    }

    // ���� ������ �ҷ�����
    private Dictionary<int, WeaponItemData> LoadWeaponData()
    {
        Dictionary<int, WeaponItemData> dataDictionary = new Dictionary<int, WeaponItemData>();

        if (File.Exists(weaponItemDataPath))
        {
            string jsonData = File.ReadAllText(weaponItemDataPath);
            var weaponDict = JsonConvert.DeserializeObject<Dictionary<string, List<WeaponItemDTO>>>(jsonData);

            // ī�װ��� ��� ������ ����
            if(weaponDict != null)
            {
                foreach(var category in weaponDict)
                {
                    // DTO�� WeaponItemData�� ��ȯ�Ͽ� ����
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
                Debug.LogWarning("Json �����͸� �Ľ��� �� ����.");
            }
        }
        else
        {
            Debug.LogWarning("WeaponData.json ������ ����.");
        }
        return new Dictionary<int, WeaponItemData>();
    }

    // ���� ������ �ҷ�����
    private Dictionary<int, PortionItemData> LoadPortionData()
    {
        Dictionary<int, PortionItemData> dataDictionary = new Dictionary<int, PortionItemData>();

        if (File.Exists(portionItemDataPath))
        {
            string jsonData = File.ReadAllText(portionItemDataPath);
            var portionDict = JsonConvert.DeserializeObject<Dictionary<string, List<PortionItemDTO>>>(jsonData);

            // ī�װ��� ��� ������ ����
            if (portionDict !=  null)
            {
                foreach (var category in portionDict)
                {
                    // DTO�� WeaponItemData�� ��ȯ�Ͽ� ����
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
                Debug.LogWarning("Json �����͸� �Ľ��� �� ����.");
            }
        }
        else
        {
            Debug.LogWarning("PortionData.json ������ ����.");
        }
        return new Dictionary<int, PortionItemData>();
    }

    // �� ������ �ҷ�����
    private Dictionary<int, ArmorItemData> LoadArmorData()
    {
        Dictionary<int, ArmorItemData> dataDictionary = new Dictionary<int, ArmorItemData>();

        if (File.Exists(armorItemDataPath))
        {
            string jsonData = File.ReadAllText(armorItemDataPath);
            var armorDict = JsonConvert.DeserializeObject<Dictionary<string, List<ArmorItemDTO>>>(jsonData);

            // ī�װ��� ��� ������ ����
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
                Debug.LogWarning("Json �����͸� �Ľ��� �� ����.");
            }
        }
        else
        {
            Debug.LogWarning("ArmorData.json ������ ����.");
        }
        return new Dictionary<int, ArmorItemData>();
    }

    // ID�� ���� ������ ��������
    public WeaponItemData GetWeaponDataById(int id)
    {
        if(weaponDataDictionary != null && weaponDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID�� �ش��ϴ� ���ⵥ���Ͱ� ����.");
            return null;
        }
    }

    // ID�� ���� ������ ��������
    public PortionItemData GetPortionDataById(int id)
    {
        if (portionDataDictionary != null && portionDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID�� �ش��ϴ� ���ǵ����Ͱ� ����.");
            return null;
        }
    }

    // ID�� �� ������ ��������
    public ArmorItemData GetArmorDataById(int id)
    {
        if(armorDataDictionary != null && armorDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning(id + "�� �ش��ϴ� �������Ͱ� ����.");
            return null;
        }
    }

    // �÷��̾� �����Ϳ� ���� ������ �޼���
    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    // �÷��̾� ������ ����
    public void SavePlayerData()
    {
        PlayerDataDTO dto = playerData.ToDTO();
        SaveData(dto, "PlayerData");
        Debug.Log("���̺� �Ϸ�!!");
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }
}
