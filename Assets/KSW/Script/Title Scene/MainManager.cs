using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#region MainManager
public class MainManager : MonoBehaviour
{
    [Header("Main Camera Position")]
    [SerializeField] private Vector3 MainPosition = Vector3.zero;
    [SerializeField] private Vector3 MainRotation = Vector3.zero;

    [Header("Settings Camera Position")]
    [SerializeField] private Vector3 SettingsPosition = new Vector3(3.35f, 0, 0);
    [SerializeField] private Vector3 SettingsRotation = new Vector3(0f, 90f, 0f);

    [Header("Panel")]
    public GameObject mainPanel;
    public GameObject mainCamera;

    [Header("Canvas")]
    public GameObject HUD;
    public GameObject OVERLAY;

    [Header("Button")]
    public Button newGameButton;
    public Button settingButton;
    public Button exitButton;
    public Button s_BackButton;
    public Button s_SaveButton;

    public Settings settings;

    private bool isConversion = false;
    private float moveDuration = 1f;

    private void PlayButton()
    {
        StartCoroutine(Loading.Instance.LoadGameScene());
    }

    private void SettingsButton()
    {
        if (isConversion) return;
        StartCoroutine(MoveCameraSmoothly(SettingsPosition, Quaternion.Euler(SettingsRotation)));
    }

    private void ExitButton()
    {
        Application.Quit();
    }

    private void Back()
    {
        if (isConversion) return;
        StartCoroutine(MoveCameraSmoothly(MainPosition, Quaternion.Euler(MainRotation)));
    }

    private IEnumerator MoveCameraSmoothly(Vector3 targetPosition, Quaternion targetRotation)
    {
        isConversion = true;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
        isConversion = false;
    }

    

    private IEnumerator StartEffect()
    {
        mainPanel.GetComponent<UIAnimator>()?.Close();
        StartCoroutine(Fade.Instance.FadeIn(Color.black));
        yield return new WaitForSeconds(0.5f);
        mainPanel.GetComponent<UIAnimator>()?.Show();
    }

    private void Start()
    {
        StartCoroutine(StartEffect());
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

        // Main 화면 버튼
        newGameButton.onClick.AddListener(PlayButton);
        settingButton.onClick.AddListener(SettingsButton);
        exitButton.onClick.AddListener(ExitButton);
        s_BackButton.onClick.AddListener(Back);

        // Settings 초기화
        settings.Init();

        // 카메라 초기 위치
        mainCamera.transform.position = Vector3.zero;
        mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
#endregion