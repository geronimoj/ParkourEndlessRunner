using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Removes _HeadCut texture from the material instance to render the head
/// </summary>
public class RenderHead : MonoBehaviour
{
    /// <summary>
    /// On awake, render the head. This is done for the model options menu
    /// </summary>
    private void Awake()
    {
        UpdateMaterial();
    }
    /// <summary>
    /// Removes _HeadCut texture from the material instance to render the head
    /// </summary>
    public void UpdateMaterial()
    {
        SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();
        //Remove the Head cut texture so that the head renders
        foreach (SkinnedMeshRenderer m in smr)
            m.material.SetTexture("_HeadCut", null);
    }
}
