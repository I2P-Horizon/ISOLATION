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
        IslandManager.OnGenerationComplete += StartFlow1;
    }

    private void OnDisable()
    {
        IslandManager.OnGenerationComplete -= StartFlow1;
    }

    private void StartFlow1()
    {
        StartCoroutine(Flow1());
    }

    public void StartFlow2()
    {
        StartCoroutine(Flow2());
    }

    public void StartFlow3()
    {
        StartCoroutine(Flow3());
    }

    private void OnDialogFinishedCallback()
    {
        dialogDone = true;
    }

    private IEnumerator Flow1()
    {
        yield return new WaitForSeconds(1f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(0, 2);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T1(TutorialManager.Instance));
    }

    private IEnumerator Flow2()
    {
        yield return new WaitForSeconds(2f);
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(3, 3);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T2(TutorialManager.Instance));
    }

    private IEnumerator Flow3()
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
}