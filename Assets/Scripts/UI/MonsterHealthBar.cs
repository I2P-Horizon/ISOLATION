using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MonsterHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;                 // ü�¹� �����̴�

    private Monster monster;
    private Transform cam;                                  // ���� ī�޶� Transform

    private void Start()
    {
        cam = Camera.main.transform;
        monster = GetComponentInParent<Monster>();

        SetHpAmount();
    }
    private void Update()
    {
        // UI�� ī�޶� �������� �ٶ󺸵���
        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
        SetHpAmount();
    }

    // ���� ü�� ǥ��
    private void SetHpAmount()
    {
        float hpFillAmount = (float)monster.curHp / monster.maxHp;
        slider.value = hpFillAmount;
    }
}
