using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/*
                     DungeonManager - ���� ����

                * ���� ���൵ ����
                    - ��� �Ϲݸ��� óġ�� �ƾ� �� �������� ����
                    - �������� óġ�� ����
                    - �÷��̾� ������ ��ȯ
 */
public class DungeonManager : Singleton<DungeonManager>
{
    [SerializeField] private Transform startingPoint;               // �÷��̾� ���� ��ġ
    [SerializeField] private Transform bossSpawnPoint;              // ���� ���� ��ġ
    [SerializeField] private Transform monsters;                    // �Ϲ� ���� �׷�
    [SerializeField] private Transform cutScenePlayerPos;           // �ƾ� ��½� �÷��̾� ��ġ
    [SerializeField] private TimelineAsset[] timelineAsset;         // Ÿ�Ӷ��� ����
    [SerializeField] private GameObject cutSceneObj;                // �ƾ� ī�޶� ������Ʈ
    [SerializeField] private GameObject bossMonsterPrefab;          // �������� ������
    [SerializeField] private GameObject portal;                     // �ⱸ
    [Header("#UI")]
    [SerializeField] private GameObject clearUI;

    private bool bossSpawned = false;                               // ���� ���忩��
    private PlayableDirector pd;
    private BossMonster boss;

    private void Start()
    {
        pd = GetComponent<PlayableDirector>();

        // �̺�Ʈ ���� 
        pd.played += OnCutsceneStarted;
        pd.stopped += OnCutsceneEnded;

        // �÷��̾� ��ġ ����
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

    // ���� ����
    private void Spawn()
    {
        for(int i = 0;i<monsters.childCount;i++)
        {
            monsters.GetChild(i).gameObject.SetActive(true);
        }

        Debug.Log("���� ��ȯ �Ϸ�");
    }

    // �������� ����
    private void SpawnBoss()
    {
        bossSpawned = true;

        Instantiate(bossMonsterPrefab, bossSpawnPoint);

        // �������� ���� ��������
        boss = GetComponentInChildren<BossMonster>();
        // ���� ���� �̺�Ʈ ���
        boss.OnBossDied += OnBossDied;
        // �ƾ� ���
        cutSceneObj.gameObject.SetActive(false);
        pd.Play(timelineAsset[0]);

        Debug.Log("��� ���� ó�� �Ϸ�");
    }
    
    // ���� ����
    private void OnBossDied()
    {
        StartCoroutine(DungeonClear());
    }

    // ���� Ŭ���� UIȰ��ȭ
    private IEnumerator DungeonClear()
    {
        // ������
        yield return new WaitForSeconds(3f);

        clearUI.SetActive(true);
    }

    // �ƾ� ����(�÷��̾� ������ ����)
    private void OnCutsceneStarted(PlayableDirector director)
    {
        GameManager.Instance.player.isCutscenePlaying = true;

        // �÷��̾� ��ġ ����
        GameManager.Instance.player.transform.position = cutScenePlayerPos.position;
        GameManager.Instance.player.transform.rotation = Quaternion.identity;

        Debug.Log("�ƾ� ����");
    }

    // �ƾ� ��(�÷��̾� ������ ��������)
    private void OnCutsceneEnded(PlayableDirector director)
    {
        GameManager.Instance.player.isCutscenePlaying = false;
        Debug.Log("�ƾ� ��");
    }

    // ���� Ŭ���� UI �ݱ� �� ����ȹ��
    public void GetRewardsAndSetActiveFalse()
    {
        clearUI.SetActive(false);
        DataManager.Instance.GetPlayerData().UseGold(-500);
        portal.SetActive(true);
    }
}
