using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private float timer;
    private float maxtime;
    private float targetValue; // 목표 슬라이더 값
    public energy energycs;
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private float smoothSpeed = 5f; // 부드럽게 움직이는 속도 조절

    void Start()
    {
        slider.maxValue = 100.0f;
        slider.minValue = 0.1f;
        timer = 0.0f;

        // 처음에 0으로 시작
        maxtime = 0.0f;
        targetValue = 0.0f;
        slider.value = 0.0f;
    }

    void Update()
    {
        // 에너지 충전
        if (energycs.interaction == true)
        {
            maxtime = 100.0f;
            targetValue = maxtime;
            energycs.interaction = false;
        }

        // 에너지 사용
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (maxtime >= 0.1f)
            {
                timer += Time.deltaTime;
                maxtime -= Time.deltaTime;

                if (timer >= 1.0f)
                {
                    Debug.Log("예지의 눈 사용");
                    targetValue -= 1.0f;
                    maxtime = targetValue;
                    timer = 0.0f;
                }
            }
            else
            {
                Debug.Log("예지의 눈 에너지 부족");
            }
        }

        // 슬라이더를 부드럽게 변화시킴
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
    }
}