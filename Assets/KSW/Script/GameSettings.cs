using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private GameObject[] screenCheckmarks; // 화면 설정 버튼 배열
    [SerializeField] private GameObject[] graphicsCheckmarks; // 그래픽 설정 버튼 배열

    private int currentScreenIndex = 0; // 화면 설정 현재 인덱스 값
    private int currentGraphicsIndex = 0; // 그래픽 설정 현재 인덱스 값

    /// <summary>
    /// 선택된 항목만 체크 표시
    /// </summary>
    /// <param name="index"></param>
    private void SetActiveOnly(GameObject[] checkmarks, int index)
    {
        for (int i = 0; i < checkmarks.Length; i++) checkmarks[i].SetActive(i == index);
    }

    /// <summary>
    /// 화면 설정
    /// </summary>
    /// <param name="index"></param>
    public void OnClickScreenSetting(int index)
    {
        if (index == currentScreenIndex) return;
        SetActiveOnly(screenCheckmarks, index);
        currentScreenIndex = index;
    }

    /// <summary>
    /// 그래픽 설정
    /// </summary>
    /// <param name="index"></param>
    public void OnClickGraphicsSetting(int index)
    {
        if (index == currentGraphicsIndex) return;
        SetActiveOnly(graphicsCheckmarks, index);
        currentGraphicsIndex = index;
    }

    private void Start()
    {
        OnClickScreenSetting(0);
        OnClickGraphicsSetting(0);
    }
}