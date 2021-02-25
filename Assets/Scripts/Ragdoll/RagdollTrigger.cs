using UnityEngine;
/// <summary>
/// Enables the ragdoll component on any objects that enter the trigger.
/// </summary>
public class RagdollTrigger : MonoBehaviour
{
    /// <summary>
    /// Determine if a ragdollable object has entered the trigger
    /// </summary>
    /// <param name="other">The collider of the other object</param>
    private void OnTriggerEnter(Collider other)
    {
        Ragdoll ragdoll = other.GetComponentInParent<Ragdoll>();

        if (ragdoll != null)
            ragdoll.RagdollOn = true;
    }
}
