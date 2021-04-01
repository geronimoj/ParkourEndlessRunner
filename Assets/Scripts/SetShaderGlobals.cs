using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sets the Global/uniform values for all shaders
/// </summary>
public class SetShaderGlobals : MonoBehaviour
{
    /// <summary>
    /// The distance at which the world begins to bend into the ground
    /// </summary>
    [Tooltip("The distance at which the world begins to bend into the ground")]
    [SerializeField]
    private float _dropDist = 70;
    /// <summary>
    /// The distance at which runner vision begins lerping to colour Runner (Runner is defined on indiviual shaders)
    /// </summary>
    [Tooltip("The distance at which runner vision begins lerping to colour Runner (Runner is defined on indiviual shaders)")]
    [SerializeField]
    private float _runnerBegin = 25;
    /// <summary>
    /// The distance at which runner vision reaches colour Runner (Runner is defined on indiviual shaders)
    /// </summary>
    [Tooltip("The distance at which runner vision reaches colour Runner (Runner is defined on indiviual shaders)")]
    [SerializeField]
    private float _runnerEnd = 20;
    /// <summary>
    /// Set the uniform values for shaders
    /// </summary>
    void Awake()
    {   //Set the uniforms
        SetUniforms();
#if !UNITY_EDITOR
        //Delete this as we don't need it anymore, just don't delete it if we are in the unity editor otherwise we can't debug
        Destroy(this);
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// If we are in the unity editor, keep updating the shader values for debugging purposes
    /// </summary>
    void Update()
    {   //Set the uniforms
        SetUniforms();
    }
#endif
    /// <summary>
    /// Set the uniform/global values of all shaders
    /// </summary>
    private void SetUniforms()
    {
        Shader.SetGlobalFloat("_DropEnd", _dropDist);
        Shader.SetGlobalFloat("_RunnerStart", _runnerBegin);
        Shader.SetGlobalFloat("_RunnerEnd", _runnerEnd);
    }
}
