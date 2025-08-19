using UnityEngine;

public class ChunkCulling : MonoBehaviour
{
    public Transform[] chunks;
    public Camera mainCamera;

    void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        foreach (var chunk in chunks)
        {
            if (chunk == null) continue;

            Renderer rend = chunk.GetComponentInChildren<Renderer>();
            if (rend == null) continue;

            Bounds bounds = rend.bounds;
            bool visible = GeometryUtility.TestPlanesAABB(planes, bounds);

            chunk.gameObject.SetActive(visible);
        }
    }
}