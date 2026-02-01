using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] Color effectColor = Color.red;
    
    private float DurationTime = 0.5f;
    private List<Material> targetMaterials = new List<Material>();
    private List<Color> originalColors = new List<Color>();



    void Awake()
    {
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer childRenderer in childRenderers)
        {

            Material newMaterialInstance = childRenderer.material;

            targetMaterials.Add(newMaterialInstance);

            if (newMaterialInstance.shader.name == "Shader Graphs/PredictEyesRadiusShaderGraph") continue;
            originalColors.Add(newMaterialInstance.color);
        }
    }



    /// <summary>
    /// 피격 효과 적용 함수
    /// </summary>
    public void ApplyEffect()
    {
        StartCoroutine(ApplyEffectRoutine());
    }



    IEnumerator ApplyEffectRoutine()
    {
        foreach(Material targetMaterial in targetMaterials)
        {
            targetMaterial.color = effectColor;
        }

        yield return new WaitForSecondsRealtime(DurationTime);

        for(int i = 0; i < targetMaterials.Count; i++)
            targetMaterials[i].color = originalColors[i];
    }
}
