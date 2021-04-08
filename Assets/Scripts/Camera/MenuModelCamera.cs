using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The camera controller for swapping models
/// </summary>
public class MenuModelCamera : MonoBehaviour
{
    /// <summary>
    /// The index of the model that should be visualised in the window
    /// </summary>
    private int _modelIndex = 0;
    /// <summary>
    /// A reference to the main menu manager to get the model spawn position and spacing
    /// </summary>
    private MainMenuManager _mmm = null;
    /// <summary>
    /// The cameras offset from each model
    /// </summary>
    [SerializeField]
    private Vector3 _cameraOffsetFromModel = new Vector3();
    /// <summary>
    /// Gets a reference to the MainMenuManager
    /// </summary>
    private void Awake()
    {
        _mmm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MainMenuManager>();
    }
    /// <summary>
    /// Sets the position of the camera
    /// </summary>
    void Update()
    {  
        transform.position = _mmm.ModelSpawnOffset + _mmm.ModelSpacing * _modelIndex + _cameraOffsetFromModel;
    }
    /// <summary>
    /// Cycles to the model on the left
    /// </summary>
    public void CycleLeft()
    {   //Decrement the model index then loop it if necessary
        _modelIndex--;
        if (_modelIndex < 0)
            _modelIndex = (int)_mmm.NumberOfPlayerModels - 1;
    }
    /// <summary>
    /// Cycles to the model on the right
    /// </summary>
    public void CycleRight()
    {   //Increment the model index then loop it if necessary
        _modelIndex++;
        if (_modelIndex >= _mmm.NumberOfPlayerModels)
            _modelIndex = 0;
    }
    /// <summary>
    /// Swaps the players model to the selected model
    /// </summary>
    public void SelectHighlightedModel()
    {
        ModelInfo mi = _mmm.GetModel(_modelIndex);
        //Make sure the model is valid before swapping to it
        if (mi.IsValid)
        {
            Player.modelToSwapTo = mi;
            //Store the index of the model
            Player.s_modelIndex = _modelIndex;
        }
        else
            Debug.LogError("Model info for model: " + mi + " is incomplete.");
    }
}
