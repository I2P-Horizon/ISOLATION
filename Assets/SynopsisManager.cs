using System.Collections;
using UnityEngine;

public class SynopsisManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float duration = 5f;

    [SerializeField] private GameObject player;
    [SerializeField] private Animator playerMotion;

    private Fade fade;

    /// <summary>
    /// 카메라 무빙 씬
    /// </summary>
    /// <returns></returns>
    private IEnumerator Scene1()
    {
        mainCamera.transform.position = new Vector3(-15.29f, 15f, 17.7f);
        mainCamera.transform.rotation = Quaternion.Euler(26f, 135f, 0f);
        playerMotion.Play("sit");

        if (fade != null)
        {
            // 이동과 동시에 페이드 아웃 시작
            StartCoroutine(fade.FadeIn(Color.black));
        }

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

        if (fade != null)
        {
            // 이동하면서 동시에 FadeIn 코루틴 실행
            StartCoroutine(fade.FadeOut(Color.black));
        }

        StartCoroutine(Scene2());
    }

    /// <summary>
    /// 플레이어 의자
    /// </summary>
    /// <returns></returns>
    private IEnumerator Scene2()
    {
        StartCoroutine(fade.FadeIn(Color.black));
        mainCamera.transform.position = new Vector3(0.258f, 4.056f, 4.052f);
        mainCamera.transform.rotation = Quaternion.Euler(4.021f, 135f, 0f);
        yield return new WaitForSeconds(3f);
        StartCoroutine(Scene3());
    }

    /// <summary>
    /// 플레이어 의자에서 선글라스 올림
    /// </summary>
    /// <returns></returns>
    private IEnumerator Scene3()
    {
        yield return null;
        playerMotion.SetBool("Scene2", true);
        yield return new WaitForSeconds(5f);

        playerMotion.SetBool("Walking", true);
        yield return StartCoroutine(MovePlayerForward(1.5f, 1));

        playerMotion.SetBool("Idle", true);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Scene4());
    }

    private IEnumerator Scene4()
    {
        yield return null;
        mainCamera.transform.position = new Vector3(2.5f, 4.3f, 6.6f);
        mainCamera.transform.rotation = Quaternion.Euler(3.8f, 213f, 0f);

        StartCoroutine(MoveCameraForwardSmooth(0.5f, 5f));
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

        // 마지막 위치 보정
        mainCamera.transform.position = endPos;
    }


    private void Start()
    {
        fade = FindFirstObjectByType<Fade>();
        StartCoroutine(Scene1());
    }
}