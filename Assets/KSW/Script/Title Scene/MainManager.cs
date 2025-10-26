using System;
using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#region Selector
[Serializable]
public class Selector<T>
{
    /* 선택지 목록 */
    public T[] options;
    /* 선택된 항목 표시용 UI */
    public Text displayText;
    /* 선택 시 적용할 액션 */
    public Action<T> onApply;
    /* 현재 인덱스 */
    private int currentIndex = 0;

    public T GetCurrent() => options[currentIndex];

    public void Next()
    {
        currentIndex = (currentIndex + 1) % options.Length;
        Apply();
    }

    public void Prev()
    {
        currentIndex = (currentIndex - 1 + options.Length) % options.Length;
        Apply();
    }

    private void Apply()
    {
        onApply?.Invoke(GetCurrent());
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (GetCurrent() is Vector2Int res) displayText.text = $"{res.x} X {res.y}";
        else displayText.text = GetCurrent().ToString();
    }
}
#endregion

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

    private void NewStart()
    {
        HUD.SetActive(false);
        OVERLAY.SetActive(false);
        StartCoroutine(Loading.Instance.LoadGameScene());
    }

    private void SettingsScreen()
    {
        if (isConversion) return;
        StartCoroutine(MoveCameraSmoothly(SettingsPosition, Quaternion.Euler(SettingsRotation)));
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

    private void Exit()
    {
        Application.Quit();
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

        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

        // Main 화면 버튼
        newGameButton.onClick.AddListener(NewStart);
        settingButton.onClick.AddListener(SettingsScreen);
        exitButton.onClick.AddListener(Exit);
        s_BackButton.onClick.AddListener(Back);

        // Settings 초기화
        settings.Init();

        // 카메라 초기 위치
        mainCamera.transform.position = Vector3.zero;
        mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
#endregion