using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 접근했을 때 상호작용 가능한 오브젝트가 수행해야 할 공통 인터페이스
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 플레이어와 상호작용 수행.
    /// context 파라미터를 통해 다양한 데이터 전달 가능.
    /// </summary>
    /// <param name="context">
    /// 상호작용 시 추가 정보를 담는 매개변수. 
    /// 필요 없을 경우 생략 가능.
    /// 사용 시 null값일 수 있으므로, 구현체에서 타입 확인 필요
    /// </param>
    void Interact(object context = null);
}
