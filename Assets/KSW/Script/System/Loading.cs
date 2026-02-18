using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    private static Loading instance;
    public static Loading Instance => instance;

    [SerializeField] private Slider loadingBar;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Text loadingText;
    [SerializeField] private Text tipText;
    [SerializeField] private Image fillImage;

    [SerializeField] private float interval = 5f;

    public GameObject loadingPanel;
    public bool isLoading = false;

    /// <summary>
    /// 팁 목록
    /// </summary>
    private string[] tips =
    {
        "밤이 되면 동물들이 몬스터로 변해 배회합니다. 조심하세요.",
        "예지의 눈으로 꿈의 현상을 예측해 보세요.",
        "예지의 눈을 사용하려면 조각을 모아 에너지를 채워야 합니다.",
        "인벤토리에서 아이템을 우클릭하면 사용하거나 배치할 수 있습니다.",
        "수분량이 0이 되면 체력이 서서히 감소합니다. 주의하세요.",
        "고기는 모닥불에 구워 섭취하는 것이 좋습니다.",
        "소화불량에는 파인애플이 도움이 됩니다.",
        "이온음료와 소화제 재료는 주변을 잘 살펴보세요.",
        "탈수를 막기 위해 수분량을 꾸준히 유지하세요.",
        "밤에는 예상치 못한 일이 벌어질 수 있습니다."
    };

    /// <summary>
    /// 차례대로 팁 재생
    /// </summary>
    /// <returns></returns>
    private IEnumerator Tip()
    {
        int index = 0;

        while (true)
        {
            tipText.text = "TIP: " + tips[index];
            index = (index + 1) % tips.Length;
            yield return new WaitForSeconds(interval);
        }
    }

    private void UpdateLoadingBar(float progress)
    {
        loadingBar.value = progress;

        Color startColor = new Color(0f, 0.5f, 0f);
        Color endColor = new Color(0.4f, 0f, 1f);
        fillImage.color = Color.Lerp(startColor, endColor, progress);
    }

    /// <summary>
    /// 로딩 처리
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadGameScene()
    {
        ///* 시놉시스 끝나고 페이드 인되어 실행 */
        //yield return StartCoroutine(Fade.Instance.FadeOut(Color.black));
        //SceneManager.LoadScene("Synopsis", LoadSceneMode.Additive);
        //yield return new WaitForSeconds(35f);
        //SceneManager.UnloadSceneAsync("Synopsis");
        //StartCoroutine(Fade.Instance.FadeIn(Color.black));

        yield return StartCoroutine(Fade.Instance.FadeOut(Color.black, 0.5f));
        StartCoroutine(Fade.Instance.FadeIn(Color.black, 0.5f));

        /* 로딩 패널, 로딩 활성화 */
        loadingPanel.SetActive(true);
        isLoading = true;

        /* 게임 씬 활성화하여 섬 생성 시작 */
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        operation.allowSceneActivation = true;

        yield return null;

        /* 월드 맵 객체 찾기 */
        WorldMapMarker worldMapMarker = null;
        while (worldMapMarker == null)
        {
            worldMapMarker = FindObjectOfType<WorldMapMarker>();
            yield return null;
        }

        while (IslandManager.generationProgress < 0.9f)
        {
            loadingBar.value = IslandManager.generationProgress;
            UpdateLoadingBar(IslandManager.generationProgress);
            loadingText.text = "섬으로 진입 중...";
            yield return null;
        }

        StartCoroutine(worldMapMarker.DelayedRender());

        while (IslandManager.generationProgress < 1f)
        {
            loadingBar.value = IslandManager.generationProgress;
            UpdateLoadingBar(IslandManager.generationProgress);
            loadingText.text = "캐릭터를 찾는 중...";
            yield return null;
        }
    }

    private void Start()
    {
        if (mainCamera != null) mainCamera.enabled = false;
        StartCoroutine(Tip());
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }
}