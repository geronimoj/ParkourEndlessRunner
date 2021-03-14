using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Contains a function for swapping the scenes
/// </summary>
public class SwapScene : MonoBehaviour
{
    /// <summary>
    /// Swaps the scene to the scene with the given name
    /// </summary>
    /// <param name="sceneName">The name of the scene to swap too</param>
    public void Swap(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        //When a scene is swapped, reset time scale just in case
        Time.timeScale = 1;
    }
    /// <summary>
    /// Loads a scene without deleting the previous scene
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}
