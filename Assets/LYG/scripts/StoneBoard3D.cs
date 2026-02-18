using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoneBoard3D : MonoBehaviour
{
    public static StoneBoard3D Instance; // **SMJ : 싱글톤 패턴 적용**

    [System.Serializable]
    public struct StoneMapping
    {
        public int itemID;       // 조각 종류(ID) **SMJ : LYG_Item 대신 int로 변경**
        public GameObject stonePrefab;  // 대응되는 프리팹
    }

    [Header("조각별 매핑 리스트 (아이템 ↔ 프리팹)")]
    public List<StoneMapping> stoneMappings;

    [Header("조각이 배치될 슬롯 Transform (순서대로 연결)")]
    public Transform[] stoneSlots;

    [Header("Stone 오브젝트가 배치될 Layer")]
    public LayerMask stoneLayer; // 예: StonePiece

    private Dictionary<int, GameObject> stoneDict = new(); // **SMJ : LYG_Item 대신 int로 변경**
    private List<GameObject> currentStones = new();

    int nextSlotIndex = 0; // **SMJ : 다음에 배치할 슬롯 인덱스**

    private Inventory inventory; // **SMJ : 인벤토리 참조 추가**

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
            return;
        }

        foreach (var mapping in stoneMappings)
        {
            if (!stoneDict.ContainsKey(mapping.itemID))
                stoneDict.Add(mapping.itemID, mapping.stonePrefab);
        }
    }

    private void OnEnable()
    {
        inventory = FindFirstObjectByType<Inventory>(); // **SMJ : 인벤토리 참조 찾기**
    }

    // **SMJ : 기존 코드 제거 - LYG_Inventory와 연결되지 않음**
    //private void OnEnable()
    //{
    //    LYG_Inventory.OnInventoryChanged += UpdateBoard;
    //}

    //private void OnDisable()
    //{
    //    LYG_Inventory.OnInventoryChanged -= UpdateBoard;
    //}

    //private void UpdateBoard(List<LYG_Item> currentItems)
    //{
    //    // 기존 조각 제거
    //    foreach (var s in currentStones)
    //        Destroy(s);
    //    currentStones.Clear();

    //    if (currentItems == null || currentItems.Count == 0) return;

    //    // 인벤토리 순서대로 슬롯에 배치
    //    for (int i = 0; i < currentItems.Count && i < stoneSlots.Length; i++)
    //    {
    //        var item = currentItems[i];

    //        if (stoneDict.TryGetValue(item, out var prefab))
    //        {
    //            var slot = stoneSlots[i];
    //            GameObject newStone = Instantiate(prefab, slot.position, slot.rotation, slot);



    //            currentStones.Add(newStone);
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"Stone prefab not found for {item}");
    //        }
    //    }
    //}

    //private void Update()
    //{
    //    // 3D 오브젝트 클릭 처리 예시
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // UI 클릭 중이면 3D 오브젝트 클릭 무시
    //        if (EventSystem.current.IsPointerOverGameObject()) return;

    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(ray, out RaycastHit hit, 100f, stoneLayer))
    //        {
    //            Debug.Log($"Stone clicked: {hit.collider.gameObject.name}");
    //            // 원하는 클릭 처리
    //        }
    //    }
    //}

    // **SMJ: 버튼 클릭 시 조각 배치 메서드**
    public void TryInsertStone(int itemID)
    {
        if (nextSlotIndex >= stoneSlots.Length)
        {
            Debug.LogWarning("[StondBoard] No more slots available for stones.");
            return;
        }

        if (inventory.GetItemIndexByID(itemID) != -1)
        {
            if (stoneDict.TryGetValue(itemID, out var prefab))
            {
                inventory.ConsumeItem(itemID, 1);

                Transform targetSlot = stoneSlots[nextSlotIndex];
                GameObject newStone = Instantiate(prefab, targetSlot.position, targetSlot.rotation, targetSlot);
                newStone.transform.SetParent(targetSlot);

                LYG_Inventory.Instance.AddStoneToBoard(itemID);

                nextSlotIndex++;
            }
            else
            {
                Debug.LogWarning($"[StoneBoard] No prefab found for item ID {itemID}");
            }
        }
        else
        {
            Debug.LogWarning($"[StoneBoard] Item ID {itemID} not found in inventory.");
        }
    }

    public void ResetBoard()
    {
        nextSlotIndex = 0;

        foreach (Transform slot in stoneSlots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
    }
}
