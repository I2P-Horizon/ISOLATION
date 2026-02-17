using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class CombineMesh : MonoBehaviour
{
    /// <summary>
    /// 메쉬 병합 함수
    /// </summary>
    /// <param name="parent">부모 오브젝트</param>
    /// <param name="originalMaterial">오브젝트의 기본 머티리얼</param>
    /// <param name="mergedName">병합 후 지정할 오브젝트 이름</param>
    /// <returns></returns>
    public GameObject Combine(Transform parent, Material originalMaterial, string mergedName)
    {
        /* Gets the components that are children of a prefab. */
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (var mf in meshFilters)
        {
            if (mf == null || mf.sharedMesh == null) continue;

            /* Saving mesh and world transformation matrices to CombineInstance. */
            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combine.Add(ci);
        }

        if (combine.Count == 0) return null;

        /* 1. Create a new GameObject to contain the merged mesh. */
        /* 2. Attach the merged object to the parent object. */
        /* 3. Set the world position of the merged object to be the same as its parent */
        /* 4. Set the rotation of the merged object to the same as its parent. */
        /* 5. Initialize the scale of the merged object to 1 */
        GameObject combinedObject = new GameObject(mergedName);
        combinedObject.transform.SetParent(parent.parent);
        combinedObject.transform.position = parent.position;
        combinedObject.transform.rotation = parent.rotation;
        combinedObject.transform.localScale = Vector3.one;

        /* Merge Mesh */
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray());

        /* Add MeshFilter */
        MeshFilter mfCombined = combinedObject.AddComponent<MeshFilter>();
        mfCombined.sharedMesh = combinedMesh;

        /* Add MeshRenderer */
        MeshRenderer mrCombined = combinedObject.AddComponent<MeshRenderer>();
        mrCombined.sharedMaterial = originalMaterial;

        /* Delete existing child objects after merging. */
        /* Merge 되기 전에 있었던 블록들을 삭제하기 위함 */
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child == combinedObject.transform) continue;
            GameObject.DestroyImmediate(child.gameObject);
        }

        /* 레이어 이름 설정 */
        if (combinedObject.name.EndsWith("Grass") || combinedObject.name.EndsWith("Sand") || combinedObject.name.EndsWith("Rock"))
            combinedObject.gameObject.layer = LayerMask.NameToLayer("Ground");

        /* Add MeshCollider (Water, bush excluded) */
        /* MeshCollider 적용하지 않을 오브젝트는 아래 조건에 오브젝트의 시작/끝 이름 추가 */
        if (!combinedObject.name.EndsWith("Water") && !combinedObject.name.StartsWith("Bush"))
        {
            MeshCollider meshCollider = combinedObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = combinedMesh;
            meshCollider.convex = false;
        }

        // JSH
        /* Add NavMeshSurface (Creature Pathing.) */
        /* Merge 오브젝트의 끝 이름으로 판별하여 해당 오브젝트에만 경로 설정 */
        //if (combinedObject.name.EndsWith("_Grass") || combinedObject.name.EndsWith("_Sand"))
        //{
        //    NavMeshSurface navMeshSurface = combinedObject.AddComponent<NavMeshSurface>();
        //    navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        //    navMeshSurface.collectObjects = CollectObjects.Children;
        //    navMeshSurface.BuildNavMesh(); // Bake
        //}

        return combinedObject;
    }
}