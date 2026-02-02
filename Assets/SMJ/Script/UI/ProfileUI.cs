using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileUI : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private EquipmentSlotUI[] _equipmentSlots;

    [Header("References")]
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _satietyText;
    [SerializeField] private Text _hydrationText;
    [SerializeField] private Text _atkPText;
    [SerializeField] private Text _defPText;
    [SerializeField] private Text _moveSpeedText;

    private Player _player;
    private PlayerEquipment _playerEquipment;

    private void Start()
    {
        _player = Player.Instance;
        _playerEquipment = _player.Equipment;

        foreach (var slot in _equipmentSlots)
        {
            slot.Init(_playerEquipment);
        }

        _playerEquipment.OnEquipmentChanged += RefreshUI;

        RefreshUI();

        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_playerEquipment != null)
        {
            RefreshUI();
        }
    }

    private void Update()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (var slot in _equipmentSlots)
        {
            slot.UpdateSlotUI();
        }

        if (_player != null)
        {
            _hpText.text = $"{_player.State.GetCurrentHp():F0} / {_player.State.MaxHp:F0}";
            _satietyText.text = $"{_player.State.GetCurrentSatiety():F0}";
            _hydrationText.text = $"{_player.State.GetCurrentHydration():F0}";
            _atkPText.text = $"{_player.State.AttackPower:F1}";
            _defPText.text = $"{_player.State.DefensivePower:F1}";
            _moveSpeedText.text = $"{_player.State.MoveSpeed:F1}";
        }
    }
}
