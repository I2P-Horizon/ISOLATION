using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class CombineMesh : MonoBehaviour
{
    public void Combine(Transform parent, Material originalMaterial, string mergedName)
    {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();

        List<CombineInstance> combine = new List<CombineInstance>();
        foreach (var mf in meshFilters)
        {
            if (mf == null || mf.sharedMesh == null) continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combine.Add(ci);
        }

        if (combine.Count == 0) return;

        GameObject combinedObject = new GameObject(mergedName);
        combinedObject.transform.SetParent(parent.parent);
        combinedObject.transform.position = parent.position;
        combinedObject.transform.rotation = parent.rotation;
        combinedObject.transform.localScale = Vector3.one;

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray());

        MeshFilter mfCombined = combinedObject.AddComponent<MeshFilter>();
        mfCombined.sharedMesh = combinedMesh;

        MeshRenderer mrCombined = combinedObject.AddComponent<MeshRenderer>();

        mrCombined.sharedMaterial = originalMaterial;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child == combinedObject.transform) continue;
            DestroyImmediate(child.gameObject);
        }

        if (combinedObject.name.EndsWith("Water") || combinedObject.name.StartsWith("bush")) return;
        MeshCollider meshCollider = combinedObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;
        meshCollider.convex = false;

        // JSH
        if (combinedObject.name.EndsWith("_Grass") || combinedObject.name.EndsWith("_Sand"))
        {
            NavMeshSurface navMeshSurface = combinedObject.AddComponent<NavMeshSurface>();
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navMeshSurface.collectObjects = CollectObjects.Children;

            navMeshSurface.BuildNavMesh();
        }
    }
}