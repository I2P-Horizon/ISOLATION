using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, ICycleListener
{
    float timer = 0;
    int count = 0;

    public GameObject go;
    public GameObject goInstance;

    public GameObject tombStone;
    public GameObject tombStoneInstance;

    private void Awake()
    {
        TimeManager.instance.Register(this);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1 && count <= 3)
        {
            goInstance = Instantiate(go);
            goInstance.transform.position = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
            timer = 0;

            count++;
        }
    }
    public virtual void OnCycleChanged(bool isNight)
    {
        if (isNight)
        {
            for (int i = 0; i < 5; i++)
            {
                tombStoneInstance = Instantiate(tombStone);
                tombStone.transform.position = new Vector3(Random.Range(-20, 20), 10, Random.Range(-20, 20));
            }
        }
    }
}