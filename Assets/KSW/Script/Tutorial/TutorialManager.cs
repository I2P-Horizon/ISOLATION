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
            FlowManager.Instance.StartFlow2();
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
            FlowManager.Instance.StartFlow3();
        }
    }

    public override void Exit() { }
}
#endregion

#region T3: 마우스 우클릭
public class T3 : TutorialState
{
    public T3(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("마우스 우클릭으로 공격하기");
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

#region 나무 조각 4개 획득
public class T4 : TutorialState
{
    public T4(TutorialManager manager) : base(manager) { }

    public override void Enter()
    {
        manager.Message("나무 조각 4개 획득");
        isCompleted = false;
    }

    public override void Update()
    {
        if (!isCompleted && Input.GetKeyDown(KeyCode.Tab))
        {
            isCompleted = true;
            manager.StartCoroutine(manager.EndTutorial("완료!"));
        }
    }

    public override void Exit() { }
}
#endregion

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Text messageText;

    private TutorialState currentState;

    /* 메시지 출력 */
    public void Message(string msg)
    {
        if (panel != null) panel.GetComponent<UIAnimator>().Show();
        if (messageText != null) messageText.text = msg;
    }

    /* 튜토리얼 상태 시작 */
    public void StartState(TutorialState nextState)
    {
        currentState?.Exit();
        currentState = nextState;
        currentState?.Enter();
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
        currentState?.Update();
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