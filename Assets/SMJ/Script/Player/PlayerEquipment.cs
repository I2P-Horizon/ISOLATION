using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    private Dictionary<EquipmentType, EquipmentItem> _equippedItems = new Dictionary<EquipmentType, EquipmentItem>();

    private Player _player;

    [SerializeField] private Inventory _inventory;

    [Header("Model References")]
    [SerializeField] private Transform _rightHandMount;
    [SerializeField] private Transform _leftHandMount;
    [SerializeField] private Transform _bucketMount;
    [SerializeField] private GameObject _bagGo;
    [SerializeField] private GameObject _sunglassesGo;

    private GameObject _currentWeaponItem;
    private GameObject _currentToolItem;
    private GameObject _currentFaceItem;
    private GameObject _currentBackItem;

    public event Action OnArmorChanged;
    public event Action OnStateChanged;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }
    
    public bool Equip(EquipmentItem newItem)
    {
        if (newItem == null) return false;

        EquipmentType type = newItem.EquipmentData.TargetSlot;

        if (_equippedItems.TryGetValue(type, out EquipmentItem oldItem))
        {
            UnEquip(type);
        }

        _equippedItems[type] = newItem;

        applyStats(newItem, true);

        if (newItem is WeaponItem weapon)
        {
            updateWeaponModel(weapon);
        }
        else if (newItem is ArmorItem armor)
        {
            updateArmorModel(armor);
            OnArmorChanged?.Invoke();
        }

        return true;
    }

    public void UnEquip(EquipmentType type)
    {
        if (_equippedItems.TryGetValue(type, out EquipmentItem oldItem))
        {
            _inventory.AddItem(oldItem.EquipmentData);

            applyStats(oldItem, false);

            _equippedItems.Remove(type);

            if (type == EquipmentType.RightHand || type == EquipmentType.LeftHand)
            {
                removeWeaponModel(type);
            }
            else if (type == EquipmentType.Face || type == EquipmentType.Back)
            {
                removeArmorModel(type);
                OnArmorChanged?.Invoke();
            }
        }
    }

    public void UnEquipBucket()
    {
        if (_equippedItems.TryGetValue(EquipmentType.RightHand, out EquipmentItem bucket))
        {
            applyStats(bucket, false);
            _equippedItems.Remove(EquipmentType.RightHand);
            removeWeaponModel(EquipmentType.RightHand);
        }
    }

    private void updateWeaponModel(WeaponItem weapon)
    {
        removeWeaponModel(weapon.EquipmentData.TargetSlot);

        GameObject weaponPrefab = Resources.Load<GameObject>($"Prefabs/{weapon.WeaponData.ItemPrefab}");

        if (weaponPrefab == null)
        {
            Debug.LogWarning($"Weapon prefab not found: {weapon.WeaponData.ItemPrefab}");
            return;
        }

        if (weapon.EquipmentData.TargetSlot == EquipmentType.RightHand)
        {
            Transform mountPoint = _rightHandMount;

            if (weapon.WeaponData.Type == "Bucket")
            {
                mountPoint = _bucketMount;
            }

            _currentWeaponItem = Instantiate(weaponPrefab, mountPoint);

            _currentWeaponItem.transform.localPosition = Vector3.zero;
            _currentWeaponItem.transform.localRotation = Quaternion.identity;

            var rb = _currentWeaponItem.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);

            var collider = _currentWeaponItem.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var pickupScript = _currentWeaponItem.GetComponent<PickupItem>();
            if (pickupScript != null) Destroy(pickupScript);

            _currentWeaponItem.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        else if (weapon.EquipmentData.TargetSlot == EquipmentType.LeftHand)
        {
            _currentToolItem = Instantiate(weaponPrefab, _leftHandMount);

            _currentToolItem.transform.localPosition = Vector3.zero;
            _currentToolItem.transform.localRotation = Quaternion.identity;

            var rb = _currentToolItem.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);

            var collider = _currentToolItem.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var pickupScript = _currentToolItem.GetComponent<PickupItem>();
            if (pickupScript != null) Destroy(pickupScript);

            _currentToolItem.layer = LayerMask.NameToLayer("Player");
        }
    }

    private void removeWeaponModel(EquipmentType type)
    {
        if (type == EquipmentType.RightHand && _currentWeaponItem != null)
        {
            Destroy(_currentWeaponItem);
            _currentWeaponItem = null;
        }
        else if (type == EquipmentType.LeftHand && _currentToolItem != null)
        {
            Destroy(_currentToolItem);
            _currentToolItem = null;
        }
    }

    private void updateArmorModel(ArmorItem armor)
    {
        removeArmorModel(armor.EquipmentData.TargetSlot);

        if (armor.EquipmentData.TargetSlot == EquipmentType.Face)
        {
            _sunglassesGo.SetActive(true);
            _currentFaceItem = _sunglassesGo;
        }
        else if (armor.EquipmentData.TargetSlot == EquipmentType.Back)
        {
            _bagGo.SetActive(true);
            _currentBackItem = _bagGo;
        }
    }

    private void removeArmorModel(EquipmentType type)
    {
        if (type == EquipmentType.Face && _currentFaceItem != null)
        {
            _sunglassesGo.SetActive(false);
            _currentFaceItem = null;
        }
        else if (type == EquipmentType.Back && _currentBackItem != null)
        {
            _bagGo.SetActive(false);
            _currentBackItem = null;
        }
    }

    private void applyStats(EquipmentItem item, bool isEquipping)
    {
        float multiplier = isEquipping ? 1f : -1f;
        var data = item.EquipmentData;

        if (data is WeaponItemData weaponData)
        {
            _player.State.BaseAttackSpeed += weaponData.Rate * multiplier;
            _player.State.AttackPower += weaponData.Damage * multiplier;
        }
        else if (data is ArmorItemData armorData)
        {
            _player.State.DefensivePower += armorData.Defense * multiplier;
        }

        OnStateChanged?.Invoke();
    }

    public EquipmentItem GetEquippedItem(EquipmentType type)
    {
        if (_equippedItems.TryGetValue(type, out EquipmentItem item))
        {
            return item;
        }

        return null;
    }
}
