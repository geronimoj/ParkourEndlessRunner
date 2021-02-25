using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls everything player related
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// The speed at which the player runs
    /// </summary>
    public float m_runSpeed = 0;
    /// <summary>
    /// The current lane of the player, for position calculations
    /// </summary>
    public uint m_lane = 0;

    //Reference to character controller

    private Animator m_a = null;

    private CameraController m_cc = null;
    /// <summary>
    /// Gets references to components
    /// </summary>
    private void Awake()
    {
        m_a = transform.GetChild(0).GetComponent<Animator>();
        m_cc = transform.GetChild(1).GetComponent<CameraController>();
    }
    /// <summary>
    /// Moves the player forwards and checks for player input.
    /// If there is input, does the corresponding action
    /// </summary>
    private void Update()
    {
        
    }
}
