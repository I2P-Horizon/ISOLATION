using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    private PlayerCondition _playerCondition;

    [SerializeField] private RectTransform _buffParent;
    [SerializeField] private BuffIcon _buffIconPrefab;
    [SerializeField] private float _spacing = 75f;
    [SerializeField] private float _offset = -35f;

    private Dictionary<ConditionType, Sprite> _iconMap = new();
    private List<BuffIcon> _icons = new();

    [SerializeField] private BuffIconData[] _buffIconData;

    [System.Serializable]
    public struct BuffIconData
    {
        public ConditionType type;
        public Sprite icon;
    }

    private readonly ConditionType[] watchBuffs =
    {
        ConditionType.StrengthUp,
        ConditionType.Electrolyte,
        ConditionType.FrostResist,
        ConditionType.FireResist
    };

    private void Awake()
    {
        _playerCondition = FindFirstObjectByType<PlayerCondition>();

        foreach (var data in _buffIconData)
            _iconMap[data.type] = data.icon;
    }

    //private void Start()
    //{
    //    StartCoroutine(buff());
    //}

    //private IEnumerator buff()
    //{
    //    yield return new WaitForSeconds(1f);
    //    _playerCondition.AddCondition(ConditionType.StrengthUp, 6f);
    //    yield return new WaitForSeconds(1f);
    //    _playerCondition.AddCondition(ConditionType.Electrolyte, 6f);
    //    yield return new WaitForSeconds(1f);
    //    _playerCondition.AddCondition(ConditionType.FrostResist, 8f);
    //    yield return new WaitForSeconds(1f);
    //    _playerCondition.AddCondition(ConditionType.FireResist, 6f);
    //}

    private void Update()
    {
        sync();
    }

    private void sync()
    {
        /* 새로 생긴 버프 처리 */
        foreach (var type in watchBuffs)
        {
            if (_playerCondition.HasCondition(type) &&
                !_icons.Exists(i => i.Type == type))
            {
                add(type);
            }
        }

        /* 끝난 버프 처리 */
        for (int i = _icons.Count - 1; i >= 0; i--)
        {
            if (!_playerCondition.HasCondition(_icons[i].Type))
            {
                remove(_icons[i]);
            }
        }

        /* 아이콘 재정렬 */
        updatePositions();
    }

    private void add(ConditionType type)
    {
        BuffIcon icon = Instantiate(_buffIconPrefab, _buffParent);
        icon.Init(type, _iconMap[type]);

        /* 새로운 버프를 맨 앞에 추가 */
        _icons.Insert(0, icon);

        /* 생성 시 바로 기준 위치에 배치 */
        icon.SetImmediatePosition(new Vector2(_offset, 0));

        /* UIAnimator 추가 실행 */
        UIAnimator anim = icon.GetComponent<UIAnimator>();
        if (anim != null) anim.Show();
    }

    private void remove(BuffIcon icon)
    {
        _icons.Remove(icon);
        StartCoroutine(removeRoutine(icon));
    }

    private IEnumerator removeRoutine(BuffIcon icon)
    {
        UIAnimator anim = icon.GetComponent<UIAnimator>();

        if (anim != null)
        {
            anim.Close();

            /* UIAnimator가 비활성화될 때까지 대기 */
            while (icon.gameObject.activeSelf)
                yield return null;
        }

        Destroy(icon.gameObject);
    }


    private void updatePositions()
    {
        /* 현재 아이콘 순서에 맞게 위치를 재계산하여 이동 애니메이션 적용 */
        for (int i = 0; i < _icons.Count; i++)
        {
            float x = _offset - _spacing * i;
            _icons[i].MoveTo(new Vector2(x, 0));
        }
    }
}