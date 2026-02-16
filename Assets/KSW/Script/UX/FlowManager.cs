using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    #region Singleton
    public static FlowManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private bool dialogDone = false;

    private void OnEnable()
    {
        IslandManager.OnGenerationComplete += startFlow1;
    }

    private void OnDisable()
    {
        IslandManager.OnGenerationComplete -= startFlow1;
    }

    private void startFlow1()
    {
        StartCoroutine(Flow1());
    }

    private void OnDialogFinishedCallback()
    {
        dialogDone = true;
    }

    private IEnumerator Flow1()
    {
        yield return new WaitForSeconds(2.5f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(0, 2);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T1(TutorialManager.Instance));
    }

    public IEnumerator Flow2()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(3, 3);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T2(TutorialManager.Instance));
    }

    /// <summary>
    /// 주머니에 지도가 있다. -> T4 : M키로 지도 열기
    /// </summary>
    /// <returns></returns>
    public IEnumerator Flow3()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(4, 4);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        yield return new WaitForSeconds(1.5f);
        TutorialManager.Instance.StartState(new T3(TutorialManager.Instance));
    }

    /// <summary>
    /// Tab키를 눌러 인벤토리를 열 수 있습니다. -> T5 : 인벤토리 열기
    /// </summary>
    /// <returns></returns>
    public IEnumerator Flow4()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(5, 5);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T5(TutorialManager.Instance));
    }

    /// <summary>
    /// 우클릭을 하여 작업대를 배치해 보세요. -> T6 : 작업대 배치
    /// </summary>
    /// <returns></returns>
    public IEnumerator Flow5()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(6, 6);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T6(TutorialManager.Instance));
    }

    /// <summary>
    /// 작업대에 다가간 후 F키를 눌러 상호작용하세요. -> T7 : 나무 곡괭이 제작
    /// </summary>
    /// <returns></returns>
    public IEnumerator Flow6()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(7, 7);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T7(TutorialManager.Instance));
    }

    /// <summary>
    /// P키를 눌러 캐릭터 정보를 확인할 수 있습니다. -> T8 : 나무 곡괭이를 드래그하여 장착
    /// </summary>
    /// <returns></returns>
    public IEnumerator Flow7()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(8, 8);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T8(TutorialManager.Instance));
    }

    /// <summary>
    ///  밤에 일어날 일을 예측하려면‘예지의 눈’을 사용해야 합니다. -> T10 : 광산에서 예지의 눈 조각 획득
    /// </summary>
    /// <returns></returns>
    public IEnumerator Flow8()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(9, 10);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T10(TutorialManager.Instance));
    }

    /// <summary>
    ///  예지의 눈 조각은 섬 중앙 근처 고대 사원 내부에서 사용할 수 있습니다. -> T11 : 고대 사원에서 예지의 눈 조각 맞추기
    /// </summary>
    /// <returns></returns>
    public IEnumerator Flow9()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(11, 11);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T11(TutorialManager.Instance));
    }
}