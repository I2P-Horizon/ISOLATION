using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    private bool dialogDone = false;

    private void OnEnable()
    {
        IslandManager.OnGenerationComplete += OnGenerationComplete;
    }

    private void OnDisable()
    {
        IslandManager.OnGenerationComplete -= OnGenerationComplete;
    }

    private void OnGenerationComplete()
    {
        StartCoroutine(Flow1());
    }

    private void OnDialogFinishedCallback()
    {
        dialogDone = true;
    }

    private IEnumerator Flow1()
    {
        dialogDone = false;
        DialogManager.Instance.OnDialogFinished += OnDialogFinishedCallback;
        DialogManager.Instance.Show(0, 2);
        yield return new WaitUntil(() => dialogDone);
        DialogManager.Instance.OnDialogFinished -= OnDialogFinishedCallback;
        TutorialManager.Instance.StartState(new T1(TutorialManager.Instance));
    }
}