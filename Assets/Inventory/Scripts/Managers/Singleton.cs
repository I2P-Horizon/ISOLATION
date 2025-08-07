using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        Singleton ���ʸ� Ŭ����
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
                // �ν��Ͻ��� �����ϴ��� �ѹ� �� üũ
                instance = (T)FindAnyObjectByType(typeof(T));

                // ���ٸ�
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
