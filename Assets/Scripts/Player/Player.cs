using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls everything player related
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// The speed at which the player runs
    /// </summary>
    public float m_runSpeed = 0;
    /// <summary>
    /// The velocity the player gains upon jumping
    /// </summary>
    public float m_jumpVelocity = 10;
    /// <summary>
    /// The current lane of the player, for position calculations
    /// </summary>
    private uint m_lane = 1;
    /// <summary>
    /// Reference to a player controller used for moving thisn player
    /// </summary>
    private PlayerController m_pc = null;
    /// <summary>
    /// Reference to this players animator
    /// </summary>
    private Animator m_a = null;
    /// <summary>
    /// Reference to this players ragdoller
    /// </summary>
    private Ragdoll m_r = null;
    /// <summary>
    /// Reference to this player camera controller
    /// </summary>
    private CameraController m_cc = null;
    /// <summary>
    /// A reference to the level generator to get and get only, a few variables
    /// </summary>
    private LevelGenerator m_lg = null;
    /// <summary>
    /// Stores the players movement vector
    /// </summary>
    private Vector3 move;

    public float laneSwapTime = 0.5f;

    private float laneSwapTimer = 0;
    /// <summary>
    /// Gets references to components
    /// </summary>
    private void Awake()
    {
        m_a = transform.GetChild(0).GetComponent<Animator>();
        m_r = m_a.GetComponent<Ragdoll>();
        m_cc = transform.GetChild(1).GetComponent<CameraController>();
        m_pc = GetComponent<PlayerController>();
        m_lg = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelGenerator>();
        Vector3 pos = transform.position;
        pos.y = (m_lg.m_numberOfLayers + 1) * m_lg.m_tileHeight + m_lg.m_generateOffset.y;
        pos.z = m_lg.m_generateOffset.z;
        transform.position = pos;
    }

    private void Start()
    {
        move = Vector3.forward * m_runSpeed;
    }
    /// <summary>
    /// Moves the player forwards and checks for player input.
    /// If there is input, does the corresponding action
    /// </summary>
    private void Update()
    {   //Apply gravity
        if (!m_pc.OnGround)
            move += Physics.gravity * Time.deltaTime;
        else
            move.y = 0;
        ChangeLane(ref move);
        ParkourMove(ref move);
        Move(move);
    }
    /// <summary>
    /// Moves the player forward
    /// </summary>
    private void Move(Vector3 moveVec)
    {   //If we are ragdolled. don't move
        if (m_r.RagdollOn == true)
            return;
        //Cast the players movement
        bool doRagdoll = false;

        //If it hits something, make sure its not a valid surface. Otherwise ragdoll
        if (ColliderInfo.Cast(m_pc.colInfo, moveVec * Time.deltaTime, out RaycastHit hit))
            if (!m_pc.collidersToIgnore.Contains(hit.collider) && !hit.transform.CompareTag("Ramp") && hit.normal != Vector3.up)
            {
                doRagdoll = true;
                Debug.Log("Killed by: " + hit.transform.gameObject.name);
            }

        //Else move
        if (doRagdoll)
            m_r.RagdollOn = true;
        else
            m_pc.MoveTo(moveVec * Time.deltaTime);
    }
    /// <summary>
    /// Moves the player into the correct lane
    /// </summary>
    private void ChangeLane(ref Vector3 moveVec)
    {   //Check if the player can change lane this frame
        if (laneSwapTimer > laneSwapTime)
        {
            //Check if the player wants to change lanes this frame and that the lane change is a valid lane change
            if (m_lane != 0 && Input.GetKeyDown(KeyCode.A))
            {
                m_lane--;
                laneSwapTimer = 0;
            }
            else if (m_lane != m_lg.m_numberOfLanes - 1 && Input.GetKeyDown(KeyCode.D))
            {
                m_lane++;
                laneSwapTimer = 0;
            }
        }
        //Increment the timer
        laneSwapTimer += Time.deltaTime;
        //We don't need to clamp the value since we ensured it was valid when initially changing it.
        //Lerp the player over to the new lane
        //Calculate the total move vector we need to move along
        moveVec.x = (m_lane * m_lg.m_laneWidth) - transform.position.x + m_lg.m_generateOffset.x;
        //Scale it by the time until we reach the lane swap completion
        moveVec.x *= Mathf.Clamp(laneSwapTimer, 0, laneSwapTime) / laneSwapTime;
        //Divide by time to ensure this is applied as pure velocity
        moveVec.x /= Time.deltaTime;
    }
    /// <summary>
    /// Checks and performs a parkour move if possible
    /// </summary>
    private void ParkourMove(ref Vector3 moveVec)
    {   //Jump
        if (m_pc.OnGround && Input.GetKey(KeyCode.Space))
            moveVec.y = m_jumpVelocity;
    }
}
