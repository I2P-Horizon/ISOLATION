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

    [SerializeField] private float interval = 5f;

    public GameObject loadingPanel;
    public bool isLoading = false;

    /// <summary>
    /// 팁 목록
    /// </summary>
    private string[] tips =
    {
        "예지의 눈을 사용해서 꿈의 현상을 예측하세요!",
        "밤에는 동물들이 몬스터로 변형하여 배회합니다. 조심하세요!",
        "예지의 눈을 사용하기 위해 조각을 모아 에너지를 채우세요.",
        "인벤토리 창에서 아이템을 우클릭하면 사용 또는 배치가 가능합니다.",
        "캐릭터의 수분량이 0이 되면 체력이 서서히 감소하니 주의하세요!",
        "고기는 익혀서 섭취하는 것을 권장합니다.",
        "이온음료 재료는 변형된 나무에서 획득 가능합니다.",
        "소화불량에는 파인애플이 좋을지도 모릅니다.",
        "탈수에 걸리지 않도록 수분량을 유지하세요.",
        "밤에는 무슨 일이 일어날지 모릅니다."
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
            tipText.text = "Tip: " + tips[index];
            index = (index + 1) % tips.Length;
            yield return new WaitForSeconds(interval);
        }
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

        yield return StartCoroutine(Fade.Instance.FadeOut(Color.black));
        StartCoroutine(Fade.Instance.FadeIn(Color.black));

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

        /* 로딩 값이 0.9 이전까지만 "Loading..." 표시 */
        while (IslandManager.generationProgress < 0.9f)
        {
            loadingBar.value = IslandManager.generationProgress;
            loadingText.text = "Loading...";
            yield return null;
        }

        /* 로딩 값이 0.9 이상이면 "Rendering..." 으로 변경 후 월드 맵 렌더링 */
        StartCoroutine(worldMapMarker.DelayedRender());

        while (IslandManager.generationProgress < 1f)
        {
            loadingBar.value = IslandManager.generationProgress;
            loadingText.text = "Rendering...";
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