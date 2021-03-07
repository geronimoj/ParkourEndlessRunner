using UnityEngine;
/// <summary>
/// Catches animation events for the Player script.
/// Requires Player class in parent script
/// </summary>
public class EventCatcher : MonoBehaviour
{
    /// <summary>
    /// Stores a reference to the Player so we can call the corresponding event functions on it
    /// </summary>
    private Player _p;
    /// <summary>
    /// Gets a reference to the player. The player is expected to be in a parent object
    /// </summary>
    private void Awake()
    {
        _p = GetComponentInParent<Player>();
        //Log an error into the console if we can't find the Player script
        if (_p == null)
            Debug.LogError("Player class not found in parent objects! Called from: " + this.name);
    }
    /// <summary>
    /// Calls corresponding function on Player for Animation Event GetUp
    /// </summary>
    private void GetUp()
    {
        _p.GetUp();
    }

    /// <summary>
    /// Calls corresponding function on Player for Animation Event EnterSlide
    /// </summary>
    private void EnterSlide()
    {
        _p.EnterSlide();
    }

    /// <summary>
    /// Calls corresponding function on Player for Animation Event ReleaseCamera
    /// </summary>
    private void ReleaseCamera()
    {
        _p.ReleaseCamera();
    }

    /// <summary>
    /// Calls corresponding function on Player for Animation Event ExitVault
    /// </summary>
    private void ExitVault()
    {
        _p.ExitVault();
    }

    /// <summary>
    /// Calls corresponding function on Player for Animation Event EnterAnim
    /// </summary>
    private void EnterAnim()
    {
        _p.EnterAnim();
    }
}
