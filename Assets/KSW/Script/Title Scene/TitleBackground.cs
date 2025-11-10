using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TitleBackground : MonoBehaviour
{
    [SerializeField] private VideoPlayer[] titleVideo;

    private int lastIndex = -1;

    private IEnumerator Title()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            yield return StartCoroutine(Fade.Instance.FadeOut(Color.black));

            foreach (var video in titleVideo) video.Stop();

            int index;
            do
            {
                index = Random.Range(0, titleVideo.Length);
            } while (index == lastIndex && titleVideo.Length > 1);

            titleVideo[index].Play();
            lastIndex = index;

            yield return new WaitForSeconds(0.05f);
            yield return StartCoroutine(Fade.Instance.FadeIn(Color.black));
        }
    }

    private void Start()
    {
        StartCoroutine(Title());
    }
}