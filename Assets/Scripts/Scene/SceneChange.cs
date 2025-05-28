using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    [Header("Next Scene Name")]
    public string nextScene;                        // 연결된 씬 이름

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        // 씬 불러오기
        Loading.LoadNextScene(nextScene);
    }
}
