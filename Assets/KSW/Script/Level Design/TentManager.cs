using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentManager : MonoBehaviour
{
    public GameObject tentGround;
    public GameObject tentWall;

    public Vector3 tentSize = new Vector3(10, 6, 10);
    public float blockSize = 1f;

    private void Awake()
    {
        //BuildTent();
    }

    [ContextMenu("Build Tent Now")]
    public void BuildTentNow()
    {
        BuildTent();
    }

    Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
    {
        GameObject temp = Instantiate(prefab);
        Renderer rend = temp.GetComponentInChildren<Renderer>();

        if (rend == null)
        {
            DestroyImmediate(temp);
            return Vector3.one;
        }

        Vector3 originalSize = rend.bounds.size;
        DestroyImmediate(temp);

        if (originalSize == Vector3.zero) return Vector3.one;

        float yScale = originalSize.y < 0.01f ? 1f : targetSize.y / originalSize.y;

        return new Vector3(targetSize.x / originalSize.x, yScale, targetSize.z / originalSize.z);
    }

    public void BuildTent()
    {
        if (tentGround == null || tentWall == null) return;

        Transform oldTentRoot = transform.Find("Tent");
        if (oldTentRoot != null)
        {
            DestroyImmediate(oldTentRoot.gameObject);
        }

        GameObject tentRoot = new GameObject("Tent");
        tentRoot.transform.parent = this.transform;
        tentRoot.transform.localPosition = Vector3.zero;
        tentRoot.transform.localRotation = Quaternion.identity;

        int width = Mathf.RoundToInt(tentSize.x / blockSize);
        int height = Mathf.RoundToInt(tentSize.y / blockSize);
        int depth = Mathf.RoundToInt(tentSize.z / blockSize);

        Vector3 startPos = transform.position - new Vector3(tentSize.x / 2f, 0, tentSize.z / 2f);

        Vector3 groundScale = GetScaleToFit(tentGround, new Vector3(blockSize, blockSize, blockSize));
        Vector3 wallScale = GetScaleToFit(tentWall, new Vector3(blockSize, blockSize, blockSize));

        int exitZ = depth - 1;
        int exitXStart = (width / 2) - 1;
        int exitXEnd = exitXStart + 1;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 pos = startPos + new Vector3(x * blockSize + blockSize / 2f, blockSize / 2f, z * blockSize + blockSize / 2f);
                GameObject block = Instantiate(tentGround, pos, Quaternion.identity, tentRoot.transform);
                block.transform.localScale = groundScale;
            }
        }

        for (int protrudeX = exitXStart; protrudeX <= exitXEnd; protrudeX++)
        {
            for (int protrudeZ = depth; protrudeZ <= depth + 1; protrudeZ++)
            {
                Vector3 pos = startPos + new Vector3(protrudeX * blockSize + blockSize / 2f, blockSize / 2f, protrudeZ * blockSize + blockSize / 2f);
                GameObject block = Instantiate(tentGround, pos, Quaternion.identity, tentRoot.transform);
                block.transform.localScale = groundScale;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    bool isWall = x == 0 || x == width - 1 || z == 0 || z == depth - 1;

                    if (isWall && y > 0)
                    {
                        if (z == exitZ && (x >= exitXStart && x <= exitXEnd)) continue;

                        Vector3 pos = startPos + new Vector3(x * blockSize + blockSize / 2f, y * blockSize + blockSize / 2f, z * blockSize + blockSize / 2f);
                        GameObject block = Instantiate(tentWall, pos, Quaternion.identity, tentRoot.transform);
                        block.transform.localScale = wallScale;
                    }
                }
            }
        }
    }
}