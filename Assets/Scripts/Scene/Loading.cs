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
        "플레이어가 사망하면 마을로 돌아갑니다.",
        "포션으로 체력을 보충할 수 있습니다.",
        "더 좋은 장비는 던전을 클리어하는데 큰 도움이 됩니다."
    };

    private void Start()
    {
        StartCoroutine(LoadSceneProgress());
        ShowGameTips();
    }

    // 로딩씬 불러오기
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
