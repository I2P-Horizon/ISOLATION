using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LYG_Inventory : MonoBehaviour
{
    public static LYG_Inventory Instance;

    //public List<LYG_Item> items;

    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private Slot[] slots;

    public Slot[] Slots => slots;

    //public static System.Action<List<LYG_Item>> OnInventoryChanged;

    public List<int> insertedItems = new List<int>();

    [System.Serializable]
    public struct IDtoImage
    {
        public int itemID;
        public Sprite itemImage;
    }

    public List<IDtoImage> imageDB;
    private Dictionary<int, Sprite> imageDict = new Dictionary<int, Sprite>();

#if UNITY_EDITOR
    private void OnValidate() {
        slots = slotParent.GetComponentsInChildren<Slot>();
    }
#endif

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var mapping in imageDB)
        {
            if (!imageDict.ContainsKey(mapping.itemID))
                imageDict.Add(mapping.itemID, mapping.itemImage);
        }

        RefreshBoard();
    }

    //public void FreshSlot()
    //{
    //    int i = 0;
    //    for (; i < items.Count && i < slots.Length; i++)
    //    {
    //        slots[i].item = items[i];
    //    }
    //    for (; i < slots.Length; i++)
    //    {
    //        slots[i].item = null;
    //    }

    //    OnInventoryChanged?.Invoke(items);

    //}

    //public void AddItem(LYG_Item _item)
    //{
    //    if (items.Count < slots.Length)
    //    {
    //        items.Add(_item);
    //        FreshSlot();
    //    }
    //    else
    //    {
    //        print("½½·ÔÀÌ °¡µæ Â÷ ÀÖ½À´Ï´Ù.");
    //    }
    //}

    //public void Resetslot()
    //{
    //    items.Clear();

    //    for (int j = 0; j < slots.Length; j++)
    //    {
    //        slots[j].item = null;
    //    }

    //    FreshSlot();
    //}

    public void RefreshBoard()
    {
        int i = 0;

        for (; i < insertedItems.Count && i < slots.Length; i++)
        {
            int itemID = insertedItems[i];
            if (imageDict.TryGetValue(itemID, out Sprite itemImage))
            {
                slots[i].SetSlot(itemImage, true);
            }
        }

        for (; i < slots.Length; i++)
        {
            slots[i].SetSlot(null, false);
        }
    }

    public void AddStoneToBoard(int itemID)
    {
        if (insertedItems.Count < slots.Length)
        {
            insertedItems.Add(itemID);
            RefreshBoard();
        }
        else
        {
            Debug.LogWarning("No more slots available for stones.");
        }
    }

    public void ResetBoard()
    {
        insertedItems.Clear();
        RefreshBoard();
        StoneBoard3D.Instance.ResetBoard();
    }

    public bool IsBoardFull()
    {
        return insertedItems.Count >= slots.Length;
    }
}