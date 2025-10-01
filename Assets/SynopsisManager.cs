using System.Collections;
using UnityEngine;

public class SynopsisManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float duration = 5f;

    [SerializeField] private Animator playerMotion;

    private Fade fade;

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

    private IEnumerator Scene2()
    {
        StartCoroutine(fade.FadeIn(Color.black));
        mainCamera.transform.position = new Vector3(0.258f, 4.056f, 4.052f);
        mainCamera.transform.rotation = Quaternion.Euler(4.021f, 135f, 0f);
        yield return new WaitForSeconds(3f);
        StartCoroutine(Scene3());
    }

    private IEnumerator Scene3()
    {
        yield return null;
        playerMotion.SetBool("Scene2", true);
    }

    private void Start()
    {
        fade = FindFirstObjectByType<Fade>();
        StartCoroutine(Scene1());
    }
}