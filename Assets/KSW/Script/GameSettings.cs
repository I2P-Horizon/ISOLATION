using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private GameObject[] screenCheckmarks; // ȭ�� ���� ��ư �迭
    [SerializeField] private GameObject[] graphicsCheckmarks; // �׷��� ���� ��ư �迭

    private int currentScreenIndex = 0; // ȭ�� ���� ���� �ε��� ��
    private int currentGraphicsIndex = 0; // �׷��� ���� ���� �ε��� ��

    /// <summary>
    /// ���õ� �׸� üũ ǥ��
    /// </summary>
    /// <param name="index"></param>
    private void SetActiveOnly(GameObject[] checkmarks, int index)
    {
        for (int i = 0; i < checkmarks.Length; i++) checkmarks[i].SetActive(i == index);
    }

    /// <summary>
    /// ȭ�� ����
    /// </summary>
    /// <param name="index"></param>
    public void OnClickScreenSetting(int index)
    {
        if (index == currentScreenIndex) return;
        SetActiveOnly(screenCheckmarks, index);
        currentScreenIndex = index;
    }

    /// <summary>
    /// �׷��� ����
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