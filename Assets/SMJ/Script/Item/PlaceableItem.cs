using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    /// <summary>
    /// ���� ������ - ��ġ ������
    /// </summary>
    public class PlaceableItem : CountableItem, IUsableItem
    {
        public PlaceableItem(PlaceableItemData data, int amount = 1) : base(data, amount) { }

        public bool Use()
        {
            Amount--;

            // ���� ��ġ �ý��� ����

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PlaceableItem(CountableData as PlaceableItemData, amount);
        }
    }
}