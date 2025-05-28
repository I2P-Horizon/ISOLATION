using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/*
                        DamageTextManager - ������ ǥ��
 
                    - Ǯ������ ������ �ؽ�Ʈ ����
                    - ���� ��ġ�� ������ 
 */
public class DamageTextManager : Singleton<DamageTextManager>
{
    [SerializeField] private GameObject damageTextPrefab;               // ������ �ؽ�Ʈ ������

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
            // ������ �ؽ�Ʈ ���� �� ��Ȱ��ȭ
            GameObject obj = Instantiate(damageTextPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<TextMeshPro>());
        }
    }

    public void ShowDamage(Vector3 position, int damage)
    {
        TextMeshPro damageText = GetDamageText();

        // ��ġ ����
        damageText.transform.position = position;

        // ������ �ؽ�Ʈ ����
        damageText.text = damage.ToString();

        // Fade-out ȿ��
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
        float duration = 1f;                // fade-out ���� �ɸ��� �ð�
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

        // fade-out ��ó��
        text.gameObject.SetActive(false);
        pool.Enqueue(text);
    }
}
