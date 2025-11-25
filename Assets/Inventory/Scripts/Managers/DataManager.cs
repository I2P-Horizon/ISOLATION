using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
///                             DataManager 
///                     1. 각 데이터 로드 및 세이브
///                     2. 각 데이터에 대한 접근자 메서드 제공 
///                     
/// </summary>
public class DataManager : Singleton<DataManager>
{
    private string playerDataPath;          // 플레이어 데이터 저장경로
    private string weaponItemDataPath;      // 무기 데이터 저장경로
    private string portionItemDataPath;     // 포션 데이터 저장경로
    private string armorItemDataPath;       // 방어구 데이터 저장경로
    private string foodItemDataPath;        // 음식 데이터 저장경로
    private string materialItemDataPath;    // 재료 데이터 저장경로
    private string placeableItemDataPath;   // 설치 아이템 데이터 저장경로
    private string recipeDataPath;        // 조합 레시피 데이터 저장경로

    private Dictionary<int, WeaponItemData> weaponDataDictionary;
    private Dictionary<int, PortionItemData> portionDataDictionary;
    private Dictionary<int, ArmorItemData> armorDataDictionary;
    private Dictionary<int, FoodItemData> foodDataDictionary;
    private Dictionary<int, MaterialItemData> materialDataDictionary;
    private Dictionary<int, PlaceableItemData> placeableDataDictionary;
    private PlayerData playerData;

    public List<RecipeData> RecipeList { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitAndLoadData();
        Debug.Log(Application.persistentDataPath);
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
        recipeDataPath = Path.Combine(Application.persistentDataPath, "RecipeData.json");

        playerData = LoadPlayerData();
        weaponDataDictionary = LoadWeaponData();
        portionDataDictionary = LoadPortionData();
        armorDataDictionary = LoadArmorData();
        foodDataDictionary = LoadFoodData();
        materialDataDictionary = LoadMaterialData();
        placeableDataDictionary = LoadPlaceableData();
        RecipeList = LoadRecipeData();
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
        string persistentPath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "PlayerData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("PlayerData.json 세이브 파일이 없어 StreamingAssets 폴더에서 복사합니다.");

            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("PlayerData.json 파일이 복사되었습니다.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets 폴더에 PlayerData.json 파일이 없습니다.");
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
            Debug.LogWarning("PlayerData.json 파일에 Status 또는 Position 정보가 없음.");
            return null;
        }
    }

    // 무기 데이터 불러오기
    private Dictionary<int, WeaponItemData> LoadWeaponData()
    {
        Dictionary<int, WeaponItemData> dataDictionary = new Dictionary<int, WeaponItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "WeaponData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "WeaponData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("WeaponData.json 세이브 파일이 없어 StreamingAssets 폴더에서 복사합니다.");
            
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("WeaponData.json 파일이 복사되었습니다.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets 폴더에 WeaponData.json 파일이 없습니다.");
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
            Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            return new Dictionary<int, WeaponItemData>();
        }
    }

    // 포션 데이터 불러오기
    private Dictionary<int, PortionItemData> LoadPortionData()
    {
        Dictionary<int, PortionItemData> dataDictionary = new Dictionary<int, PortionItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "PortionData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "PortionData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("PortionData.json 세이브 파일이 없어 StreamingAssets 폴더에서 복사합니다.");
            
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("PortionData.json 파일이 복사되었습니다.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets 폴더에 PortionData.json 파일이 없습니다.");
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
            Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            return new Dictionary<int, PortionItemData>();
        }
    }

    // 방어구 데이터 불러오기
    private Dictionary<int, ArmorItemData> LoadArmorData()
    {
        Dictionary<int, ArmorItemData> dataDictionary = new Dictionary<int, ArmorItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "ArmorData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "ArmorData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("ArmorData.json 세이브 파일이 없어 StreamingAssets 폴더에서 복사합니다.");
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("ArmorData.json 파일이 복사되었습니다.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets 폴더에 ArmorData.json 파일이 없습니다.");
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
            Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            return new Dictionary<int, ArmorItemData>();
        }
    }

    // 음식 데이터 불러오기
    private Dictionary<int, FoodItemData> LoadFoodData()
    {
        Dictionary<int, FoodItemData> dataDictionary = new Dictionary<int, FoodItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "FoodData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "FoodData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("FoodData.json 세이브 파일이 없어 StreamingAssets 폴더에서 복사합니다.");
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("FoodData.json 파일이 복사되었습니다.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets 폴더에 FoodData.json 파일이 없습니다.");
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
            Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            return new Dictionary<int, FoodItemData>();
        }
    }

    // 재료 데이터 불러오기
    private Dictionary<int, MaterialItemData> LoadMaterialData()
    {
        Dictionary<int, MaterialItemData> dataDictionary = new Dictionary<int, MaterialItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "MaterialData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "MaterialData.json");
        
        if (!File.Exists(persistentPath))
        {
            Debug.Log("MaterialData.json 세이브 파일이 없어 StreamingAssets 폴더에서 복사합니다.");
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("MaterialData.json 파일이 복사되었습니다.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets 폴더에 MaterialData.json 파일이 없습니다.");
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
            Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            return new Dictionary<int, MaterialItemData>();
        }
    }

    // 설치 아이템 데이터 불러오기
    private Dictionary<int, PlaceableItemData> LoadPlaceableData()
    {
        Dictionary<int, PlaceableItemData> dataDictionary = new Dictionary<int, PlaceableItemData>();
        string persistentPath = Path.Combine(Application.persistentDataPath, "PlaceableData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "PlaceableData.json");
        
        if (!File.Exists(persistentPath))
        {
            Debug.Log("PlaceableData.json 세이브 파일이 없어 StreamingAssets 폴더에서 복사합니다.");
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("PlaceableData.json 파일이 복사되었습니다.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets 폴더에 PlaceableData.json 파일이 없습니다.");
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
            Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            return new Dictionary<int, PlaceableItemData>();
        }
    }

    // 조합 레시피 데이터 불러오기
    private List<RecipeData> LoadRecipeData()
    {
        string persistentPath = Path.Combine(Application.persistentDataPath, "RecipeData.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "RecipeData.json");

        if (!File.Exists(persistentPath))
        {
            Debug.Log("RecipeData.json 세이브 파일이 없어 StreamingAssets 폴더에서 복사합니다.");

            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("RecipeData.json 파일이 복사되었습니다.");
            }
            else
            {
                Debug.LogWarning("StreamingAssets 폴더에 RecipeData.json 파일이 없습니다.");
                return new List<RecipeData>();
            }
        }

        string jsonData = File.ReadAllText(persistentPath);

        List<RecipeData> recipes = JsonConvert.DeserializeObject<List<RecipeData>>(jsonData);

        if (recipes != null)
        {
            Debug.Log("조합 레시피 데이터 로드 완료. 총 " + recipes.Count + "개의 레시피가 로드되었습니다.");
            return recipes;
        }
        else
        {
            Debug.LogWarning("Json 데이터를 파싱할 수 없음.");
            return new List<RecipeData>();
        }
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
        if (armorDataDictionary != null && armorDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning(id + "에 해당하는 방어구데이터가 없음.");
            return null;
        }
    }

    // ID로 음식 데이터 가져오기
    public FoodItemData GetFoodDataById(int id)
    {
        if (foodDataDictionary != null && foodDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID에 해당하는 음식데이터가 없음.");
            return null;
        }
    }

    // ID로 재료 데이터 가져오기
    public MaterialItemData GetMaterialDataById(int id)
    {
        if (materialDataDictionary != null && materialDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID에 해당하는 재료데이터가 없음.");
            return null;
        }
    }

    // ID로 설치 아이템 데이터 가져오기
    public PlaceableItemData GetPlaceableDataById(int id)
    {
        if (placeableDataDictionary != null && placeableDataDictionary.TryGetValue(id, out var resultData))
        {
            return resultData;
        }
        else
        {
            Debug.LogWarning("ID에 해당하는 설치아이템데이터가 없음.");
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

    public ItemData GetItemDataByID(int id)
    {
        if (id > 10000 && id < 20000)
        {
            return GetPortionDataById(id);
        }
        else if (id > 20000 && id < 30000)
        {
            return GetArmorDataById(id);
        }
        else if (id > 30000 && id < 40000)
        {
            return GetWeaponDataById(id);
        }
        else if (id > 40000 && id < 50000)
        {
            return GetFoodDataById(id);
        }
        else if (id > 50000 && id < 60000)
        {
            return GetMaterialDataById(id);
        }
        else if (id > 60000 && id < 70000)
        {
            return GetPlaceableDataById(id);
        }
        else
        {
            Debug.LogWarning("ID에 해당하는 아이템데이터가 없음.");
            return null;
        }
    }
}
