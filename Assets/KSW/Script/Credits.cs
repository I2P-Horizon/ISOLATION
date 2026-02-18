using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] private GameObject _text;
    [SerializeField] private float _speed = 1f;

    [SerializeField] private Button _backButton;

    private void mainSceneGo()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void Update()
    {
        _text.transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    private void Start()
    {
        _backButton.onClick.AddListener(() => mainSceneGo());
    }
}