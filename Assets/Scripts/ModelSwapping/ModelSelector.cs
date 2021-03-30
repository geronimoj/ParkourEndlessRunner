using UnityEngine;
/// <summary>
/// Determines which model was clicked on a loads that model on the player.
/// Requires Camera component.
/// THIS IS NOW OBSELETE. THE MenuModelCamera DOES WHAT IS REQUIRED THROUGH BUTTONS.
/// </summary>
[RequireComponent(typeof(Camera))]
public class ModelSelector : MonoBehaviour
{
    /// <summary>
    /// A reference to the camera
    /// </summary>
    private Camera _camera;
    /// <summary>
    /// Gets the camera component
    /// </summary>
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    /// <summary>
    /// Checks if the user clicked on a model and, if so, swaps too said model
    /// </summary>
    private void Update()
    {   //If the player left clicks
        if (Input.GetMouseButtonDown(0)
            //Perform a raycast from the camera into the scene
            && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)
            //And it is a model
            && hit.transform.CompareTag("Model"))
        {
            AISelector aiS = hit.transform.GetComponent<AISelector>();
            //And it has an AI selector component
            if (aiS == null)
                return;
            //Swap the model to the model of the hit gameObject
            Player.modelToSwapTo = aiS.ModelInfo;
        }
    }
}
