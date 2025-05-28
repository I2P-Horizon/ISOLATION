using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        Singleton 제너릭 클래스
 */
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                // 인스턴스가 존재하는지 한번 더 체크
                instance = (T)FindAnyObjectByType(typeof(T));

                // 없다면
                if(instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
