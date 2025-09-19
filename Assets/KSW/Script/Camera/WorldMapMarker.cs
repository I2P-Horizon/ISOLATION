using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WorldMapMarker : MonoBehaviour
{
    [Header("참조")]
    public Transform player;
    public RectTransform mapRect;
    public RectTransform playerIcon;
    public Camera mapCamera;

    [Header("RenderTexture 설정")]
    public int textureSize = 512;

    private RenderTexture mapTexture;
    public bool isRendering = false;

    private List<GameObject> waters = new List<GameObject>();
    private List<GameObject> dirts = new List<GameObject>();

    private void Start()
    {
        mapTexture = new RenderTexture(textureSize, textureSize, 24);
        mapTexture.filterMode = FilterMode.Point;

        mapCamera.targetTexture = mapTexture;
        mapCamera.enabled = false;

        RawImage mapImage = mapRect.GetComponent<RawImage>();
        if (mapImage != null) mapImage.texture = mapTexture;
    }

    private void LateUpdate()
    {
        if (player == null || mapCamera == null || mapRect == null || playerIcon == null) return;

        Vector3 viewportPos = mapCamera.WorldToViewportPoint(player.position);
        if (viewportPos.z < 0f) return;

        Vector2 uiPos = new Vector2(
            (viewportPos.x - 0.5f) * mapRect.rect.width,
            (viewportPos.y - 0.5f) * mapRect.rect.height
        );

        playerIcon.anchoredPosition = uiPos;
    }

    public IEnumerator DelayedRender(int frameCount = 130)
    {
        isRendering = true;

        Hide();

        for (int i = 0; i < frameCount; i++)
        {
            mapCamera.Render();
            yield return new WaitForEndOfFrame();
        }

        Restore();

        isRendering = false;
    }

    private void Hide()
    {
        waters.Clear();
        dirts.Clear();

        waters = FindObjectsOfType<Transform>()
            .Where(t => t.gameObject.name.EndsWith("_Water"))
            .Select(t => t.gameObject)
            .ToList();

        dirts = FindObjectsOfType<Transform>()
            .Where(t => t.gameObject.name.EndsWith("_Dirt"))
            .Select(t => t.gameObject)
            .ToList();

        foreach (var w in waters) w.SetActive(false);
        foreach (var d in dirts) d.SetActive(false);
    }

    private void Restore()
    {
        foreach (var w in waters) if (w != null) w.SetActive(true);
        foreach (var d in dirts) if(d != null) d.SetActive(true);

        waters.Clear();
        dirts.Clear();
    }
}