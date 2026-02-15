using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Energysystem : MonoBehaviour
{
    private energy _energy;

    // Start is called before the first frame update
    void Start()
    {
        _energy = FindFirstObjectByType<energy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void close()
    {
        if (!_energy.isFilling) gameObject.GetComponent<UIAnimator>().Close();
    }
}
