using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapMarker : MonoBehaviour
{
    [Header("ÂüÁ¶")]
    public Transform player;
    public RectTransform mapRect;
    public RectTransform playerIcon;
    public Camera mapCamera;

    public bool isRender = false;

    private RenderTexture mapTexture;

    public IEnumerator DelayedRender()
    {
        isRender = true;
        for (int i = 0; i < 130; i++)
        {
            mapCamera.Render();
            yield return new WaitForEndOfFrame();
        }
        isRender = false;
    }

    void LateUpdate()
    {
        if (player == null || mapCamera == null || mapRect == null || playerIcon == null) return;

        Vector3 viewportPos = mapCamera.WorldToViewportPoint(player.position);

        if (viewportPos.z < 0f) return;

        Vector2 uiPos = new Vector2((viewportPos.x - 0.5f) * mapRect.rect.width, (viewportPos.y - 0.5f) * mapRect.rect.height);
        playerIcon.anchoredPosition = uiPos;
    }

    void Start()
    {
        mapTexture = new RenderTexture(512, 512, 24);
        mapTexture.filterMode = FilterMode.Point;

        mapCamera.targetTexture = mapTexture;

        RawImage mapImage = mapRect.GetComponent<RawImage>();
        mapCamera.enabled = false;
        if (mapImage != null) mapImage.texture = mapTexture;
    }
}