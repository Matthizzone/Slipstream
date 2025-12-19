using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CutoutMaskUI : Image
{
    private Material cachedMaterial;

    public override Material materialForRendering
    {
        get
        {
            if (cachedMaterial == null)
            {
                cachedMaterial = new Material(base.materialForRendering);
                cachedMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }
            return cachedMaterial;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (cachedMaterial != null)
        {
            cachedMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
        }
    }
}
