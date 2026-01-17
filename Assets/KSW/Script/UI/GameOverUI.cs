using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : Overlay
{
    [SerializeField] private GameObject _gameOverUI;

    [SerializeField] private Button replayButton;
    [SerializeField] private Button mainButton;

    public IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(0.01f);
        Show();
        Time.timeScale = 0;
    }

    protected override void Awake()
    {
        replayButton.onClick.AddListener(() => GameManager.Instance.SceneChange("GameScene"));
        mainButton.onClick.AddListener(() => GameManager.Instance.SceneChange("MainScene"));

        _animator = _gameOverUI.GetComponent<UIAnimator>();
    }
}