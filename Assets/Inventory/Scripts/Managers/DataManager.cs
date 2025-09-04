using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
///                             DataManager 
///                     1. �� ������ �ε� �� ���̺�
///                     2. �� �����Ϳ� ���� ������ �޼��� ���� 
///                     
/// </summary>
public class DataManager : Singleton<DataManager>
{
    private string playerDataPath;          // �÷��̾� ������ ������
    private string weaponItemDataPath;      // ���� ������ ������
    private string portionItemDataPath;     // ���� ������ ������
    private string armorItemDataPath;       // �� ������ ������
    private string foodItemDataPath;        // ���� ������ ������
    private string materialItemDataPath;    // ��� ������ ������
    private string placeableItemDataPath;   // ��ġ ������ ������ ������

    private Dictionary<int, WeaponItemData> weaponDataDictionary;
    private Dictionary<int, PortionItemData> portionDataDictionary;
    private Dictionary<int, ArmorItemData> armorDataDictionary;
    private Dictionary<int, FoodItemData> foodDataDictionary;
    private Dictionary<int, MaterialItemData> materialDataDictionary;
    private Dictionary<int, PlaceableItemData> placeableDataDictionary;
    private PlayerData playerData;

    protected override void Awake()
    {
        base.Awake();
        InitAndLoadData();
    }

    private void InitAndLoadData()
    {
        playerDataPath = Path.Combine(Application.persistentDataPath, "Playerdata.json");
        weaponItemDataPath = Path.Combine(Application.persistentDataPath, "WeaponData.json");
        portionItemDataPath = Path.Combine(Application.persistentDataPath, "PortionData.json");
        armorItemDataPath = Path.Combine(Application.persistentDataPath, "ArmorData.json");
        foodItemDataPath = Path.Combine(Application.persistentDataPath, "FoodData.json");
        materialItemDataPath = Path.Combine(Application.persistentDataPath, "MaterialData.json");
        placeableItemDataPath = Path.Combine(Application.persistentDataPath, "PlaceableData.json");

        playerData = LoadPlayerData();
        weaponDataDictionary = LoadWeaponData();
        portionDataDictionary = LoadPortionData();
        armorDataDictionary = LoadArmorData();
        foodDataDictionary = LoadFoodData();
        materialDataDictionary = LoadMaterialData();
        placeableDataDictionary = LoadPlaceableData();
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
        string persistentPath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "PlayerData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("PlayerData.json ���̺� ������ ���� StreamingAssets �������� �����մϴ�.");

            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("PlayerData.json ������ ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets ������ PlayerData.json ������ �����ϴ�.");
                return null;
            }
        }

        string jsonData = File.ReadAllText(persistentPath);
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

    // ���� ������ �ҷ�����
    private Dictionary<int, WeaponItemData> LoadWeaponData()
    {
        Dictionary<int, WeaponItemData> dataDictionary = new Dictionary<int, WeaponItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "WeaponData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "WeaponData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("WeaponData.json ���̺� ������ ���� StreamingAssets �������� �����մϴ�.");
            
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("WeaponData.json ������ ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets ������ WeaponData.json ������ �����ϴ�.");
                return dataDictionary;
            }
        }

        string jsonData = File.ReadAllText(persistentPath);
        var weaponDict = JsonConvert.DeserializeObject<Dictionary<string, List<WeaponItemDTO>>>(jsonData);

        if (weaponDict != null)
        {
            foreach (var category in weaponDict)
            {
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
            return new Dictionary<int, WeaponItemData>();
        }
    }

    // ���� ������ �ҷ�����
    private Dictionary<int, PortionItemData> LoadPortionData()
    {
        Dictionary<int, PortionItemData> dataDictionary = new Dictionary<int, PortionItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "PortionData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "PortionData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("PortionData.json ���̺� ������ ���� StreamingAssets �������� �����մϴ�.");
            
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("PortionData.json ������ ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets ������ PortionData.json ������ �����ϴ�.");
                return new Dictionary<int, PortionItemData>();
            }
        }

        string jsonData = File.ReadAllText(persistentPath);
        var portionDict = JsonConvert.DeserializeObject<Dictionary<string, List<PortionItemDTO>>>(jsonData);

        if (portionDict != null)
        {
            foreach (var category in portionDict)
            {
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
            return new Dictionary<int, PortionItemData>();
        }
    }

    // �� ������ �ҷ�����
    private Dictionary<int, ArmorItemData> LoadArmorData()
    {
        Dictionary<int, ArmorItemData> dataDictionary = new Dictionary<int, ArmorItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "ArmorData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "ArmorData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("ArmorData.json ���̺� ������ ���� StreamingAssets �������� �����մϴ�.");
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("ArmorData.json ������ ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets ������ ArmorData.json ������ �����ϴ�.");
                return new Dictionary<int, ArmorItemData>();
            }
        }

        string jsonData = File.ReadAllText(persistentPath);
        var armorDict = JsonConvert.DeserializeObject<Dictionary<string, List<ArmorItemDTO>>>(jsonData);

        if (armorDict != null)
        {
            foreach (var category in armorDict)
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
            return new Dictionary<int, ArmorItemData>();
        }
    }

    // ���� ������ �ҷ�����
    private Dictionary<int, FoodItemData> LoadFoodData()
    {
        Dictionary<int, FoodItemData> dataDictionary = new Dictionary<int, FoodItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "FoodData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "FoodData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("FoodData.json ���̺� ������ ���� StreamingAssets �������� �����մϴ�.");
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("FoodData.json ������ ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets ������ FoodData.json ������ �����ϴ�.");
                return new Dictionary<int, FoodItemData>();
            }
        }

        string jsonData = File.ReadAllText(persistentPath);
        var foodDict = JsonConvert.DeserializeObject<Dictionary<string, List<FoodItemDTO>>>(jsonData);
       
        if (foodDict != null)
        {
            foreach (var category in foodDict)
            {
                foreach (var foodDTO in category.Value)
                {
                    FoodItemData foodData = new FoodItemData(foodDTO);
                    dataDictionary[foodData.ID] = foodData;
                }
            }
            return dataDictionary;
        }
        else
        {
            Debug.LogWarning("Json �����͸� �Ľ��� �� ����.");
            return new Dictionary<int, FoodItemData>();
        }
    }

    // ��� ������ �ҷ�����
    private Dictionary<int, MaterialItemData> LoadMaterialData()
    {
        Dictionary<int, MaterialItemData> dataDictionary = new Dictionary<int, MaterialItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "MaterialData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "MaterialData.json");
        
        if (!File.Exists(persistentPath))
        {
            Debug.Log("MaterialData.json ���̺� ������ ���� StreamingAssets �������� �����մϴ�.");
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("MaterialData.json ������ ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets ������ MaterialData.json ������ �����ϴ�.");
                return new Dictionary<int, MaterialItemData>();
            }
        }
        
        string jsonData = File.ReadAllText(persistentPath);
        var materialDict = JsonConvert.DeserializeObject<Dictionary<string, List<MaterialItemDTO>>>(jsonData);
        
        if (materialDict != null)
        {
            foreach (var category in materialDict)
            {
                foreach (var materialDTO in category.Value)
                {
                    MaterialItemData materialData = new MaterialItemData(materialDTO);
                    dataDictionary[materialData.ID] = materialData;
                }
            }
            return dataDictionary;
        }
        else
        {
            Debug.LogWarning("Json �����͸� �Ľ��� �� ����.");
            return new Dictionary<int, MaterialItemData>();
        }
    }

    // ��ġ ������ ������ �ҷ�����
    private Dictionary<int, PlaceableItemData> LoadPlaceableData()
    {
        Dictionary<int, PlaceableItemData> dataDictionary = new Dictionary<int, PlaceableItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "PlaceableData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "PlaceableData.json");
        
        if (!File.Exists(persistentPath))
        {
            Debug.Log("PlaceableData.json ���̺� ������ ���� StreamingAssets �������� �����մϴ�.");
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("PlaceableData.json ������ ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets ������ PlaceableData.json ������ �����ϴ�.");
                return new Dictionary<int, PlaceableItemData>();
            }
        }
        
        string jsonData = File.ReadAllText(persistentPath);
        var placeableDict = JsonConvert.DeserializeObject<Dictionary<string, List<PlaceableItemDTO>>>(jsonData);
        
        if (placeableDict != null)
        {
            foreach (var category in placeableDict)
            {
                foreach (var placeableDTO in category.Value)
                {
                    PlaceableItemData placeableData = new PlaceableItemData(placeableDTO);
                    dataDictionary[placeableData.ID] = placeableData;
                }
            }
            return dataDictionary;
        }
        else
        {
            Debug.LogWarning("Json �����͸� �Ľ��� �� ����.");
            return new Dictionary<int, PlaceableItemData>();
        }
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
        if (armorDataDictionary != null && armorDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning(id + "�� �ش��ϴ� �������Ͱ� ����.");
            return null;
        }
    }

    // ID�� ���� ������ ��������
    public FoodItemData GetFoodDataById(int id)
    {
        if (foodDataDictionary != null && foodDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID�� �ش��ϴ� ���ĵ����Ͱ� ����.");
            return null;
        }
    }

    // ID�� ��� ������ ��������
    public MaterialItemData GetMaterialDataById(int id)
    {
        if (materialDataDictionary != null && materialDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID�� �ش��ϴ� ��ᵥ���Ͱ� ����.");
            return null;
        }
    }

    // ID�� ��ġ ������ ������ ��������
    public PlaceableItemData GetPlaceableDataById(int id)
    {
        if (placeableDataDictionary != null && placeableDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID�� �ش��ϴ� ��ġ�����۵����Ͱ� ����.");
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

}
