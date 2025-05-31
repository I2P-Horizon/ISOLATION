using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    float timer = 0;
    int count = 0;

    public GameObject go;
    public GameObject goInstance;
    // Start is called before the first frame update
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
}