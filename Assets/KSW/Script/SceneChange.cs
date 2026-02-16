using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 씬 이동 값 설정 (예: GameScene -> TentScene)
 * (Tent 프리팹에서 설정)
 * string _sceneName: "TentScene" -> TentScene으로 이동.
 * string _spawnPoint: "Tent(Inside)" -> TentScene의 스폰 지점으로 이동.
 * float _triggerDistance: 1.5f -> 트리거 오브젝트와 플레이어의 거리가 1.5f 이내 시 전환. */

/* 씬 이동 값 설정 (예: Tent -> GameScene)
 * (TentScene 에서 설정)
 * string _sceneName: "GameScene" -> GameScene으로 이동.
 * string _spawnPoint: "Tent(Outside)" -> GameScene에 배치되어 있는 Tent 프리팹 앞에서 스폰.
 * float _triggerDistance: 1.5f -> 트리거 오브젝트와 플레이어의 거리가 1.5f 이내 시 전환. */

public class SceneChange : MonoBehaviour
{
    [Tooltip("이동할 씬 이름")]
    [SerializeField] private string _sceneName;

    [Tooltip("스폰 위치 오브젝트 이름")]
    [SerializeField] private string _spawnPoint;

    [Tooltip("씬 전환 트리거 거리")]
    [SerializeField] private float _triggerDistance;

    /// <summary>
    /// 씬 전환 플래그
    /// </summary>
    private bool _isChangingScene = false;

    /// <summary>
    /// 씬 전환
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <returns></returns>
    private IEnumerator changeScene(string _sceneName)
    {
        _isChangingScene = true;

        yield return null;

        /* 씬 전환 */
        if (!SceneManager.GetSceneByName(_sceneName).isLoaded)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            yield return op;
        }

        /* 플레이어 위치 변경 */
        GameObject spawnPoint = GameObject.Find("SpawnPoint_" + _spawnPoint);

        if (spawnPoint != null)
        {
            CharacterController controller = Player.Instance.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;

                Vector3 targetPos = spawnPoint.transform.position;
                targetPos.y += 0.1f;

                Player.Instance.transform.position = targetPos;

                yield return null;

                controller.enabled = true;
            }

            else Player.Instance.transform.position = spawnPoint.transform.position;
        }

        /* 카메라 변경 */
        changeSceneCamera(_sceneName);

        yield return StartCoroutine(Fade.Instance.FadeIn(Color.black, 1f));

        _isChangingScene = false;
    }

    /// <summary>
    /// 씬 카메라 변경
    /// </summary>
    /// <param name="sceneName"></param>
    private void changeSceneCamera(string sceneName)
    {
        /* 모든 카메라 비활성화 */
        foreach (var cam in Camera.allCameras)
        {
            if (cam.name == "Mini Map Camera") continue;
            cam.gameObject.SetActive(false);
        }

        /* 새로운 씬 가져오기 */
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded) return;

        /* 새로운 씬 메인 카메라 활성화 */
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            Camera cam = obj.GetComponentInChildren<Camera>(true);
            if (cam != null && cam.CompareTag("MainCamera"))
            {
                cam.gameObject.SetActive(true);
                break;
            }
        }
    }

    /// <summary>
    /// 플레이어의 콜라이더 반지름에 따라, 충돌 중 특정 거리 이내로 접근했을 때 씬 전환 방식으로 로직 구현
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (_isChangingScene) return;

        if (other.CompareTag("Player"))
        {
            /* 플레이어, 씬 전환 트리거 거리 계산 */
            float distance = Vector3.Distance(other.transform.position, transform.position);

            /* 플레이어가 트리거 거리 이내로 접근했을 때 씬 전환 */
            if (distance <= _triggerDistance) StartCoroutine(changeScene(_sceneName));
        }
    }
}