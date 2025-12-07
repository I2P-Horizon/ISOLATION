using UnityEngine;
using UnityEngine.UI;

public class PauseUI : Overlay
{
    [SerializeField] private GameObject _pauseUI;

    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _exitButton;

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_pauseUI.activeSelf) Show();
            else Close();
        }
    }

    private void Start()
    {
        _continueButton.onClick.AddListener(Close);
        //_settingButton.onClick.AddListener();
        _exitButton.onClick.AddListener(() => GameManager.Instance.SceneChange("MainScene"));

        _animator = _pauseUI.GetComponent<UIAnimator>();
    }
}