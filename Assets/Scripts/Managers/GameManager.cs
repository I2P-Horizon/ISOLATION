using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public PlayerController player;
    [SerializeField] public GameObject profileUI;
    [SerializeField] public CinemachineVirtualCamera virtualCamera;

    protected override void Awake()
    {
        base.Awake();
    }

  
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UIManager.Instance.ToggleUI(profileUI);
        }
        else if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            DataManager.Instance.SavePlayerData();
            Debug.Log("저장완료");
        }
     
    }
}
