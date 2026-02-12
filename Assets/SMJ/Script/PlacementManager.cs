using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [Header("References")]
    [SerializeField] private GameObject _inventoryGo;
    [SerializeField] private Inventory _inventory;

    [Header("Settings")]
    ///<summary>배치 가능 거리</summary>
    [SerializeField] private float _placeRange = 10.0f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _CraftingTableLayer;
    [SerializeField] private string _prefabPath = "Prefabs/";

    [Header("Materials")]
    [SerializeField] private Material _previewMatGreen;
    [SerializeField] private Material _previewMatRed;

    private GameObject _currentGhostObj;
    private PlaceableItem _currentItem;
    private int _currentItemIndex;
    private bool _isPlacementMode = false;

    private Vector3 _debugCenter;
    private Vector3 _debugSize;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!_isPlacementMode) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cancelPlacement();
            return;
        }

        updateGhostPosition();
    }

    public void BeginPlacement(PlaceableItem item, int index)
    {
        string path = _prefabPath + (item.Data as PlaceableItemData).PrefabName;
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError($"[PlacementManager] Prefab not found at path: {path}");
            return;
        }

        _currentItem = item;
        _currentItemIndex = index;
        _isPlacementMode = true;
        
        if (index >= _inventory.QuickSlotCount)
        {
            _inventoryGo.GetComponent<UIAnimator>().Close();
        }


        if (_currentGhostObj != null) Destroy(_currentGhostObj);
        _currentGhostObj = Instantiate(prefab);

        prepareGhost(_currentGhostObj);
    }

    private void prepareGhost(GameObject ghost)
    {
        Collider[] colliders = ghost.GetComponentsInChildren<Collider>();
        foreach (var col in colliders) col.enabled = false;
    }

    private void updateGhostPosition()
    {
        if (_currentGhostObj == null)
        {
            Debug.LogError("[PlacementManager] Current ghost object is null.");
            _isPlacementMode = false;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _placeRange, _groundLayer))
        {
            _currentGhostObj.SetActive(true);
            _currentGhostObj.transform.position = hit.point;

            Bounds bounds = getVisualBounds(_currentGhostObj);

             float difference = hit.point.y - bounds.min.y;

            _currentGhostObj.transform.position += Vector3.up * difference;

            _currentGhostObj.transform.rotation = Quaternion.identity;

            bool isValid = checkValidity(hit.point);
            updateGhostColor(isValid);

            if (Input.GetMouseButtonDown(1) && isValid)
            {
                placeObject(_currentGhostObj.transform.position);
            }
        }
        else
        {
            _currentGhostObj.SetActive(false);
        }
    }

    private Bounds getVisualBounds(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            return new Bounds(target.transform.position, Vector3.zero);
        }

        Bounds combinedBounds = renderers[0].bounds;
        bool hasBounds = false;

        foreach (var render in renderers)
        {
            if (render is ParticleSystemRenderer || render is TrailRenderer || render is LineRenderer)
                continue;

            if (render.GetComponent<RectTransform>() != null) continue;

            if (!hasBounds)
            {
                combinedBounds = render.bounds;
                hasBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(render.bounds);
            }
        }

        if (!hasBounds)
        {
            return new Bounds(target.transform.position, Vector3.zero);
        }

        return combinedBounds;
    }

    private bool checkValidity(Vector3 center)
    {
        Bounds visualBounds = getVisualBounds(_currentGhostObj);

        float halfHeight = visualBounds.size.y * 0.5f;
        Vector3 checkPos = center + Vector3.up * halfHeight;

        Vector3 halfExtents = (visualBounds.size * 0.5f) * 0.9f;

        _debugCenter = checkPos;
        _debugSize = halfExtents * 2;

        LayerMask combinedLayer = _obstacleLayer | _CraftingTableLayer;

        Collider[] hits = Physics.OverlapBox(checkPos, halfExtents, Quaternion.identity, combinedLayer);
        return hits.Length == 0;
    }

    private void updateGhostColor(bool isValid)
    {
        Renderer[] renderers = _currentGhostObj.GetComponentsInChildren<Renderer>();
        Material targetMat = isValid ? _previewMatGreen : _previewMatRed;

        foreach (var r in renderers)
        {
            r.material = targetMat;
        }
    }
    
    private void placeObject(Vector3 position)
    {
        string path = _prefabPath + (_currentItem.Data as PlaceableItemData).PrefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        Instantiate(prefab, position, Quaternion.identity);

        endPlacement();
    }

    private void cancelPlacement()
    {
        _isPlacementMode = false;
        if (_currentGhostObj != null) Destroy(_currentGhostObj);
        _currentItem = null;
        _currentItemIndex = -1;
    }

    private void endPlacement()
    {
        _currentItem.OnPlaced(_currentItemIndex);

        _isPlacementMode = false;
        if (_currentGhostObj != null) Destroy(_currentGhostObj);

        if (_currentItemIndex >= _inventory.QuickSlotCount)
        {
            _inventoryGo.GetComponent<UIAnimator>().Show();
        }

        _currentItem = null;
        _currentItemIndex = -1;
    }

    private void OnDrawGizmos()
    {
        if (_isPlacementMode)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_debugCenter, _debugSize);
        }
    }
}
