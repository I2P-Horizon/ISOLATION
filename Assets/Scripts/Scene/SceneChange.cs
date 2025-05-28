using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    [Header("Next Scene Name")]
    public string nextScene;                        // ����� �� �̸�

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        // �� �ҷ�����
        Loading.LoadNextScene(nextScene);
    }
}
