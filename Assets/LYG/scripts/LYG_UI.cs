using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LYG_UI : MonoBehaviour
{
    [SerializeField] private Slider hpslider;
    [SerializeField] private Slider hungerslider;
    [SerializeField] private player Player;


    private bool isHPClick;
    private bool isHungerClick;

    private float dotTime = 1f;
    private float currentHpTime = 0f;
    private float currentHungerTime = 0f;

    void Start ()
    {
        currentHpTime = dotTime;
        currentHungerTime = dotTime;
        hpslider.value = Player.GetHp();
        hungerslider.value = Player.GetHunger();
    }
    void Update()
    {
        hpslider.value = Player.GetHp();
        hungerslider.value = Player.GetHunger();

        if (isHPClick)
        {
            currentHpTime -= Time.deltaTime;

            if (currentHpTime <= 0)
            {
                hpslider.value -= Time.deltaTime;

                if (currentHpTime <= -1f)
                {
                    currentHpTime = dotTime;
                }
            }
        }

        if (isHungerClick)
        {
            currentHungerTime -= Time.deltaTime;

            if (currentHungerTime <= 0)
            {
                hungerslider.value -= Time.deltaTime;

                if (currentHungerTime <= -1f)
                {
                    currentHungerTime = dotTime;
                }
            }
        }
    }

    public void HPButton()
    {
        isHPClick = true;
    }

    public void HungerButton()
    {
        isHungerClick = true;
    }
}
