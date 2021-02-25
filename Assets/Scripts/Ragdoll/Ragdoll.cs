using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For toggling ragdolls with animations.
/// </summary>
[RequireComponent(typeof(Animator))]
public class Ragdoll : MonoBehaviour
{
    /// <summary>
    /// Store a reference to the animator
    /// </summary>
    private Animator animator = null;
    /// <summary>
    /// A list of the rigidbodies on the ragdoll. This is automatically calculated
    /// </summary>
    private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    private Collider[] colliders;
    /// <summary>
    /// Sets or returns the state of the ragdoll
    /// </summary>
    public bool RagdollOn
    {
        get { return !animator.enabled; }
        set
        {
            animator.enabled = !value;
            foreach (Rigidbody r in rigidbodies)
                r.isKinematic = !value;
            foreach (Collider c in colliders)
                c.enabled = value;
        }
    }
    /// <summary>
    /// Initialises the class and finds the rigidbodies
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();

        rigidbodies = GetComponentsInChildren<Rigidbody>().OfType<Rigidbody>().ToList();
        colliders = GetComponentsInChildren<Collider>();

        RagdollOn = false;
    }
#if DEBUG
    /// <summary>
    /// Toggles the ragdoll for debugging purposes.
    /// </summary>
    [ContextMenu("Toggle Ragdoll")]
    private void ToggleRagdoll()
    {
        RagdollOn = !RagdollOn;
    }
#endif
}
