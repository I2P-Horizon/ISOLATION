using UnityEngine;
using UnityEngine.UI;

public class StatusSystem : MonoBehaviour
{
    public Slider healthSlider;
    public Slider hungerSlider;

    void Update()
    {
        if (healthSlider != null)
            healthSlider.value = Mathf.PingPong(Time.time * 10, 100);

        if (hungerSlider != null)
            hungerSlider.value = Mathf.PingPong(Time.time * 15, 100);
    }
}