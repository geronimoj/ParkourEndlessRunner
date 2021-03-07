//Do defines here. This define simply states if the colliders should be toggled.
#define ToggleColliders

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls if an animated object should be ragdolling. The animator should not be manually toggled.
/// </summary>
[RequireComponent(typeof(Animator))]
public class Ragdoll : MonoBehaviour
{
    /// <summary>
    /// Store a reference to the animator
    /// </summary>
    private Animator _animator = null;
    /// <summary>
    /// A list of the rigidbodies on the ragdoll. This is automatically calculated
    /// </summary>
    private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
#if ToggleColliders
    /// <summary>
    /// A list of the colliders used for ragdolling. This is automatically calculated
    /// </summary>
    private Collider[] _colliders;
#endif
#if DEBUG
    /// <summary>
    /// Stores the animators current state to avoid desync due to people changing it in the inspector
    /// </summary>
    private bool _animatorState = false;
#endif
    /// <summary>
    /// Sets or returns the state of the ragdoll
    /// </summary>
    public bool RagdollOn
    {
        get => !_animator.enabled; 
        set
        {   //Toggle the animator
            _animator.enabled = !value;
#if DEBUG
            //Store the animators state to avoid the two Monobehaviours from becoming out of sync due to incorrect medling of the monobehaviours in the inspector
            _animatorState = _animator.enabled;
#endif
            //Toggle the rigidbodies sp they become simulated by physics
            foreach (Rigidbody r in _rigidbodies)
                r.isKinematic = !value;
#if ToggleColliders
            //Toggle the colliders back on or off. This is because the colliders may interact with the world in ways not wanted.
            foreach (Collider c in _colliders)
                c.enabled = value;
#endif
        }
    }
    /// <summary>
    /// Initialises the class and finds the rigidbodies
    /// </summary>
    private void Awake()
    {   
        _animator = GetComponent<Animator>();

        _rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
#if ToggleColliders
        _colliders = GetComponentsInChildren<Collider>();
#endif
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
    /// <summary>
    /// Makes sure a developer does not toggle the animator in the inspector without toggling the ragdoll state
    /// </summary>
    private void Update()
    {   //If the states do not match up. Set this to be the inverse of the animator
        if (_animatorState != _animator.enabled)
        {
            RagdollOn = !_animator.enabled;
            //Since RagdollOn also toggles the animators enabled state, we need to toggle it back again
            //To re-synce the two mono behaviours
            _animator.enabled = !_animator.enabled;
        }
    }
#endif
}
