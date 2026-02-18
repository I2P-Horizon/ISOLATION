using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour
{
    [SerializeField] private energy _energy;

    public GameObject ui;
    bool Trigger = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Trigger)
        {
            ui.GetComponent<UIAnimator>().Show();
            UIManager.Instance.ui.alpha = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Trigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Trigger = false;
        }
    }

    public void close()
    {
        if (!_energy.isFilling)
        {
            ui.GetComponent<UIAnimator>().Close();
            UIManager.Instance.ui.alpha = 1;
        }
    }
}
