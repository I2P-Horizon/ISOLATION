using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private GameObject timeValue_Day;
    [SerializeField] private GameObject timeValue_Night;

    private void Update()
    {
        if (TimeManager.Instance == null || TimeManager.Instance.RealTimePerDaySec <= 0) return;

        float currentTime = TimeManager.Instance.CurrentTime;
        bool isNight = TimeManager.Instance.IsNight;
        float dayTime = TimeManager.Instance.RealTimePerDaySec;
        float halfDay = dayTime / 2f;

        float fillDay = 0f;
        float fillNight = 0f;

        if (!isNight)
        {
            fillDay = (currentTime % halfDay) / halfDay;
            fillNight = 0f;

            if (!timeValue_Day.activeSelf) timeValue_Day.GetComponent<UIAnimator>().Show();
            if (timeValue_Night.activeSelf) timeValue_Night.SetActive(false);
        }

        else
        {
            fillNight = (currentTime % halfDay) / halfDay;
            fillDay = 0f;

            if (!timeValue_Night.activeSelf) timeValue_Night.GetComponent<UIAnimator>().Show();
            if (timeValue_Day.activeSelf) timeValue_Day.SetActive(false);
        }

        timeValue_Day.GetComponent<Image>().fillAmount = fillDay;
        timeValue_Night.GetComponent<Image>().fillAmount = fillNight;
    }
}