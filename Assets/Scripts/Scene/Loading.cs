using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private Text tipText;

    
    private static string nextScene;
    string[] gameTips =
    {
        "�÷��̾ ����ϸ� ������ ���ư��ϴ�.",
        "�������� ü���� ������ �� �ֽ��ϴ�.",
        "�� ���� ���� ������ Ŭ�����ϴµ� ū ������ �˴ϴ�."
    };

    private void Start()
    {
        StartCoroutine(LoadSceneProgress());
        ShowGameTips();
    }

    // �ε��� �ҷ�����
    public static void LoadNextScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    private IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        
        float timer = 0f;

        while(!op.isDone)
        {
            yield return null;

            if(op.progress < 0.9f)
            {
                progressBar.rectTransform.sizeDelta = new Vector2(op.progress * 1920f, 80f);
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.rectTransform.sizeDelta = new Vector2(1728f + timer * 20f, 80f);
                if(progressBar.rectTransform.sizeDelta.x >= 1920f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private void ShowGameTips()
    {
        int ran = Random.Range(0, gameTips.Length);

        tipText.text = "Game Tip. " + gameTips[ran];
    }
}
