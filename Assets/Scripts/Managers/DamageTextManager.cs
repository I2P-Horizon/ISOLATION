using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/*
                        DamageTextManager - 데미지 표시
 
                    - 풀링으로 데미지 텍스트 재사용
                    - 지정 위치에 데미지 
 */
public class DamageTextManager : Singleton<DamageTextManager>
{
    [SerializeField] private GameObject damageTextPrefab;               // 데미지 텍스트 프리팹

    private int poolSize = 10;

    private Queue<TextMeshPro> pool = new Queue<TextMeshPro>();

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        for(int i = 0;i<poolSize; i++)
        {
            // 데미지 텍스트 생성 및 비활성화
            GameObject obj = Instantiate(damageTextPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<TextMeshPro>());
        }
    }

    public void ShowDamage(Vector3 position, int damage)
    {
        TextMeshPro damageText = GetDamageText();

        // 위치 설정
        damageText.transform.position = position;

        // 데미지 텍스트 설정
        damageText.text = damage.ToString();

        // Fade-out 효과
        StartCoroutine(FadeOut(damageText));
    }

    private TextMeshPro GetDamageText()
    {
        if(pool.Count > 0)
        {
            TextMeshPro text = pool.Dequeue();
            text.gameObject.SetActive(true);
            return text;
        }
        else
        {
            GameObject obj = Instantiate(damageTextPrefab, transform);
            return obj.GetComponent<TextMeshPro>();
        }
    }

    private IEnumerator FadeOut(TextMeshPro text)
    {
        float duration = 1f;                // fade-out 까지 걸리는 시간
        float elapsedTime = 0;
        Vector3 startPos = text.transform.position;
        Color startColor = text.color;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            text.transform.position = startPos + new Vector3(0, elapsedTime * 1.5f, 0);
            text.color = new Color(startColor.r, startColor.g, startColor.b, 1 - (elapsedTime / duration));
            yield return null;
        }

        // fade-out 후처리
        text.gameObject.SetActive(false);
        pool.Enqueue(text);
    }
}
