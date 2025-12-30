using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    private Button _button;

    private void onClick()
    {
        _button.interactable = false;
    }

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.interactable = true;
        _button.onClick.AddListener(onClick);
    }
}