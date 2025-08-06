using UnityEngine;
using UnityEngine.UI;

public class WorldMapMarker : MonoBehaviour
{
    [Header("참조")]
    public Transform player;
    public RectTransform mapRect;
    public RectTransform playerIcon;
    public Camera mapCamera;

    [Header("렌더 설정")]
    public int renderWidth = 100;
    public int renderHeight = 100;

    private RenderTexture mapTexture;

    void Start()
    {
        mapTexture = new RenderTexture(renderWidth, renderHeight, 16);
        mapTexture.filterMode = FilterMode.Point;

        mapCamera.targetTexture = mapTexture;

        RawImage mapImage = mapRect.GetComponent<RawImage>();
        mapCamera.enabled = false;
        if (mapImage != null) mapImage.texture = mapTexture;
        mapCamera.Render();
    }

    void OnEnable()
    {
        if (mapCamera != null) mapCamera.Render();
    }

    void LateUpdate()
    {
        if (player == null || mapCamera == null || mapRect == null || playerIcon == null) return;

        Vector3 viewportPos = mapCamera.WorldToViewportPoint(player.position);

        if (viewportPos.z < 0f) return;

        Vector2 uiPos = new Vector2((viewportPos.x - 0.5f) * mapRect.rect.width, (viewportPos.y - 0.5f) * mapRect.rect.height);
        playerIcon.anchoredPosition = uiPos;
    }
}