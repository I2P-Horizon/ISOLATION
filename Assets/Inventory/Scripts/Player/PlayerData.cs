using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
                        PlayerData

            - �÷��̾� ������ ����
            
            - �÷��̾� �����Ϳ� ���õ� ��� �Լ� ����
                - UsePortion() : ���ǻ��� ���������� ���� �ɷ�ġ ��ȭ
                - EquipItem()  : ��������� ��������� ���� �ɷ�ġ ��ȭ
                - UnequipItem(): ��������� ��������� ���� �ɷ�ġ ��ȭ
                - GetDamaged() : �ǰݴ��� ������������ �ɷ�ġ ��ȭ
                - UseGold()    : ����뿡 ���� ���� ��� ��ȭ
*/

public class PlayerData
{
    #region ** Player Status **
    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float defense;
    [SerializeField] private int gold;
    [SerializeField] private float maxSatiety;
    [SerializeField] private float curSatiety;
    #endregion

    #region ** Player Position **
    [SerializeField] private float posX;
    [SerializeField] private float posY;
    [SerializeField] private float posZ;
    #endregion

    #region ** Properties **
    public float MaxHp => maxHp;
    public float CurHp => curHp;
    public float Speed => speed;
    public float RotateSpeed => rotateSpeed;
    public float Damage => damage;
    public float Defense => defense;
    public int Gold => gold;
    public float MaxSatiety => maxSatiety;
    public float CurSatiety => curSatiety;
    public float PosX => posX;
    public float PosY => posY;
    public float PosZ => posZ;
    #endregion

    // �÷��̾� ������ �ʱ�ȭ
    public PlayerData(PlayerDataDTO dto)
    {
        this.maxHp = dto.Status.maxHp;
        this.curHp = dto.Status.curHp;
        this.speed = dto.Status.speed;
        this.rotateSpeed = dto.Status.rotateSpeed;
        this.damage = dto.Status.damage;
        this.defense = dto.Status.defense;
        this.gold = dto.Status.gold;
        this.maxSatiety = dto.Status.maxSatiety;
        this.curSatiety = dto.Status.curSatiety;

        this.posX = dto.Position.posX;
        this.posY = dto.Position.posY;
        this.posZ = dto.Position.posZ;
    }

    // PlayerData -> DTO
    public PlayerDataDTO ToDTO()
    {
        return new PlayerDataDTO
        {
            Status = new PlayerDataDTO.StatusDTO
            {
                maxHp = this.maxHp,
                curHp = this.curHp,
                speed = this.speed,
                rotateSpeed = this.rotateSpeed,
                damage = this.damage,
                defense = this.defense,
                gold = this.gold,
                maxSatiety = this.maxSatiety,
                curSatiety = this.curSatiety
            },
            Position = new PlayerDataDTO.PositionDTO
            {
                // �÷��̾� ���� ��ġ
                posX = this.posX,
                posY = this.posY,
                posZ = this.posZ
            }
        };
    }

    // ���� ȸ��
    public void UsePortion(float value, string type)
    {
        switch(type)
        {
            case "Health":
                curHp = Mathf.Min(curHp + value, maxHp);
                break;
        }
    }

    // ��� ����
    public void EquipItem(float value, string type)
    {
        switch(type)
        {
            case "Weapon":
                damage += value;
                break;
            case "Armor":
                defense += value;
                break;
        }
    }

    // ��� ����
    public void UnequipItem(float value, string type)
    {
        switch(type)
        {
            case "Weapon":
                damage -= value;
                break;
            case "Armor":
                defense -= value;
                break;
        }
    }

    // ������ ����
    public void GetDamaged(float damage)
    {
        Debug.Log(damage + "��ŭ ������ ����");
        curHp -= damage;
        if (curHp <= 0)
            curHp = 0;
    }

    // ������ ���� (���� ���, ���� ���� ��)
    public bool IncreaseSatiety(float value)
    {
        // �������� ���� �������� false ��ȯ (��� ����)
        if (curSatiety >= maxSatiety)
            return false;

        curSatiety = Mathf.Min(curSatiety + value, maxSatiety);
        return true;
    }
}
