using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoneBoard3D : MonoBehaviour
{
    [System.Serializable]
    public struct StoneMapping
    {
        public LYG_Item itemType;       // 조각 종류
        public GameObject stonePrefab;  // 대응되는 프리팹
    }

    [Header("조각별 매핑 리스트 (아이템 ↔ 프리팹)")]
    public List<StoneMapping> stoneMappings;

    [Header("조각이 배치될 슬롯 Transform (순서대로 연결)")]
    public Transform[] stoneSlots;

    [Header("Stone 오브젝트가 배치될 Layer")]
    public LayerMask stoneLayer; // 예: StonePiece

    private Dictionary<LYG_Item, GameObject> stoneDict = new();
    private List<GameObject> currentStones = new();

    private void Awake()
    {
        foreach (var mapping in stoneMappings)
        {
            if (!stoneDict.ContainsKey(mapping.itemType))
                stoneDict.Add(mapping.itemType, mapping.stonePrefab);
        }
    }

    private void OnEnable()
    {
        LYG_Inventory.OnInventoryChanged += UpdateBoard;
    }

    private void OnDisable()
    {
        LYG_Inventory.OnInventoryChanged -= UpdateBoard;
    }

    private void UpdateBoard(List<LYG_Item> currentItems)
    {
        // 기존 조각 제거
        foreach (var s in currentStones)
            Destroy(s);
        currentStones.Clear();

        if (currentItems == null || currentItems.Count == 0) return;

        // 인벤토리 순서대로 슬롯에 배치
        for (int i = 0; i < currentItems.Count && i < stoneSlots.Length; i++)
        {
            var item = currentItems[i];

            if (stoneDict.TryGetValue(item, out var prefab))
            {
                var slot = stoneSlots[i];
                GameObject newStone = Instantiate(prefab, slot.position, slot.rotation, slot);

                

                currentStones.Add(newStone);
            }
            else
            {
                Debug.LogWarning($"Stone prefab not found for {item}");
            }
        }
    }

    private void Update()
    {
        // 3D 오브젝트 클릭 처리 예시
        if (Input.GetMouseButtonDown(0))
        {
            // UI 클릭 중이면 3D 오브젝트 클릭 무시
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, stoneLayer))
            {
                Debug.Log($"Stone clicked: {hit.collider.gameObject.name}");
                // 원하는 클릭 처리
            }
        }
    }
}
