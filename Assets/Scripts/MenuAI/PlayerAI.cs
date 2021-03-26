using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The AI that is used to display the players model.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RenderHead))]
public class PlayerAI : MonoBehaviour
{
    /// <summary>
    /// The default model to swap too. Should be the skeletons model
    /// </summary>
    [Tooltip("The default model to swap too. Should be the skeletons model")]
    [SerializeField]
    private ModelInfo defaultModel = new ModelInfo();
    /// <summary>
    /// The current model of the AI. Used for comparing the current model
    /// </summary>
    private GameObject currentModel = null;
    /// <summary>
    /// Get a reference to the head renderer to render the head
    /// </summary>
    private RenderHead _rm = null;
    /// <summary>
    /// Immediately swaps the model
    /// </summary>
    private void Start()
    {
        _rm = GetComponent<RenderHead>();
        //Swap the models
        SkinnedMeshBoneRebinder.SwapModel(defaultModel, Player.modelToSwapTo);
        //Store the swapped model
        currentModel = Player.modelToSwapTo.Model;

        _rm.UpdateMaterial();
    }
    /// <summary>
    /// Swaps the players model if they are not equal
    /// </summary>
    void Update()
    {   //If the model to swap too is valid and different, swap to it
        if (Player.modelToSwapTo.IsValid && currentModel != Player.modelToSwapTo.Model)
        {   //Swap the models
            SkinnedMeshBoneRebinder.SwapModel(defaultModel, Player.modelToSwapTo);
            //Store the swapped model
            currentModel = Player.modelToSwapTo.Model;
            //Update the material
            _rm.UpdateMaterial();
        }
    }
}
