using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
                    WeaponManager : 무기전략 및 무기별 애니메이션 설정

            - SetWeapon() : 현재 무기 설정
 */

public class WeaponManager : Singleton<WeaponManager>
{
    #region ** Serialized Fields **
    [SerializeField] Transform weaponTransform;             // 무기가 생성될 위치
    #endregion

    #region ** Fields **
    private GameObject myWeaponGo;                      // 장착중인 무기 오브젝트
    private WeaponType myWeaponType;                    // 장착중인 무기의 타입
    public Weapon currentWeapon;

    readonly private int hashWeaponType = Animator.StringToHash("WeaponType");
    #endregion 


    // 무기 타입별 WeaponType 파라미터값 
    public int CurWeaponType
    {
        get => GameManager.Instance.player.Anim.GetInteger(hashWeaponType);
        set => GameManager.Instance.player.Anim.SetInteger(hashWeaponType, value);
    }

    private void Start()
    {
        InitWeapon();
    }

    // 시작 무기 세팅
    public void InitWeapon()
    {
        currentWeapon = weaponTransform.GetComponentInChildren<Weapon>();          // 현재 장착중인 무기 가져오기
        
        // 무기가 없을 때
        if(currentWeapon == null)
        {
            // 맨손 무기 생성
            ResourceManager.Instance.LoadWeaponPrefab("Punch.prefab", prefab =>
            {
                if (prefab != null)
                {
                    // 프리팹 생성
                    GameObject newWeapon = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, weaponTransform);
                    newWeapon.transform.localPosition = prefab.transform.localPosition;
                    newWeapon.transform.localRotation = prefab.transform.localRotation;

                    // 무기 설정
                    myWeaponGo = newWeapon;
                    currentWeapon = myWeaponGo.GetComponent<Weapon>();
                    myWeaponType = WeaponType.None;
                }
                else
                {
                    Debug.Log($"Failed to load prefab for item : {prefab}");
                }
            });
        }
        else
        {
            myWeaponType = currentWeapon.type;
        }

        // 현재 무기 애니메이션설정
        CurWeaponType = (int)myWeaponType;
    }

    // 현재 무기 세팅(기본값 Punch)
    public void SetWeapon(string type = "None", string weapon = "Punch")
    {
        if(Enum.TryParse(type, out WeaponType result))
        {
            ResourceManager.Instance.LoadWeaponPrefab(weapon + ".prefab", prefab =>
            {
                if (prefab != null)
                {
                    currentWeapon = null;
                    // 기존 무기 삭제
                    Destroy(myWeaponGo);
                    // 프리팹 생성
                    GameObject newWeapon = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, weaponTransform);
                    newWeapon.transform.localPosition = prefab.transform.localPosition;
                    newWeapon.transform.localRotation = prefab.transform.localRotation;

                    myWeaponGo = newWeapon;
                    currentWeapon = myWeaponGo.GetComponent<Weapon>();

                    // 새로운 무기로 설정
                    myWeaponGo = newWeapon;
                    myWeaponType = result;
                    CurWeaponType = (int)myWeaponType;
                }
                else
                {
                    Debug.Log($"Fail to load prefab for item : {prefab}");
                }
            });
        }
        else
        {
            Debug.Log($"{type} 은(는) 유효한 타입이 아닙니다.");
        }
    }
}
