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
    private Vector3 m_move;
    /// <summary>
    /// How long it takes to swap lanes
    /// </summary>
    public float m_laneSwapTime = 0.5f;
    /// <summary>
    /// The timer for how long it takes to swap lanes
    /// </summary>
    private float m_laneSwapTimer = 0;
    /// <summary>
    /// The time in which before landing, if the shift key is pressed, a roll occurs
    /// </summary>
    public float m_rollReactionTime = 0.3f;
    /// <summary>
    /// The speed at which, if the player lands with a vertical velocity greater than this value without rolling, will die
    /// </summary>
    public float m_fallKillSpeed = 1;
    /// <summary>
    /// The timer to store if you have successfully rolled
    /// </summary>
    private float m_rollTimer = 0;
    /// <summary>
    /// How far away from a vaultable object you can begin the vault. This value extends from the edge of the players collider.
    /// </summary>
    public float m_vaultRange = 0.5f;
    /// <summary>
    /// Is the player attempting to roll
    /// </summary>
    bool m_doRoll = false;
    /// <summary>
    /// Is the player doing a vault
    /// </summary>
    bool m_inVault = false;
    /// <summary>
    /// Is the player is the middle of an animation we don't want to interrupt
    /// </summary>
    bool m_inAnimation = false;
    /// <summary>
    /// Did the player press left shift 
    /// </summary>
    bool m_lShift = false;
    /// <summary>
    /// Did the player press jump
    /// </summary>
    bool m_space = false;
    /// <summary>
    /// Did the player press A or D
    /// </summary>
    int m_swapLane = 0;

    public float baseScoreMultiplier = 1;

    [HideInInspector]
    public float score = 0;
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
    }

    private void Start()
    {
        m_move = Vector3.forward * m_runSpeed;
        m_a.SetFloat("Forwards", 1);

        ResetPosition();
    }
    /// <summary>
    /// Moves the player forwards and checks for player input.
    /// If there is input, does the corresponding action
    /// </summary>
    private void FixedUpdate()
    {
        float fallingSpeed = m_move.y;
        //Apply gravity
        if (!m_pc.OnGround && !m_inVault)
            m_move += Physics.gravity * Time.fixedDeltaTime;
        else
            m_move.y = 0;
        ChangeLane(ref m_move);
        ParkourMove(ref m_move);
        Move(m_move, fallingSpeed);
    }

    private void Update()
    {   //Get the inputs
        m_lShift = m_lShift || Input.GetKeyDown(KeyCode.LeftShift);
        m_space = !m_space || Input.GetKeyDown(KeyCode.Space);

        if (m_swapLane == 0 && Input.GetKeyDown(KeyCode.A))
            m_swapLane = -1;
        if (m_swapLane == 0 && Input.GetKeyDown(KeyCode.D))
            m_swapLane = 1;
    }
    /// <summary>
    /// Moves the player forward
    /// </summary>
    private void Move(Vector3 moveVec, float fallingSpeed = 0)
    {   //If we are ragdolled. don't move
        if (m_r.RagdollOn == true)
            return;
        //Cast the players movement
        bool doRagdoll = false;

        //If it hits something, make sure its not a valid surface. Otherwise ragdoll
        if (ColliderInfo.CastWithOffset(m_pc.colInfo, moveVec * Time.fixedDeltaTime, out RaycastHit hit)
            && !m_pc.collidersToIgnore.Contains(hit.collider) 
            //If its a ramp, we don't need to check the tolerance as the direction won't be up.
            //If it IS a ramp, we need to check that we aren't colliding with its left or right sides
            && (!hit.transform.CompareTag("Ramp") || (InToleraceNorm(hit.normal, Vector3.right, 0.01f) || InToleraceNorm(hit.normal, -Vector3.right, 0.01f))) 
            && !InToleraceNorm(hit.normal, Vector3.up, 0.01f)
            //If the player does not roll
            || m_pc.OnGround && Mathf.Abs(fallingSpeed) > Mathf.Abs(m_fallKillSpeed) && m_rollTimer > m_rollReactionTime)
        {
            doRagdoll = true;
        }

        //If the player died, ragdoll them and don't move
        if (doRagdoll)
            m_r.RagdollOn = true;
        //Otherwise move and increment the score
        else
        {
            m_pc.MoveTo(moveVec * Time.fixedDeltaTime);
            score += baseScoreMultiplier * Time.fixedDeltaTime;
        }
    }
    /// <summary>
    /// Moves the player into the correct lane
    /// </summary>
    private void ChangeLane(ref Vector3 moveVec)
    {
        //Check if the player can change lane this frame
        //If we are in a slide or vault, the player cannot change lanes
        if (!m_inAnimation && m_laneSwapTimer > m_laneSwapTime)
        {
            //Check if the player wants to change lanes this frame and that the lane change is a valid lane change
            if (m_lane != 0 && m_swapLane < 0)
            {
                m_lane--;
                m_laneSwapTimer = 0;
            }
            else if (m_lane != m_lg.m_numberOfLanes - 1 && m_swapLane > 0)
            {
                m_lane++;
                m_laneSwapTimer = 0;
            }
        }
        //Increment the timer
        m_laneSwapTimer += Time.fixedDeltaTime;
        //We don't need to clamp the value since we ensured it was valid when initially changing it.
        //Lerp the player over to the new lane
        //Calculate the total move vector we need to move along
        moveVec.x = (m_lane * m_lg.m_laneWidth) - transform.position.x + m_lg.m_generateOffset.x;
        //Scale it by the time until we reach the lane swap completion
        moveVec.x *= Mathf.Clamp(m_laneSwapTimer, 0, m_laneSwapTime) / m_laneSwapTime;
        //Divide by time to ensure this is applied as pure velocity
        moveVec.x /= Time.fixedDeltaTime;

        m_swapLane = 0;
    }
    /// <summary>
    /// Checks and performs a parkour move if possible
    /// </summary>
    private void ParkourMove(ref Vector3 moveVec)
    {   //Jump or Vault
        if (m_pc.OnGround && !m_inAnimation && m_space)
        {
            //Perform a raycast forwards to see if we detect a vaultable object
            if (Physics.Raycast(m_pc.colInfo.GetLowerPoint(), Vector3.forward, out RaycastHit hit, m_pc.colInfo.TrueRadius + m_vaultRange)
                && hit.transform.CompareTag("Vaultable"))
            {
                m_a.SetTrigger("Vault");
                m_inAnimation = true;
                m_cc.followHead = true;
                m_pc.colInfo.LowerHeight = 0;
                m_inVault = true;
            }
            else
                //Otherwise, do a jump
                moveVec.y = m_jumpVelocity;
        }

        Debug.DrawLine(m_pc.colInfo.GetLowerPoint(), m_pc.colInfo.GetLowerPoint() + Vector3.forward * (m_pc.colInfo.TrueRadius + m_vaultRange), Color.blue);
        //Slide or Roll
        if (!m_inAnimation && m_lShift)
        {
            //Slide
            if (m_pc.OnGround)
            {
                //Adjust the players collider
                m_cc.followHead = true;
                m_cc.m_ignoreYAxisOnHeadFollow = true;
                m_a.SetTrigger("Crouch");
                m_inAnimation = true;
            }
            else
            {
                m_rollTimer = 0;
                m_doRoll = true;
            }
        }

        if (m_doRoll && m_pc.OnGround && m_rollTimer <= m_rollReactionTime)
        {
            m_doRoll = false;
            m_cc.followHead = true;
            m_a.SetTrigger("Roll");
            m_inAnimation = true;
        }

        m_rollTimer += Time.deltaTime;
        m_space = false;
        m_lShift = false;
    }

    public void GetUp()
    {
        m_pc.colInfo.UpperHeight = 1;
        m_inAnimation = false;
    }

    public void EnterSlide()
    {
        m_pc.colInfo.UpperHeight = 0;
    }

    public void ReleaseCamera()
    {
        m_cc.followHead = false;
        m_cc.m_ignoreYAxisOnHeadFollow = false;
    }

    public void ExitVault()
    {
        m_pc.colInfo.LowerHeight = 1;
        m_inVault = false;
        m_inAnimation = false;
    }

    public void ResetPosition()
    {
        score = 0;
        //Set the starting lane for the player
        m_lane = m_lg.m_numberOfLanes / 2;

        //Set the position of the player to spawn ever so slightly above the ground
        Vector3 pos = transform.position;
        pos.y = m_pc.colInfo.LowerHeight + m_pc.colInfo.CollisionOffset * 2 + m_lg.m_tileHeight * 1.5f + m_lg.m_generateOffset.y;
        pos.z = m_lg.m_generateOffset.z;
        pos.x = m_lg.m_laneWidth * m_lane + m_lg.m_generateOffset.x;
        transform.position = pos;
    }

    private bool InToleraceNorm(Vector3 target, Vector3 expected, float tolerance)
    {
        float dot = Vector3.Dot(target.normalized, expected.normalized);
        if (dot > 1 - tolerance)
            return true;
        return false;
    }
}
