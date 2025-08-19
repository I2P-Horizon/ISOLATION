using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Beach
{
    public GameObject sand;

    private MapScale mapScale = new MapScale();
    private MapHeight mapHeight = new MapHeight();

    /// <summary>
    /// 모래는 따로 쌓기
    /// </summary>
    /// <param name="finalDist"></param>
    /// <returns></returns>
    public int CalculateSandLayers(float finalDist)
    {
        if (finalDist > mapScale.total / 2f - mapScale.beach)
        {
            float beachDepth = mapScale.total / 2f - finalDist;
            float t = Mathf.Clamp01(beachDepth / mapScale.beach);
            return (t > 0.5f) ? 2 : 1;
        }

        else return 2;
    }

    /// <summary>
    /// 모래 블록 생성
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="layers"></param>
    /// <param name="sandScale"></param>
    public void PlaceSand(Vector3 origin, int x, int z, int layers, Vector3 sandScale, Transform sandParent)
    {
        float y = mapHeight.sandHeight + (layers - 1) * mapScale.block.y;
        Vector3 sandPos = origin + new Vector3(x * mapScale.block.x, y, z * mapScale.block.z);
        GameObject sandBlock = MonoBehaviour.Instantiate(sand, sandPos, Quaternion.identity, sandParent);
        sandBlock.transform.localScale = sandScale;
    }
}