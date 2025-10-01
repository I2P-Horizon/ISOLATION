using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RockArea
{
    private Temple temple;
    private MapObject mapObject;
    private BlockData blockData;

    public List<Vector3> RockPositions { get; private set; } = new List<Vector3>();

    public Transform Parent { get; private set; }

    public GameObject rockObject;
    [Range(0, 1)] public float spawnChance;

    private Transform rockBlockParent;
    private Transform rockObjectParent;

    public void Set(Temple temple, MapObject mapObject, BlockData blockData)
    {
        this.temple = temple;
        this.mapObject = mapObject;
        this.blockData = blockData;
    }

    public void Initialize(Transform parent)
    {
        Parent = new GameObject("RockArea").transform;
        Parent.SetParent(parent);
        RockPositions.Clear();

        rockBlockParent = new GameObject("RockBlocks").transform;
        rockBlockParent.SetParent(Parent);

        rockObjectParent = new GameObject("RockObjects").transform;
        rockObjectParent.SetParent(Parent);
    }

    public void Generate(Vector3 pos, GameObject rockBlock, GameObject rockObject)
    {
        if (temple.exists && Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(temple.pos.x, 0, temple.pos.z)) <= temple.radius)
            return;

        /* 돌 블록 생성 */
        GameObject rock = MonoBehaviour.Instantiate(rockBlock, pos, Quaternion.identity, rockBlockParent);

        /* BlockData 에서 지정한 캐시 값에 따라 블록 스케일 조정 */
        if (blockData != null && blockData.scaleCache.TryGetValue(rockBlock, out Vector3 scale))
            rock.transform.localScale = scale;
        else rock.transform.localScale = Vector3.one;

        /* 돌 영역 위치 저장 */
        RockPositions.Add(pos);

        /* RockObject 생성 확률 처리 (0, 1) */
        if (rockObject != null && Random.value <= spawnChance)
        {
            /* 돌 블록 위에 돌 오브젝트 배치 */
            Vector3 objPos = pos + Vector3.up * rock.transform.localScale.y;
            GameObject obj = MonoBehaviour.Instantiate(rockObject, objPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0), rockObjectParent);

            /* RockObject 랜덤 크기 적용 */
            /* 랜덤 크기 값은 추후 클래스 멤버로 올려 인스펙터에서 관리하도록 수정 예정. */
            float rockSize = Random.Range(0.8f, 1.3f);
            obj.transform.localScale = Vector3.one * rockSize;

            /* 맵 매니저에 등록 (아이템 배치 시 비활성화 위함) */
            if (mapObject != null) mapObject.RegisterObject(obj);
        }

        /* 돌 블록 병합 */
        /* 돌 영역에 있는 블록과 프리팹을 모두 같은 부모 오브젝트에 두면 병합 시 문제 발생. */
        /* rockBlockParent, rockObjectParent로 부모 오브젝트를 분리하여 rockBlockParent만 병합되도록 설정. */
        CombineMesh combiner = MonoBehaviour.FindAnyObjectByType<CombineMesh>();
        if (combiner != null) combiner.Combine(rockBlockParent, rockBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial, $"{Parent.name}_Rock");
    }
}