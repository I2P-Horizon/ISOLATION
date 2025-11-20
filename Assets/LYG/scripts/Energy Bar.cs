using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private float slidervalue;
    private float timer;
    private float maxtime;
    public energy energycs;
    [SerializeField]
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = 100.0f;
        slider.minValue = 0.1f;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (energycs.interaction == true)
        {
            maxtime = 100.0f;
            slider.value = maxtime;
            
            energycs.interaction = false;
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if(maxtime >= 0.1f)
            {
                timer += Time.deltaTime;
                maxtime -= Time.deltaTime;
                
                if(timer >= 1.0f)
                {
                    Debug.Log("예지의 눈 사용");
                    slider.value -= 1.0f;
                    maxtime = slider.value;
                    timer = 0.0f;
                }
            }

            else
            {
                Debug.Log("예지의 눈 에너지 부족");
            }
        }
    }
}
