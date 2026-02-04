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

    [Header("Player Images")]
    [SerializeField] private Image _playerImage;
    [SerializeField] private List<Sprite> _playerSprites;
    private Dictionary<string, Sprite> _spriteDict = new Dictionary<string, Sprite>();

    private Player _player;
    private PlayerEquipment _playerEquipment;

    private void Awake()
    {
        foreach (var sprite in _playerSprites)
        {
            if (sprite != null)
            {
                if (!_spriteDict.ContainsKey(sprite.name))
                {
                    _spriteDict.Add(sprite.name, sprite);
                }
            }
        }
    }

    private void Start()
    {
        _player = Player.Instance;
        _playerEquipment = _player.Equipment;

        foreach (var slot in _equipmentSlots)
        {
            slot.Init(_playerEquipment);
        }

        RefreshUI();

        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_playerEquipment != null)
        {
            RefreshUI();
            _playerEquipment.OnArmorChanged += UpdatePlayerImage;
        }
    }

    private void OnDisable()
    {
        _playerEquipment.OnArmorChanged -= UpdatePlayerImage;
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

    private Sprite getSpriteByName(string name)
    {
        if (_spriteDict.ContainsKey(name)) return _spriteDict[name];

        return null;
    }

    public void UpdatePlayerImage()
    {
        bool isPlayerWearSunglasses = false;
        bool isPlayerWearBag = false;

        if (_player.Equipment.GetEquippedItem(EquipmentType.Face) != null)
        {
            isPlayerWearSunglasses = true;
        }

        if (_player.Equipment.GetEquippedItem(EquipmentType.Back) != null)
        {
            isPlayerWearBag = true;
        }

        if (isPlayerWearBag && isPlayerWearSunglasses)
        {
            _playerImage.sprite = getSpriteByName("PlayerSunglassesBag");
        }
        else if (isPlayerWearBag && !isPlayerWearSunglasses)
        {
            _playerImage.sprite = getSpriteByName("PlayerBag");
        }
        else if (!isPlayerWearBag && isPlayerWearSunglasses)
        {
            _playerImage.sprite = getSpriteByName("PlayerSunglasses");
        }
        else
        {
            _playerImage.sprite = getSpriteByName("PlayerNoItem");
        }
    }
}
