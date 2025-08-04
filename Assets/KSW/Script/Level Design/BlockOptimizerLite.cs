using UnityEngine;

[DisallowMultipleComponent]
public class BlockOptimizerLite : MonoBehaviour
{
    [Header("옵션")]
    public bool makeStatic = true;
    public bool enableGPUInstancing = true;
    public bool disableShadows = false;

    void OptimizeSelf()
    {
        if (makeStatic) gameObject.isStatic = true;

        var renderer = GetComponent<MeshRenderer>();
        if (renderer && renderer.sharedMaterial)
        {
            if (enableGPUInstancing && !renderer.sharedMaterial.enableInstancing)
                renderer.sharedMaterial.enableInstancing = true;

            if (disableShadows)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }
    }

    void Start()
    {
        OptimizeSelf();
    }
}