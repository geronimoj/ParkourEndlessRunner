using System.Collections;
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
    /// <summary>
    /// Sets or returns the state of the ragdoll
    /// </summary>
    public bool RagdollOn
    {
        get { return !animator.enabled; }
        set
        {
            animator.enabled = !value;
#if DEBUG
            ragdollOn = value; 
#endif
            foreach (Rigidbody r in rigidbodies)
                r.isKinematic = !value;
        }
    }
#if DEBUG
    /// <summary>
    /// Store the a boolean to allow for toggling of the ragdoll from the inspector.
    /// FOR DEBUGGING ONLY.
    /// </summary>
    [SerializeField]
    [Tooltip("Toggles the ragdoll state from the inspector")]
    private bool ragdollOn = false;
#endif
    /// <summary>
    /// Initialises the class and finds the rigidbodies
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();

        rigidbodies = GetComponentsInChildren<Rigidbody>().OfType<Rigidbody>().ToList();

        foreach (Rigidbody r in rigidbodies)
            r.isKinematic = true;
    }
#if DEBUG
    /// <summary>
    /// Sets the ragdoll state for inspector debugging
    /// </summary>
    private void Update()
    {
        if (ragdollOn != RagdollOn)
            RagdollOn = ragdollOn;
    }
#endif
}
