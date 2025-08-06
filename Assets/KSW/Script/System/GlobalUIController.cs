using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlobalUIController : MonoBehaviour
{
    public static GlobalUIController Instance { get; private set; }

    [Header("반투명 배경")]
    public GameObject background;

    private float fpsUpdateInterval = 0.1f;
    private float fpsTimer = 0f;

    public void PopUpShow(GameObject ui)
    {
        ui.SetActive(true);
        background.SetActive(true);
        ui.GetComponent<UIAnimator>().Show();
    }

    public void PopUpClose(GameObject ui)
    {
        ui.GetComponent<UIAnimator>().Close();
        StartCoroutine(CloseAfterDelay(ui, 0.01f));
    }

    private IEnumerator CloseAfterDelay(GameObject ui, float delay)
    {
        yield return new WaitForSeconds(delay);
        background.SetActive(false);
    }

    void UpdateFPS()
    {
        if (GameSettings.Instance == null || GameSettings.Instance.fpsText == null) return;

        if (GameSettings.Instance.frameText == 1) GameSettings.Instance.fpsText.gameObject.SetActive(true);
        else GameSettings.Instance.fpsText.gameObject.SetActive(false);

        float fps = 1.0f / Time.unscaledDeltaTime;
        GameSettings.Instance.fpsText.text = Mathf.Ceil(fps).ToString();
    }

    private void LateUpdate()
    {
        fpsTimer += Time.unscaledDeltaTime;
        if (fpsTimer >= fpsUpdateInterval)
        {
            UpdateFPS();
            fpsTimer = 0f;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);
    }
}