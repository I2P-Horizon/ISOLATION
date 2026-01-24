using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeTextUI : MonoBehaviour
{
    [SerializeField] private Text _dayText;
    [SerializeField] private Text _timeText;

    private int _currentDay = 0;
    private bool _morning = true;

    private void Update()
    {
        if (TimeManager.Instance == null) return;

        /* TimeManager 읽기 전용 CurrentTime 참조 */
        float currentTime = TimeManager.Instance.CurrentTime;
        float daySeconds = TimeManager.Instance.RealTimePerDaySec;

        /* 시간 계산 */
        float normalized = currentTime / daySeconds;
        float totalHours = normalized * 24f;

        int hour = Mathf.FloorToInt(totalHours);
        int minute = Mathf.FloorToInt((totalHours - hour) * 60);

        /* 아침 9시가 넘었을 때 currentDay 증가 */
        if (_morning && hour >= 9) _currentDay++;
        _morning = hour < 9;

        /* 시간 색 변경 */
        /* 21:00 ~ 23:59 -> 보라색 */
        /* 00:00 ~ 08:59 -> 보라색 */
        bool isNightTime = (hour >= 21 || hour < 9);

        if (isNightTime) _timeText.color = new Color(200f / 255f, 80f / 255f, 255f / 255f);
        else _timeText.color = Color.white;

        _dayText.text = _currentDay + "일차";
        _timeText.text = $"{hour:00}:{minute:00}";
    }
}