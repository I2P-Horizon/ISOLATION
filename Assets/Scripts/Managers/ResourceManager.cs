using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/*
                     ResourceManager

                - 에셋관리 - 반복적으로 사용된 에셋을 캐싱해서 사용      
                - LoadIcon() : 아이콘 이름으로 어드레서블에서 아이콘 로드 및 캐싱
 */

public class ResourceManager : Singleton<ResourceManager>
{ 
    private Dictionary<string, Sprite> cashedSpriteDictionary = new Dictionary<string, Sprite>();

    // 어드레서블 Sprite 경로
    private const string spriteRef = "Assets/Sprites/";

    // 어드레서블 무기 Prefab 경로
    private const string wPrefabRef = "Assets/Prefabs/WeaponPrefabs/";

    // 어드레서블 이펙트 Prefab 경로
    private const string ePrefabRef = "Assets/Prefabs/EffectPrefabs/";

    // 어드레서블 사운드 경로
    private const string soundRef = "Assets/Sounds/";

    protected override void Awake()
    {
        base.Awake();
    }

    // 아이콘 데이터 불러오기
    public void LoadIcon(string spriteName, System.Action<Sprite> onLoaded)
    {
        // 캐시에 이미 있을 때는 캐싱
        if (cashedSpriteDictionary.TryGetValue(spriteName, out var cachedSprite))
        {
            onLoaded?.Invoke(cachedSprite);
            return;
        }

        // 어드레서블에서 Sprite 로드(이름으로)
        Addressables.LoadAssetAsync<Sprite>(spriteRef + spriteName).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Sprite loadedSprite = handle.Result;
                cashedSpriteDictionary[spriteName] = loadedSprite;   // 캐시에 스프라이트 저장
                onLoaded?.Invoke(loadedSprite);
            }
            else
            {
                Debug.LogError($"다음 Sprite를 가져오는데 실패함. { spriteName }");
                onLoaded?.Invoke(null);
            }
        };
    }

    // 무기 프리팹 불러오기
    public void LoadWeaponPrefab(string prefabName, System.Action<GameObject> onLoaded)
    {
        Addressables.LoadAssetAsync<GameObject>(wPrefabRef + prefabName).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject loadedPrefab = handle.Result;
                onLoaded?.Invoke(loadedPrefab);
            }
            else
            {
                Debug.Log($"다음 Prefab을 가져오는데 실패함.{ prefabName }");
                onLoaded?.Invoke(null);
            }
        };
    }

    // 무기 프리팹 불러오기
    public void LoadEffectPrefab(string prefabName, System.Action<GameObject> onLoaded)
    {
        Addressables.LoadAssetAsync<GameObject>(ePrefabRef + prefabName + ".prefab").Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject loadedPrefab = handle.Result;
                onLoaded?.Invoke(loadedPrefab);
            }
            else
            {
                Debug.Log($"다음 Prefab을 가져오는데 실패함.{ prefabName }");
                onLoaded?.Invoke(null);
            }
        };
    }
    // 사운드 불러오기
    public void LoadSound(string clipName, System.Action<AudioClip> onLoaded)
    {
        Addressables.LoadAssetAsync<AudioClip>(soundRef + clipName + ".mp3").Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AudioClip loadedClip = handle.Result;
                onLoaded?.Invoke(loadedClip);
            }
            else
            {
                Debug.Log($"다음 Clip을 가져오는데 실패함.{clipName}");
                onLoaded?.Invoke(null);
            }
        };
    }
}