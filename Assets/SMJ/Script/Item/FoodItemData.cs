using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditorInternal;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Itme_Food_", menuName = "Inventory System/Item Data/Food", order = 4)]
    public class FoodItemData : CountableItemData
    {
        /// <summary>포만감 회복량</summary>
        public float Value => _value;
        [SerializeField] private float _value;

        public override Item CreateItem()
        {
            return new FoodItem(this);
        }
    }
}