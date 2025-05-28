using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
                        Inventory
          
            - �κ��丮�� �������� ���� ����
                - ������ �߰�, ������ ���, ������ ����, ������ �̵�
            - �κ��丮 ������ Save & Load
*/
[System.Serializable]
public class ItemSlotData
{
    public int slotIndex;
    public int itemId;
    public int amount;
    public bool isAccessibleSlot;
}

[System.Serializable]
public class InventoryDataList
{
    public List<ItemSlotData> itemList;
}

public class Inventory : MonoBehaviour
{
    #region ** Serialized Fields **
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryGo;
    [SerializeField] private Item[] items;
    [SerializeField] private EquipmentUI equipmentUI;
    [SerializeField] private PlayerItemGroupUI playerItemGruopUI;
    #endregion

    #region ** Fields **
    public ItemData[] itemDataArray;
    public int Capacity { get; private set; }   // �κ��丮 �����ѵ�

    private int initCapacity = 24;              // �ʱ� �κ��丮 �����ѵ�
    private int maxCapacity = 36;               // �ִ� �κ��丮 �����ѵ�

    #endregion

    #region  ** Unity Events **
    private void Awake()
    {
        // �κ��丮���� ������ �� �ִ� �������� �ִ� 36��
        items = new Item[maxCapacity];                  
        itemDataArray = new ItemData[maxCapacity];

        // �ʱ� ���뷮 : 24(�ӽ�)
        Capacity = initCapacity;
        inventoryUI.SetInventoryRef(this);
    }

    private void Start()
    {
        LoadInventoryData();
        UpdateAccessibleSlots();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            UIManager.Instance.ToggleUI(inventoryGo);
        }
    }

    #endregion

    #region ** Private Methods **
    
    // �κ��丮 ������ �ε�
    private void LoadInventoryData()
    {
        string path = Path.Combine(Application.persistentDataPath, "InventoryData.json");

        if(File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            InventoryDataList dataList = JsonUtility.FromJson<InventoryDataList>(jsonData);

            foreach(var data in dataList.itemList)
            {
                itemDataArray[data.slotIndex] = ItemTypeById(data.itemId);

                // �ش� �����ε����� ����� �������� ���� ��
                if (itemDataArray[data.slotIndex] == null)
                {
                    continue;
                }

                // ������ ���� �� �ش� ���Կ� ���� ��ġ
                Item item = itemDataArray[data.slotIndex].CreateItem();

                if (item is CountableItem ci)
                    ci.SetAmount(data.amount);

                AddItemAt(data.slotIndex, item);
            }
        }

        // ������ Ÿ�Ժ� ��ȯ
        ItemData ItemTypeById(int id)
        {
            if (id > 10000 && id < 20000)
            {
                PortionItemData temp = DataManager.Instance.GetPortionDataById(id);
                return temp;
            }
            else if (id > 20000 && id < 30000)
            {
                ArmorItemData temp = DataManager.Instance.GetArmorDataById(id);
                return temp;
            }
            else if (id > 30000 && id < 40000)
            {
                WeaponItemData temp = DataManager.Instance.GetWeaponDataById(id);
                return temp;
            }
            else
                return null;
        }
    }

    // �κ��丮 ������ ����
    private void SaveInventoryData()
    {
        InventoryDataList saveData = new InventoryDataList();
        saveData.itemList = new List<ItemSlotData>();

        // ��� ������ ����
        for(int i = 0; i<items.Length; i++)
        {
            ItemSlotData slotData = new ItemSlotData();
            slotData.slotIndex = i;

            // �������� �ִ°��(id, amount ����)
            if(items[i] != null)
            {
                slotData.itemId = items[i].Data.ID;

                if (items[i] is CountableItem ci)
                    slotData.amount = ci.Amount;
                else
                    slotData.amount = 1;
            }
            // �������� ���°��
            else
            {
                slotData.itemId = 0;
                slotData.amount = 0;
            }

            saveData.itemList.Add(slotData);
        }

        string jsonData = JsonUtility.ToJson(saveData, true);
        string path = Path.Combine(Application.persistentDataPath, "InventoryData.json");
        File.WriteAllText(path, jsonData);

        Debug.Log("�κ��丮 ���� �Ϸ�");
    }

    // �κ��丮 ���ʺ��� ����ִ� ���� �ε��� Ž��(������ �󽽷� �ε��� ��ȯ, ���н� -1 ��ȯ)
    private int FindEmptySlotIndex(int startIndex = 0)
    {
        // ��ü ���� Ž��
        for(int i = startIndex; i < Capacity; i++)
        {
            // �� ������ �ִٸ� �� ������ �ε��� ��ȯ
            if (items[i] == null)
                return i;
        }

        // �� ������ ������ -1 ��ȯ
        return -1;
    }

    // �κ��丮 ���ʺ��� ���� ������ �ִ� Countable Item ���� �ε��� Ž��
    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        for(int i = startIndex; i<Capacity;i++)
        {
            var current = items[i];             // Ž������ ������ ���

            // �� ������ ��
            if (current == null)
                continue;

            // �������� target�� ��ġ
            if(current.Data == target && current is CountableItem ci)
            {
                // ���� ������ �ִ��� ����
                if (!ci.IsMax)
                    return i;       // ������ ������ �ε��� ��ȯ
            }
        }

        // ������ -1 ��ȯ
        return -1;
    }

    // �ش� �ε��� ���� ����
    private void UpdateSlot(int index)
    {
        // ��ȿ�� ���Ը�
        if (!IsValidIndex(index)) return;

        // �ش� ������
        Item item = items[index];

        // ���Կ� ������ ����
        if(item != null)
        {
            // 1. ������ �ִ� �������� ���
            if(item is CountableItem ci)
            {
                inventoryUI.SetItemIconAndAmountText(index, item.Data.ItemIcon, ci.Amount);
                playerItemGruopUI.UpdateSlots();
            }
            // 2. ��� ������
            else if(item is EquipmentItem ei)
            {
                inventoryUI.SetItemIconAndAmountText(index, item.Data.ItemIcon);
            }
            // 3. �� ��
            else
            {
                // ������ ǥ��
                inventoryUI.SetItemIconAndAmountText(index, item.Data.ItemIcon);
            }
        }
        // ���Կ� �������� ���� ��
        else
        {
            Remove(index);
            playerItemGruopUI.UpdateSlots();
        }

        // �κ��丮 ������ ����
        SaveInventoryData();
    }

    // �ε��� ���� ����(���� ���� ����) Overload
    private void UpdateSlot(params int[] indices)   // index�� ������
    {
        foreach(var i in indices)
        {
            UpdateSlot(i);
        }
    }
    #endregion

    #region ** Getter & Check Methods **
    // �ش� �ε����� ������ �������� �����ִ��� Ȯ��
    public bool HasItem(int index)
    {
        // ��ȿ�ϰ� ���Կ� �������� ��������� True
        return IsValidIndex(index) && items[index] != null;
    }

    // ��ȿ�� �ε��� ��ȣ���� Ȯ��
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    // �ش� �ε����� ������ Countable Item ���� ����
    private bool IsCountableItem(int index)
    {
        return HasItem(index) && items[index] is CountableItem;
    }

    // �ش� �ε����� ���� ������ ���� ��������
    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (items[index] == null) return null;

        return items[index].Data;
    }

    // �ش� �ε����� ���� ������ �̸� ��������
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (items[index] == null) return null;

        return items[index].Data.ItemName;
    }
    #endregion

    #region ** Public Methods **
    // Ȱ��ȭ ��ų ���Թ��� ������Ʈ
    public void UpdateAccessibleSlots()
    {
        inventoryUI.SetAccessibleSlotRange(Capacity);
    }

    // �κ��丮�� ������ �߰�(�׿� ������ ���� ����, ������ 0�̸� ��� ����)
    public int AddItem(ItemData itemData, int amount = 1)
    {
        int index;

        // 1. ItemData�� CountableItem�� ��� => ���� 1��~99������ ����
        if(itemData is CountableItemData ciData)
        {
            bool findNextCi = true;
            index = -1;

            // ���� ������ ������ ���������� �ݺ�
            while(amount > 0)
            {
                // �߰��� �������� �κ��丮�� ����
                if(findNextCi)
                {
                    // ���� ������ �ִ� ���� Ž��
                    index = FindCountableItemSlotIndex(ciData, index + 1);

                    // ���ٸ�
                    if(index == -1)
                    {
                        findNextCi = false;
                    }
                    // �ִٸ� ��ġ��
                    else
                    {
                        CountableItem ci = items[index] as CountableItem;
                        // ������ �ִ� ������ ������ �߰� �� �ʰ��� ��ȯ
                        amount = ci.AddAmountAndGetExcess(amount);

                        UpdateSlot(index);
                    }
                }
                // �߰��� �������� �κ��丮�� �������� ���� ��
                else
                {
                    index = FindEmptySlotIndex(index + 1);  // �󽽷� ã��

                    // �󽽷��� ���� ��
                    if(index == -1)
                    {
                        break;
                    }
                    else
                    {
                        // ���ο� ������ ����
                        CountableItem ci = ciData.CreateItem() as CountableItem;
                        ci.SetAmount(amount);

                        // ���Կ� ������ �߰�
                        items[index] = ci;

                        // ���� ���� ���
                        amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;

                        UpdateSlot(index);
                    }
                }
            }
        }
        // 2. ������(���� ���� ������)
        else
        {
            if (amount == 1)
            {
                // �� ������ ã�Ƽ� ������ ���� �� ���Կ� �߰�
                index = FindEmptySlotIndex();

                // �� ������ �ִٸ�
                if (index != -1)
                {
                    items[index] = itemData.CreateItem();
                    amount = 0;

                    // ���� ����
                    UpdateSlot(index);
                }
            }
        }
   
        return amount;
    }

    // Ư�� �κ��丮 ���Կ� Ư�� ������ �߰�
    public void AddItemAt(int index, Item item)
    {
        if(index < 0 || index >= maxCapacity)
        {
            Debug.LogWarning("���� �ε��� ���� �ʰ�" + index);
            return;
        }

        items[index] = item;

        UpdateSlot(index);
    }

    // �÷��̾� ������ ���Կ� ������ �߰�
    public void AddItemAtPlayerItemSlot(int index, PlayerItemSlotUI slot)
    {
        if (items[index] is CountableItem ci)
            playerItemGruopUI.SetItemIconAndAmountText(slot.index, ci);
    }

    // �ش� �ε��� ������ ������ ����
    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        if(items[index] is CountableItem)
            playerItemGruopUI.RemoveItem((CountableItem)items[index]);

        // �ε����� ������ ����
        items[index] = null;

        // ������ �� �ؽ�Ʈ ����
        inventoryUI.RemoveItem(index);
    }

    // �� ���� ������ ����
    public void Swap(int beginIndex, int endIndex)
    {
        // ���ٺҰ� ���� ó��
        if (!IsValidIndex(beginIndex) || !IsValidIndex(endIndex)) return;

        Item itemA = items[beginIndex];
        Item itemB = items[endIndex];

        // (A -> B ���� ����)
        // 1. Countable Item �̸鼭 ������ �������� ��
        if(itemA != null && itemB != null && itemA.Data == itemB.Data &&
           itemA is CountableItem ciA && itemB is CountableItem ciB && (ciB.Amount < ciB.MaxAmount))
        {
            int maxAmount = ciB.MaxAmount;          // B ������ �ִ뷮
            int sum = ciA.Amount + ciB.Amount;      // �� ������ ���� ����

            // �� �������� ���ĵ� �ִ�ġ���� ũ������ ��
            if(sum <= maxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - maxAmount);     
                ciB.SetAmount(maxAmount);           
            }
        }
        // 2. �Ϲ����� ���
        else
        {
            // ������ ��ġ ����
            items[beginIndex] = itemB;
            items[endIndex] = itemA;
        }
        AudioManager.Instance.PlaySFX("InventoryChange");
        // ���� ����
        UpdateSlot(beginIndex, endIndex);
    }

    // �ش� ���� �ε����� ������ ���
    public void Use(int index)
    {
        if (!IsValidIndex(index)) return;
        if (items[index] == null) return;

        // 1. �Ҹ� �������� ��
        if(items[index] is IUsableItem usable)
        {
            // ��뿡 �������� ��
            if(usable.Use())
            {
                // ������ �ִ� �������� ���
                if(items[index] is CountableItem ci)
                {
                    if(ci.IsEmpty)
                    {
                        Remove(index);      // ������ �� ����������� ����
                    }
                }
                // �Ϲ� �������� �ٷ� ����
                else
                {
                    Remove(index);
                }
            }
        }
        // 2. ��� �������� ��
        else if(items[index] is IEquipableItem)
        {
            // 2.1. ���� �������϶� 
            if(items[index] is WeaponItem curWeapon)
            {
                // �������� �������� ������ ����
                if (equipmentUI.slotUIList[0].HasItem)
                {
                    // �������� ������
                    WeaponItem prevItem = (WeaponItem)equipmentUI.items[0];
                    // �κ��丮�� ������ �߰�
                    AddItem(prevItem.Data);
                    // ĳ���� ����â ������ ������ ����
                    equipmentUI.slotUIList[0].RemoveItemIcon();
                    // ���� ����
                    prevItem.Unequip();
                }
                curWeapon.Equip();
                equipmentUI.SetItemIcon(curWeapon, curWeapon.WeaponData.Type, curWeapon.Data.ItemIcon);
            }
            // 2.2 �� �������϶�
            else if(items[index] is ArmorItem curArmor)
            {
                // �������� �������� ������ ����
                if(equipmentUI.slotUIList[TypeToIndex(curArmor)].HasItem)
                {
                    // �������� ������
                    ArmorItem prevItem = (ArmorItem)equipmentUI.items[TypeToIndex(curArmor)];
                    // �κ��丮�� ������ �߰�
                    AddItem(prevItem.Data);
                    // ĳ���� ����â ������ ������ ����
                    equipmentUI.slotUIList[TypeToIndex(curArmor)].RemoveItemIcon();
                    // ���� ����
                    prevItem.Unequip();
                }
                curArmor.Equip();
                equipmentUI.SetItemIcon(curArmor, curArmor.ArmorData.SubType, curArmor.Data.ItemIcon);
            }
            // �� Ÿ�Ժ� �ε���
            int TypeToIndex(ArmorItem curItem)
            {
                int typeIndex;
                switch (curItem.ArmorData.SubType)
                {
                    case "Shoes":
                        typeIndex = 1;
                        return typeIndex;
                    case "Gloves":
                        typeIndex = 2;
                        return typeIndex;
                    case "Top":
                        typeIndex = 3;
                        return typeIndex;
                    default:
                        return 0;
                }
            }

            Remove(index);
        }
            UpdateSlot(index); 
    }

    // ������ ������ ������ ���
    public void Use(CountableItem ci)
    {
        for(int i = 0;i<=items.Length;i++)
        {
            if (items[i] == ci)
            {
                Use(i);
                return;
            }
        }
    }
    #endregion
}
