using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
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

    public Button s_SaveButton;
    public Button s_BackButton;

    private bool isConversion = false;

    private void NewStart()
    {
        HUD.SetActive(false);
        OVERLAY.SetActive(false);
        StartCoroutine(Loading.Instance.LoadGameScene());
    }

    private void Settings()
    {
        if (isConversion) return;
        StartCoroutine(MoveCameraSmoothly(new Vector3(3.35f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f)));
    }

    private void Back()
    {
        if (isConversion) return;
        StartCoroutine(MoveCameraSmoothly( Vector3.zero, Quaternion.Euler(0f, 0f, 0f)));
    }

    private float moveDuration = 1f;

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

    private void Exit()
    {
        Application.Quit();
    }

    private IEnumerator StartEffect()
    {
        mainPanel.GetComponent<UIAnimator>().Close();
        StartCoroutine(Fade.Instance.FadeIn(Color.white));
        yield return new WaitForSeconds(0.5f);
        mainPanel.GetComponent<UIAnimator>().Show();
    }

    private void Start()
    {
        StartCoroutine(StartEffect());
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

        newGameButton.onClick.AddListener(() => NewStart());
        settingButton.onClick.AddListener(() => Settings());
        exitButton.onClick.AddListener(() => Exit());
        s_BackButton.onClick.AddListener(() => Back());

        mainCamera.gameObject.transform.position = new Vector3(0,0,0);
        mainCamera.gameObject.transform.rotation = Quaternion.Euler(0,0,0);
    }
}