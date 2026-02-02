using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private EquipmentType _slotType;

    [Header("References")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject _emptyIcon;
    [SerializeField] private Slider _durabilitySlider;
    [SerializeField] private Image _durabilityFillImage;
    [SerializeField] private Gradient _durabilityGradient;

    private PlayerEquipment _playerEquipment;

    public void Init(PlayerEquipment playerEquip)
    {
        _playerEquipment = playerEquip;

        if (_durabilitySlider) _durabilitySlider.gameObject.SetActive(false);
    }

    public bool TryEquip(EquipmentItem newItem)
    {
        if (newItem == null) return false;

        if (newItem.EquipmentData.TargetSlot != _slotType)
        {
            return false;
        }

        return _playerEquipment.Equip(newItem);
    }

    public void UpdateSlotUI()
    {
        if (_playerEquipment == null) return;

        EquipmentItem item = _playerEquipment.GetEquippedItem(_slotType);

        if (item != null)
        {
            _iconImage.sprite = Resources.Load<Sprite>($"Icon/{item.EquipmentData.ItemIcon}");
            _iconImage.gameObject.SetActive(true);

            if (_emptyIcon) _emptyIcon.SetActive(false);

            updateDurabilityUI(item);
        }
        else
        {
            _iconImage.gameObject.SetActive(false);
            if (_emptyIcon) _emptyIcon.SetActive(true);

            if (_durabilitySlider) _durabilitySlider.gameObject.SetActive(false);
        }
    }

    private void updateDurabilityUI(EquipmentItem item)
    {
        if (_durabilitySlider == null) return;

        if (item.EquipmentData.MaxDurability > 0)
        {
            _durabilitySlider.gameObject.SetActive(true);

            float percent = item.CurrentDurability / item.EquipmentData.MaxDurability;
            _durabilitySlider.value = percent;

            if (_durabilityFillImage != null)
            {
                _durabilityFillImage.color = _durabilityGradient.Evaluate(percent);
            }
        }
        else
        {
            _durabilitySlider.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            _playerEquipment.UnEquip(_slotType);
        }
    }
}
