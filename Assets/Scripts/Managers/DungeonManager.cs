using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/*
                     DungeonManager - 던전 관리

                * 던전 진행도 관리
                    - 모든 일반몬스터 처치시 컷씬 후 보스몬스터 등장
                    - 보스몬스터 처치시 보상
                    - 플레이어 죽을시 귀환
 */
public class DungeonManager : Singleton<DungeonManager>
{
    [SerializeField] private Transform startingPoint;               // 플레이어 시작 위치
    [SerializeField] private Transform bossSpawnPoint;              // 보스 등장 위치
    [SerializeField] private Transform monsters;                    // 일반 몬스터 그룹
    [SerializeField] private Transform cutScenePlayerPos;           // 컷씬 출력시 플레이어 위치
    [SerializeField] private TimelineAsset[] timelineAsset;         // 타임라인 에셋
    [SerializeField] private GameObject cutSceneObj;                // 컷씬 카메라 오브젝트
    [SerializeField] private GameObject bossMonsterPrefab;          // 보스몬스터 프리팹
    [SerializeField] private GameObject portal;                     // 출구
    [Header("#UI")]
    [SerializeField] private GameObject clearUI;

    private bool bossSpawned = false;                               // 보스 등장여부
    private PlayableDirector pd;
    private BossMonster boss;

    private void Start()
    {
        pd = GetComponent<PlayableDirector>();

        // 이벤트 연결 
        pd.played += OnCutsceneStarted;
        pd.stopped += OnCutsceneEnded;

        // 플레이어 위치 설정
        GameManager.Instance.player.transform.position = startingPoint.position;
        GameManager.Instance.player.transform.rotation = Quaternion.identity;

        Spawn();
    }

    private void Update()
    {
        if (bossSpawned)
            return;

        if(monsters.childCount == 0)
        {
            SpawnBoss();
        }
    }

    // 몬스터 스폰
    private void Spawn()
    {
        for(int i = 0;i<monsters.childCount;i++)
        {
            monsters.GetChild(i).gameObject.SetActive(true);
        }

        Debug.Log("몬스터 소환 완료");
    }

    // 보스몬스터 스폰
    private void SpawnBoss()
    {
        bossSpawned = true;

        Instantiate(bossMonsterPrefab, bossSpawnPoint);

        // 보스몬스터 정보 가져오기
        boss = GetComponentInChildren<BossMonster>();
        // 보스 죽음 이벤트 등록
        boss.OnBossDied += OnBossDied;
        // 컷씬 출력
        cutSceneObj.gameObject.SetActive(false);
        pd.Play(timelineAsset[0]);

        Debug.Log("모든 몬스터 처리 완료");
    }
    
    // 보스 죽음
    private void OnBossDied()
    {
        StartCoroutine(DungeonClear());
    }

    // 던전 클리어 UI활성화
    private IEnumerator DungeonClear()
    {
        // 딜레이
        yield return new WaitForSeconds(3f);

        clearUI.SetActive(true);
    }

    // 컷씬 시작(플레이어 움직임 제어)
    private void OnCutsceneStarted(PlayableDirector director)
    {
        GameManager.Instance.player.isCutscenePlaying = true;

        // 플레이어 위치 설정
        GameManager.Instance.player.transform.position = cutScenePlayerPos.position;
        GameManager.Instance.player.transform.rotation = Quaternion.identity;

        Debug.Log("컷씬 시작");
    }

    // 컷씬 끝(플레이어 움직임 제어해제)
    private void OnCutsceneEnded(PlayableDirector director)
    {
        GameManager.Instance.player.isCutscenePlaying = false;
        Debug.Log("컷씬 끝");
    }

    // 던전 클리어 UI 닫기 및 보상획득
    public void GetRewardsAndSetActiveFalse()
    {
        clearUI.SetActive(false);
        DataManager.Instance.GetPlayerData().UseGold(-500);
        portal.SetActive(true);
    }
}
