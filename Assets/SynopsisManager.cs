using System.Collections;
using UnityEngine;

public class SynopsisManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float duration = 5f;

    [SerializeField] private GameObject player;
    [SerializeField] private Animator playerMotion;

    [SerializeField] private GameObject video;

    /// <summary>
    /// 장면
    /// </summary>
    /// <returns></returns>
    private IEnumerator CutScene()
    {
        ChangePosition(mainCamera.gameObject, -15.29f, 15f, 17.7f);
        ChangeRotation(mainCamera.gameObject, 26f, 135f, 0f);

        ChangePosition(player, 0.9504719f, 2.277f, 1.199228f);
        ChangeRotation(player, 0f, 0f, 0f);

        playerMotion.Play("sit");

        StartCoroutine(Fade.Instance.FadeIn(Color.black));

        float timer = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Vector3 endPos = startPos + mainCamera.transform.forward * moveSpeed * duration;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 카메라 이동
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, smoothT);

            yield return null;
        }

        // 마지막 위치 정확히 설정
        mainCamera.transform.position = endPos;
        StartCoroutine(Fade.Instance.FadeOut(Color.black));

        /* 페이드인 시작 */
        StartCoroutine(Fade.Instance.FadeIn(Color.black));

        /* 카메라 위치 설정 */
        ChangePosition(mainCamera.gameObject, 0.1f, 3.9f, 2.5f);
        ChangeRotation(mainCamera.gameObject, 4.02f, 135f, 0f);

        /* 3초 뒤에 선글라스 올림 */
        yield return new WaitForSeconds(3f);

        yield return null;
        playerMotion.SetBool("Sunglasses Up", true);

        /* 3초 기다린 후 앞으로 걸어감 */
        yield return new WaitForSeconds(3f);

        playerMotion.SetBool("Walking", true);
        yield return StartCoroutine(MovePlayerForward(1.5f, 1f));

        ChangePosition(mainCamera.gameObject, 2.5f, 4.5f, 6.6f);
        ChangeRotation(mainCamera.gameObject, 3.8f, 213f, 0f);

        yield return StartCoroutine(MovePlayerForward(1.5f, 1f));

        /* 배 앞 쪽에서 멈춤 */
        playerMotion.SetBool("Idle", true);
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(MoveCameraForwardSmooth(0.5f, 2.5f));

        video.SetActive(true);

        yield return new WaitForSeconds(3f);
        video.SetActive(false);

        yield return new WaitForSeconds(2f);

        /* 뒤로 넘어짐 */
        playerMotion.SetBool("fall", true);
        yield return StartCoroutine(CameraShake(1.5f, 0.2f));
        StartCoroutine(CameraShake(5f, 0.03f));
        yield return new WaitForSeconds(2f);

        ChangePosition(player, -1.6f, 2.277f, -0.64f);
        ChangeRotation(player, 0, -178, 0);

        playerMotion.Play("Crawl");

        /* 로컬 좌표 문제로 카메라 전환할 때마다 진동 강제로 off -> 변경 후 on */
        StartCoroutine(CameraShake(0f, 0f));
        ChangePosition(mainCamera.gameObject, -2.5f, 4.3f, -4.9f);
        ChangeRotation(mainCamera.gameObject, 23.8f, 5.1f, 1.7f);
        StartCoroutine(CameraShake(5f, 0.03f));

        yield return StartCoroutine(MovePlayerForward(1.5f, 0.6f));

        StartCoroutine(CameraShake(0f, 0f));
        ChangePosition(mainCamera.gameObject, -0.75f, 4f, -1.5f);
        ChangeRotation(mainCamera.gameObject, -0.6f, 163f, 0f);
        StartCoroutine(CameraShake(10f, 0.03f));

        ChangePosition(player, -0.17f, 2.277f, -3.594f);
        ChangeRotation(player, 0, -10.5f, 0);

        playerMotion.SetBool("Standing", true);
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(IncreaseFog(0.15f, 0.05f));

        playerMotion.SetBool("Sad", true);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(Fade.Instance.FadeOut(Color.black));
    }

    private void ChangePosition(GameObject obj, float x, float y, float z)
    {
        obj.transform.position = new Vector3(x, y, z);
    }

    private void ChangeRotation(GameObject obj, float x, float y, float z)
    {
        obj.transform.rotation = Quaternion.Euler(x, y, z);
    }

    private IEnumerator MovePlayerForward(float distance, float speed)
    {
        float moved = 0f;

        while (moved < distance)
        {
            float step = speed * Time.deltaTime;
            player.transform.position += player.transform.forward * step;
            moved += step;
            yield return null;
        }
    }

    private IEnumerator MoveCameraForwardSmooth(float distance, float duration)
    {
        Vector3 startPos = mainCamera.transform.position;
        Vector3 endPos = startPos + mainCamera.transform.forward * distance;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        mainCamera.transform.position = endPos;
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }

    private IEnumerator IncreaseFog(float targetDensity, float speed)
    {
        RenderSettings.fog = true; // Fog 켜기
        while (RenderSettings.fogDensity < targetDensity)
        {
            RenderSettings.fogDensity += speed * Time.deltaTime;
            yield return null;
        }
        RenderSettings.fogDensity = targetDensity; // 정확히 맞춤
    }


    private void Start()
    {
        StartCoroutine(CutScene());
    }
}