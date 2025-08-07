using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;

    private float hp = 10;
    private float hunger = 3;

    public float GetHp() { return hp; }
    public float GetHunger() { return hunger; }

    Vector3 moveVec;
    bool isMoving = false;
    bool hungerInvoked = false;
    bool hpInvoked = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3 (hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;

        bool nowMoving = moveVec != Vector3.zero;

        if (nowMoving && !isMoving)
        {
            if (!hungerInvoked)
            {
                InvokeRepeating("hungerdecrease", 5f, 5f);
                hungerInvoked = true;
            }
        }

        if (hunger < 1 && !hpInvoked)
        {
            CancelInvoke("hungerdecrease");
            InvokeRepeating("hpdecrease", 3f, 3f);
            hpInvoked = true;
        }

        isMoving = nowMoving;
    }

    public void hungerdecrease()
    {
        hunger -= 1;
        Debug.Log("Hunger: " + hunger);
    }

    public void hpdecrease()
    {
        hp -= 1;
        Debug.Log("HP: " + hp);
    }
}
