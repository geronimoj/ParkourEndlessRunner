using UnityEngine;
/// <summary>
/// Determines which model was clicked on a loads that model on the player.
/// Requires Camera component
/// </summary>
[RequireComponent(typeof(Camera))]
public class ModelSelector : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

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
