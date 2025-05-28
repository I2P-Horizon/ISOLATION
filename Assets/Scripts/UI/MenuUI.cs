using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsButtonGo;
    [SerializeField] private GameObject exitButtonGo;
    [SerializeField] private GameObject cancelButtonGo;

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    public void OnCancelButtonClicked()
    {
        UIManager.Instance.CloseUI(this.gameObject);
    }
}
