using UnityEngine;
/// <summary>
/// Enables the ragdoll component on any objects that enter the trigger.
/// OBSELETE
/// </summary>
public class RagdollTrigger : MonoBehaviour
{
    /// <summary>
    /// Ragdolls  an object that enters the trigger and is ragdollable
    /// </summary>
    /// <param name="other">The collider of the object that entered the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        Ragdoll ragdoll = other.GetComponentInParent<Ragdoll>();
        //Make sure we have a valid ragdoll target
        if (ragdoll != null)
            ragdoll.RagdollOn = true;
    }
}
