using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;

// Ÿ�Ӷ��ο� ī�޶� ���ε�
public class TimeLineCameraBinder : MonoBehaviour
{
    [SerializeField] PlayableDirector timelineDirector;

    private GameObject mainCamera;

    private void Start()
    {
        // ī�޶� ��������
        mainCamera = Camera.main.gameObject;

        if(timelineDirector == null || mainCamera == null)
        {
            Debug.Log("PlayableDirector �Ǵ� Main Camera ����");
            return;
        }

        TimelineAsset timelineAsset = timelineDirector.playableAsset as TimelineAsset;

        // Ÿ�Ӷ��� �� Ʈ�� �� CinemachineTrack �� ã�� ī�޶� ���ε�
        foreach(var track in timelineAsset.GetOutputTracks())
        {
            if(track is CinemachineTrack)
            {
                timelineDirector.SetGenericBinding(track, mainCamera.GetComponent<CinemachineBrain>());
                Debug.Log("Cinemachine Brain ���ε� �Ϸ�");
            }
        }
    }
}
