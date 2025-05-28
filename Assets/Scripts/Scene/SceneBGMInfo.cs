using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneBGMInfo : MonoBehaviour
{
    [Tooltip("�� ������ ����� BGM Key")]
    [SerializeField] private string bgmKey;

    private void Start()
    {
        if(!string.IsNullOrEmpty(bgmKey))
        {
            AudioManager.Instance.PlayBGM(bgmKey);
        }
    }
}
