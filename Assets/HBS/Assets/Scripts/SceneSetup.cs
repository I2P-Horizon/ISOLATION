using UnityEngine;
using UnityEngine.UI;

public class SceneSetup : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.AddComponent<PlayerController>();

        Camera.main.gameObject.AddComponent<FollowCamera>().target = player.transform;

        Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent<CanvasScaler>();
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        Slider healthSlider = CreateSlider(canvas.transform, new Vector2(10, -10), "HealthSlider", Color.red);
        Slider hungerSlider = CreateSlider(canvas.transform, new Vector2(10, -50), "HungerSlider", Color.yellow);

        GameObject statusObj = new GameObject("StatusSystem");
        StatusSystem statusSystem = statusObj.AddComponent<StatusSystem>();
        statusSystem.healthSlider = healthSlider;
        statusSystem.hungerSlider = hungerSlider;
    }

    Slider CreateSlider(Transform parent, Vector2 anchoredPos, string name, Color fillColor)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent);

        RectTransform rt = sliderObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 20);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = anchoredPos;

        Image backgroundImage = sliderObj.AddComponent<Image>();
        backgroundImage.color = Color.gray;

        UnityEngine.UI.Slider slider = sliderObj.AddComponent<UnityEngine.UI.Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(sliderObj.transform);
        RectTransform fillRt = fill.AddComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0, 0);
        fillRt.anchorMax = new Vector2(1, 1);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;

        slider.fillRect = fillRt;
        slider.targetGraphic = fillImage;

        return slider;
    }
}

