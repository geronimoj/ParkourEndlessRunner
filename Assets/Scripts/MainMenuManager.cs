using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the main menu
/// </summary>
public class MainMenuManager : Manager
{
    /// <summary>
    /// The models the player can choose from
    /// </summary>
    [Tooltip("The prefabs for the models the player can choose between")]
    [SerializeField]
    private GameObject[] _playerModels = new GameObject[0];
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
    /// The UI game object containing the buttons for rebinding controls
    /// </summary>
    [Tooltip("The UI game object containing the buttons for rebinding controls")]
    [SerializeField]
    private GameObject _rebindMenu = null;
    /// <summary>
    /// The UI that appears when opening the tutorial window
    /// </summary>
    [Tooltip("The UI that appears when opening the tutorial window")]
    [SerializeField]
    private GameObject _tutorialMenu = null;
    /// <summary>
    /// The UI that appears when swapping models
    /// </summary>
    [Tooltip("The UI that appears when swapping models")]
    [SerializeField]
    private GameObject _modelMenu = null;
    /// <summary>
    /// Makes sure everything has been assigned correctly
    /// </summary>
    private void Start()
    {   //Initialise objectes to 0 to avoid null reference exception
        _objectsToCleanUp = new GameObject[0];
        //Make sure the menus have been assigned
        Debug.Assert(_rebindMenu != null, "Rebind Menu not assigned!");
        _rebindMenu.SetActive(false);
        Debug.Assert(_modelMenu != null, "Model Menu not assigned!");
        _modelMenu.SetActive(false);
        Debug.Assert(_tutorialMenu != null, "Tutorial Menu not assigned!");
        _tutorialMenu.SetActive(false);
    }
    /// <summary>
    /// Spawns the player models
    /// </summary>
    [ContextMenu("Load Player Models")]
    public void LoadPlayerModels()
    {
        //Dissable the other menus
        _tutorialMenu.SetActive(false);
        _rebindMenu.SetActive(false);
        //Unload any player models that already exist
        UnloadPlayerModels();
        //Re-enable the model menu as UnloadPlayerModels set it to false
        _modelMenu.SetActive(true);
        //Re-size the objects to clean up array
        _objectsToCleanUp = new GameObject[_playerModels.Length];
        //Loop over the player models and create them in the scene and store them in the objects to clean up
        for (int i = 0; i < _playerModels.Length; i++)
        {
            _objectsToCleanUp[i] = Instantiate(_playerModels[i], _modelSpawnOffset + (_modelSpacing * i), Quaternion.identity);
            //Because a prefab cannot reference itself (it will instead reference the gameObject version once its been spawned)
            //We need to manually override the now instantiated prefab to reference the original prefab instead of itself.
            _objectsToCleanUp[i].GetComponent<AISelector>().SetModelInfoToPrefab(_playerModels[i]);
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
        //Disable the model menu since there are no models so no need to keep it up
        _modelMenu.SetActive(false);
    }
    /// <summary>
    /// Toggles the rebindmenu on and off. Handles if other menus should be enabled at that time
    /// </summary>
    public void ToggleRebindMenu()
    {   //Flip the rebind menus active state
        _rebindMenu.SetActive(!_rebindMenu.activeSelf);
        //Make sure other menus are set to false
        UnloadPlayerModels();
        _tutorialMenu.SetActive(false);
    }

    public void ToggleTutorial()
    {
        _tutorialMenu.SetActive(!_tutorialMenu.activeSelf);
        //Make sure other menaus are set to false
        UnloadPlayerModels();
        _rebindMenu.SetActive(false);
    }
    /// <summary>
    /// Exits the program
    /// </summary>
    public void CloseApp()
    {
        Application.Quit();
    }
}
