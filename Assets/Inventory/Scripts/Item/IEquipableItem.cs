using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipableItem 
{
    // 장비 장착
    void Equip();

    // 장비 장착 해제
    void Unequip();
}
