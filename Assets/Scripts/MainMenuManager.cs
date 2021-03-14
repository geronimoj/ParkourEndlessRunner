using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the main menu
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// The models the player can choose from
    /// </summary>
    [Tooltip("The prefabs for the models the player can choose between")]
    [SerializeField]
    private GameObject[] _playerModels;
    /// <summary>
    /// The positional offset of the models
    /// </summary>
    [Tooltip("The spacing between the models from left to right")]
    [SerializeField]
    private Vector3 _modelSpawnOffset = new Vector3();
    /// <summary>
    /// The spacing between the models
    /// </summary>
    [Tooltip("The spacing between the models from left to right")]
    [SerializeField]
    private Vector3 _modelSpacing = new Vector3();
    /// <summary>
    /// An array containing the game objects that need to be deleted eventually between menu presses
    /// </summary>
    private GameObject[] _objectsToCleanUp;
    /// <summary>
    /// Spawns the player models
    /// </summary>
    [ContextMenu("Load Player Models")]
    public void LoadPlayerModels()
    {   //Re-size the objects to clean up array
        _objectsToCleanUp = new GameObject[_playerModels.Length];
        //Loop over the player models and create them in the scene and store them in the objects to clean up
        for (int i = 0; i < _playerModels.Length; i++)
        {
            _objectsToCleanUp[i] = Instantiate(_playerModels[i], _modelSpawnOffset + (_modelSpacing * i), Quaternion.identity);
        }
    }
    /// <summary>
    /// Unloads the players models
    /// </summary>
    public void UnloadPlayerModels()
    {   //Loop over the game objects and delete them
        foreach (GameObject g in _objectsToCleanUp)
            Destroy(g);
        //Set the size of the objects to be 0
        _objectsToCleanUp = new GameObject[0];
    }
}
