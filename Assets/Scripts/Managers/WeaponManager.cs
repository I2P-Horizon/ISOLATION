using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
                    WeaponManager : �������� �� ���⺰ �ִϸ��̼� ����

            - SetWeapon() : ���� ���� ����
 */

public class WeaponManager : Singleton<WeaponManager>
{
    #region ** Serialized Fields **
    [SerializeField] Transform weaponTransform;             // ���Ⱑ ������ ��ġ
    #endregion

    #region ** Fields **
    private GameObject myWeaponGo;                      // �������� ���� ������Ʈ
    private WeaponType myWeaponType;                    // �������� ������ Ÿ��
    public Weapon currentWeapon;

    readonly private int hashWeaponType = Animator.StringToHash("WeaponType");
    #endregion 


    // ���� Ÿ�Ժ� WeaponType �Ķ���Ͱ� 
    public int CurWeaponType
    {
        get => GameManager.Instance.player.Anim.GetInteger(hashWeaponType);
        set => GameManager.Instance.player.Anim.SetInteger(hashWeaponType, value);
    }

    private void Start()
    {
        InitWeapon();
    }

    // ���� ���� ����
    public void InitWeapon()
    {
        currentWeapon = weaponTransform.GetComponentInChildren<Weapon>();          // ���� �������� ���� ��������
        
        // ���Ⱑ ���� ��
        if(currentWeapon == null)
        {
            // �Ǽ� ���� ����
            ResourceManager.Instance.LoadWeaponPrefab("Punch.prefab", prefab =>
            {
                if (prefab != null)
                {
                    // ������ ����
                    GameObject newWeapon = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, weaponTransform);
                    newWeapon.transform.localPosition = prefab.transform.localPosition;
                    newWeapon.transform.localRotation = prefab.transform.localRotation;

                    // ���� ����
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

        // ���� ���� �ִϸ��̼Ǽ���
        CurWeaponType = (int)myWeaponType;
    }

    // ���� ���� ����(�⺻�� Punch)
    public void SetWeapon(string type = "None", string weapon = "Punch")
    {
        if(Enum.TryParse(type, out WeaponType result))
        {
            ResourceManager.Instance.LoadWeaponPrefab(weapon + ".prefab", prefab =>
            {
                if (prefab != null)
                {
                    currentWeapon = null;
                    // ���� ���� ����
                    Destroy(myWeaponGo);
                    // ������ ����
                    GameObject newWeapon = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, weaponTransform);
                    newWeapon.transform.localPosition = prefab.transform.localPosition;
                    newWeapon.transform.localRotation = prefab.transform.localRotation;

                    myWeaponGo = newWeapon;
                    currentWeapon = myWeaponGo.GetComponent<Weapon>();

                    // ���ο� ����� ����
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
            Debug.Log($"{type} ��(��) ��ȿ�� Ÿ���� �ƴմϴ�.");
        }
    }
}
