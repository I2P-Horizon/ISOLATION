using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public static EnergyBar Instance { get; private set; }

    public energy energycs;
    public Slider slider;

    //private float slidervalue;
    //private float timer;
    //private float maxtime;

    private float maxEnergy = 10f;
    private float currentEnergy;
    private float smoothSpeed = 10f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = maxEnergy;
        slider.minValue = 0f;
        currentEnergy = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (energycs.interaction == true)
        //{
        //    maxtime = 100.0f;
        //    slider.value = maxtime;

        //    energycs.interaction = false;
        //}

        //if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        //{
        //    if (maxtime >= 0.1f)
        //    {
        //        timer += Time.deltaTime;
        //        maxtime -= Time.deltaTime;

        //        if (timer >= 1.0f)
        //        {
        //            Debug.Log("예지의 눈 사용");
        //            slider.value -= 1.0f;
        //            maxtime = slider.value;
        //            timer = 0.0f;
        //        }
        //    }

        //    else
        //    {
        //        Debug.Log("예지의 눈 에너지 부족");
        //    }
        //}

        //slider.value = Mathf.MoveTowards(slider.value, currentEnergy, smoothSpeed * Time.deltaTime);
    }
}