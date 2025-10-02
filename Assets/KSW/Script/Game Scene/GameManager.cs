using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    private PlayerState playerState;

    private bool isGameOver = false;

    /// <summary>
    /// Game Over 처리
    /// </summary>
    public IEnumerator GameOver()
    {
        yield return null;
        Debug.Log("Game Over 발생");
        //UIManager.Instance?.PopUpShow(UIManager.Instance.gameOverUI);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Scene 전환
    /// </summary>
    public void SceneChange(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (playerState != null && playerState.Die && !isGameOver)
        {
            isGameOver = true;
        }
    }

    private void Start()
    {
        SceneManager.LoadScene("GameSetting", LoadSceneMode.Additive);
        Time.timeScale = 1;

        playerState = FindFirstObjectByType<PlayerState>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else Destroy(gameObject);
    }
}