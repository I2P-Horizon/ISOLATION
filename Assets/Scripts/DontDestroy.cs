using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 씬 변경시 유지하고자 하는 오브젝트에 부착
public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
