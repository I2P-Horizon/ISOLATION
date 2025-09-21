using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    private static Loading instance;
    public static Loading Instance => instance;

    public GameObject loadingPanel;
    public Slider loadingBar;
    public Camera mainCamera;

    public Text loadingText;
    public bool isLoading = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if (mainCamera != null) mainCamera.enabled = false;
    }

    public IEnumerator LoadGameScene()
    {
        loadingPanel.SetActive(true);
        isLoading = true;

        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        operation.allowSceneActivation = true;

        yield return null;

        WorldMapMarker worldMapMarker = null;
        while (worldMapMarker == null)
        {
            worldMapMarker = FindObjectOfType<WorldMapMarker>();
            yield return null;
        }

        while (IslandGenerator.generationProgress < 0.9f)
        {
            loadingBar.value = IslandGenerator.generationProgress;
            loadingText.text = "Loading...";
            yield return null;
        }

        StartCoroutine(worldMapMarker.DelayedRender());

        while (IslandGenerator.generationProgress < 1f)
        {
            loadingBar.value = IslandGenerator.generationProgress;
            loadingText.text = "Rendering...";
            yield return null;
        }
    }
}