using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#region T1: WASD키로 이동하기
public class T1 : TutorialState
{
    public T1(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("WASD키로 이동하기");
        isCompleted = false;
    }

    public override void Update()
    {
        if (!isCompleted &&
            (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D)))
        {
            isCompleted = true;
            manager.StartCoroutine(manager.EndTutorial("완료!"));
            manager.StartCoroutine(FlowManager.Instance.Flow2());
        }
    }

    public override void Exit() { }
}
#endregion

#region T2: M키로 지도 열기
public class T2 : TutorialState
{
    public T2(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("M키로 지도 열기");
        isCompleted = false;
    }

    public override void Update()
    {
        if (!isCompleted && Input.GetKeyDown(KeyCode.M))
        {
            isCompleted = true;
            manager.StartCoroutine(manager.EndTutorial("완료!"));
            manager.StartCoroutine(FlowManager.Instance.Flow3());
        }
    }

    public override void Exit() { }
}
#endregion

#region T3: 마우스 좌클릭
public class T3 : TutorialState
{
    public T3(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("마우스 좌클릭으로 공격하기");
        isCompleted = false;
    }

    public override void Update()
    {
        if (!isCompleted && Input.GetMouseButtonDown(0))
        {
            isCompleted = true;
            manager.StartCoroutine(manager.NextState(new T4(manager), "완료!"));
        }
    }

    public override void Exit() { }
}
#endregion

#region T4: 나무 조각 4개 획득
public class T4 : TutorialState
{
    public T4(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        inventory = MonoBehaviour.FindFirstObjectByType<Inventory>();
        manager.Message("나무 조각 6개 획득");
        isCompleted = false;

        CheckClear();
    }

    public override void Update()
    {
        if (!isCompleted)
            CheckClear();
    }

    public override void Exit() { }

    private void CheckClear()
    {
        if (inventory.GetTotalAmountOfItem(50001) >= 6)
        {
            isCompleted = true;
            manager.StartCoroutine(CompleteRoutine());
        }
    }

    private IEnumerator CompleteRoutine()
    {
        yield return manager.EndTutorial("완료!");
        yield return FlowManager.Instance.Flow4();
    }
}
#endregion

#region T5: 인벤토리 열기
public class T5 : TutorialState
{
    public T5(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("인벤토리 열기");
        isCompleted = false;
    }

    public override void Update()
    {
        if (!isCompleted && Input.GetKeyDown(KeyCode.Tab))
        {
            isCompleted = true;
            manager.StartCoroutine(manager.EndTutorial("완료!"));
            manager.StartCoroutine(FlowManager.Instance.Flow5());
        }
    }

    public override void Exit() { }
}
#endregion

#region T6: 작업대 배치
public class T6 : TutorialState
{
    public T6(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("작업대 배치");
        isCompleted = false;
    }

    public override void Update()
    {
        CraftingTable[] tables = MonoBehaviour.FindObjectsOfType<CraftingTable>();
        if (tables.Length > 0 && Input.GetMouseButtonDown(1))
        {
            isCompleted = true;
            manager.StartCoroutine(manager.EndTutorial("완료!"));
            manager.StartCoroutine(FlowManager.Instance.Flow6());
        }
    }

    public override void Exit() { }
}
#endregion

#region T7: 나무 곡괭이 제작
public class T7 : TutorialState
{
    public T7(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("나무 곡괭이 제작");
        isCompleted = false;
    }

    public override void Update()
    {
        if (isCompleted) return;

        /* Inventory에서 나무 곡괭이 아이템이 있는지 확인 */
        Inventory playerInventory = MonoBehaviour.FindObjectOfType<Inventory>();
        if (playerInventory != null)
        {
            int count = playerInventory.GetTotalAmountOfItem(30001);
            if (count > 0)
            {
                isCompleted = true;
                manager.StartCoroutine(manager.EndTutorial("완료!"));
                manager.StartCoroutine(FlowManager.Instance.Flow7());
            }
        }
    }

    public override void Exit() { }
}
#endregion

#region T8: 나무 곡괭이 착용
public class T8 : TutorialState
{
    public T8(TutorialManager manager) : base(manager) { }

    private PlayerEquipment _equipment;

    public override void Enter()
    {
        manager.Message("나무 곡괭이를 드래그하여 장착");
        isCompleted = false;

        Player player = Player.Instance;
        if (player != null)
        {
            _equipment = player.Equipment;
            checkWeapon();
        }
    }

    public override void Update()
    {
        if (isCompleted) return;

        if (_equipment == null) return;

        checkWeapon();
    }

    private void checkWeapon()
    {
        EquipmentItem weaponItem = _equipment.GetEquippedItem(EquipmentType.RightHand);
        if (weaponItem != null)
        {
            if (weaponItem is WeaponItem weapon)
            {
                if ((weapon.WeaponData as WeaponItemData).ItemPrefab == "WoodPickax")
                {
                    isCompleted = true;
                    manager.StartCoroutine(manager.NextState(new T9(manager), "완료!"));
                }
            }
        }
    }

    public override void Exit() { }
}
#endregion

#region T9: 돌 조각 4개 획득
public class T9 : TutorialState
{
    public T9(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        inventory = MonoBehaviour.FindFirstObjectByType<Inventory>();
        manager.Message("돌 조각 4개 획득");
        isCompleted = false;
    }

    public override void Update()
    {
        if (isCompleted) return;

        if (inventory.GetTotalAmountOfItem(50002) >= 4)
        {
            isCompleted = true;
            manager.StartCoroutine(manager.EndTutorial("완료!"));
            manager.StartCoroutine(FlowManager.Instance.Flow8());

        }
    }

    public override void Exit() { }
}
#endregion

#region T10: 예지의 눈 조각 1개 획득
public class T10 : TutorialState
{
    public T10(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        inventory = MonoBehaviour.FindFirstObjectByType<Inventory>();
        manager.Message("광산에서 예지의 눈 조각 획득");
        isCompleted = false;
    }

    public override void Update()
    {
        if (isCompleted) return;

        if (inventory.GetTotalAmountOfItem(50011) == 1 ||
            inventory.GetTotalAmountOfItem(50012) == 1 ||
            inventory.GetTotalAmountOfItem(50013) == 1 ||
            inventory.GetTotalAmountOfItem(50014) == 1)
        {
            isCompleted = true;
            manager.StartCoroutine(manager.EndTutorial("완료!"));
            manager.StartCoroutine(FlowManager.Instance.Flow9());
        }
    }

    public override void Exit() { }
}
#endregion

#region T11: 고대 사원 조각 맞추기
public class T11 : TutorialState
{
    public T11(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("고대 사원에서 예지의 눈 조각 맞추기");
        isCompleted = false;
    }

    public override void Update()
    {
        if (isCompleted) return;

        // 고대 사원 조각 1개 맞추기
    }

    public override void Exit() { }
}
#endregion

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Text messageText;

    private TutorialState _currentState;

    /* 메시지 출력 */
    public void Message(string msg)
    {
        if (panel != null) panel.GetComponent<UIAnimator>().Show();
        if (messageText != null) messageText.text = msg;
    }

    /* 튜토리얼 상태 시작 */
    public void StartState(TutorialState nextState)
    {
        _currentState?.Exit();
        _currentState = nextState;
        _currentState?.Enter();
    }

    /* 이전 튜토리얼 상태 완료 + 다음 상태 시작 */
    public IEnumerator NextState(TutorialState nextState, string endMsg)
    {
        //Message(endMsg);
        /* Message로 실행하면 새로운 박스가 생성되므로 text만 변경. */
        if (messageText != null) messageText.text = endMsg;
        yield return new WaitForSeconds(1f);
        if (panel != null) panel.GetComponent<UIAnimator>().Close();
        yield return new WaitForSeconds(1.5f);

        /* 다음 상태로 전환 */
        StartState(nextState);
    }

    public IEnumerator EndTutorial(string endMsg)
    {
        if (messageText != null) messageText.text = endMsg;
        yield return new WaitForSeconds(1f);
        if (panel != null) panel.GetComponent<UIAnimator>().Close();
    }

    private void Update()
    {
        _currentState?.Update();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}