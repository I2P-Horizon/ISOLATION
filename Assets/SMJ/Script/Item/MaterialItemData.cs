using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    /// <summary>
    /// ��� ������ ����
    /// </summary>
    [CreateAssetMenu(fileName = "Item_Material_", menuName = "Inventory System/Item Data/Material", order = 6)]
    public class MaterialItemData : CountableItemData
    {
        public override Item CreateItem()
        {
            return new MaterialItem(this);
        }
    }
}