﻿using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Controls everything player related
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    #region Input KeyCodes
    /// <summary>
    /// The Input Key used for sliding and rolling
    /// </summary>
    private static KeyCode _slideKey = KeyCode.LeftShift;
    /// <summary>
    /// The Input Key used for sliding and rolling.
    /// Cannot be Escape or Return
    /// </summary>
    public static KeyCode SlideKey
    {
        get => _slideKey;
        set
        {
            if (!InvalidKeyCode(value))
                _slideKey = value;
        }
    }
    /// <summary>
    /// The Input Key used for jumping and vaulting
    /// </summary>
    private static KeyCode _jumpKey = KeyCode.Space;
    /// <summary>
    /// The Input Key used for jumping and vaulting
    /// Cannot be Escape or Return
    /// </summary>
    public static KeyCode JumpKey
    {
        get => _jumpKey;
        set
        {
            if (!InvalidKeyCode(value))
                _jumpKey = value;
        }
    }
    /// <summary>
    /// The Input Key used for doding left
    /// </summary>
    private static KeyCode _dodgeLeftKey = KeyCode.A;
    /// <summary>
    /// The Input Key used for dodging left
    /// Cannot be Escape or Return
    /// </summary>
    public static KeyCode DodgeLeftKey
    {
        get => _dodgeLeftKey; 
        set
        {
            if (!InvalidKeyCode(value))
                _dodgeLeftKey = value;
        }
    }
    /// <summary>
    /// The Input Key used for doding right
    /// </summary>
    private static KeyCode _dodgeRightKey = KeyCode.D;
    /// <summary>
    /// The Input Key used for doding right
    /// Cannot be Escape or Return
    /// </summary>
    public static KeyCode DodgeRightKey
    {
        get => _dodgeRightKey;
        set
        {
            if (!InvalidKeyCode(value))
                _dodgeRightKey = value;
        }
    }
    /// <summary>
    /// Determines if a given key can be used as an input for an action for the player.
    /// Invalid Keys: Escape, Return
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>Returns true if key is Escape or Return</returns>
    private static bool InvalidKeyCode(KeyCode key)
    {
        switch(key)
        {
            case KeyCode.Escape:
            case KeyCode.Return:
            case KeyCode.Mouse0:
            case KeyCode.Mouse1:
            case KeyCode.Mouse2:
            case KeyCode.Mouse3:
            case KeyCode.Mouse4:
            case KeyCode.Mouse5:
            case KeyCode.Mouse6:
                return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// Store a static reference to this game to make it easier to read and write to the player
    /// </summary>
    public static Player player = null;

    public UnityEvent OnLaneChange;
    /// <summary>
    /// The speed at which the player runs in global units per second
    /// </summary>
    [Tooltip("Determines the speed at which the player runs in global units per second")]
    [SerializeField]
    private float _runSpeed = 0;
    /// <summary>
    /// The speed at which the player runs in global units per second
    /// </summary>
    public float RunSpeed => _runSpeed;
    /// <summary>
    /// The vertical velocity the player gains upon jumping in global units per second
    /// </summary>
    [Tooltip("The vertical velocity the player gains upon jumping in global units per second")]
    [SerializeField]
    private float _jumpVelocity = 10;
    /// <summary>
    /// The vertical velocity the player gains upon jumping in global units per second
    /// </summary>
    public float JumpVelocity => _jumpVelocity;
    /// <summary>
    /// The height, from the lowest point of the player, that they can step over without dying
    /// </summary>
    [Tooltip("The height, from the lowest point of the player, that they can step over without dying")]
    [SerializeField]
    private float _stepHeight = 0.2f;
    /// <summary>
    /// The current lane of the player, for position calculations
    /// </summary>
    private uint _lane = 1;
    /// <summary>
    /// The previous lane the player was in
    /// </summary>
    private uint _prevLane = 1;
    /// <summary>
    /// The current lane the player is in. Used for position calculations
    /// </summary>
    public uint CurrentLane => _lane;

    #region Behvaiour References
    /// <summary>
    /// Reference to a player controller used for moving this player
    /// </summary>
    private PlayerController _pc = null;
    /// <summary>
    /// Reference to this players animator
    /// </summary>
    private Animator _a = null;
    /// <summary>
    /// Reference to this players ragdoller
    /// </summary>
    private Ragdoll _r = null;
    /// <summary>
    /// Reference to this player camera controller
    /// </summary>
    private CameraController _cc = null;
    /// <summary>
    /// A reference to the level generator to get and get only, a few variables
    /// </summary>
    private LevelGenerator _lg = null;
    /// <summary>
    /// A reference to the particle systems transform
    /// </summary>
    private Transform _particleTransform = null;
    #endregion
    /// <summary>
    /// Stores the players movement vector for smooth movement
    /// </summary>
    private Vector3 _move;
    /// <summary>
    /// The duration of a laneswap. Also determines the speed at which the lane swap takes place
    /// </summary>
    [Tooltip("The duration of a laneswap. Also determines the speed at which the lane swap takes place")]
    [SerializeField]
    private float _laneSwapTime = 0.5f;
    /// <summary>
    /// The duration of a laneswap. Also determines the speed at which the lane swap takes place
    /// </summary>
    public float LaneSwapTime => _laneSwapTime;
    /// <summary>
    /// The timer used for tracking how far through a lane swap the player is
    /// </summary>
    private float t_laneSwapTimer = 0;
    /// <summary>
    /// The time period before landing the shift key must be pressed for a roll to occur
    /// </summary>
    [Tooltip("The time period before landing the Slide key must be pressed for a roll to occur")]
    [SerializeField]
    private float _rollReactionTime = 0.3f;
    /// <summary>
    /// The time period before landing the shift key must be pressed for a roll to occur
    /// </summary>
    public float RollReactionTime => _rollReactionTime;
    /// <summary>
    /// The timer to store if the player will roll upon landing
    /// </summary>
    private float t_rollTimer = 0;
    /// <summary>
    /// The duration in which the player will float and be considered on the ground after stepping off a ledge
    /// </summary>
    [Tooltip("The duration in which the player will float and be considered on the ground after stepping off a ledge")]
    [SerializeField]
    private float _coyoteTime = 0;
    /// <summary>
    /// The timer for coyoteTime
    /// </summary>
    private float t_coyoteTimer = 0;
    /// <summary>
    /// The minimum vertical speed of the player required to kill the player from falling given the player does not roll
    /// </summary>
    [Tooltip("The minimum vertical speed of the player required to kill the player from falling given the player does not roll")]
    [SerializeField]
    private float _fallKillVelocity = 1;
    /// <summary>
    /// The minimum vertical speed of the player required to kill the player from falling given the player does not roll
    /// </summary>
    public float FallKillVelocity => _fallKillVelocity;
    /// <summary>
    /// The maximum distance at which a vault can occur from a vaultable object. The distance accounts for the players collider radius
    /// </summary>
    [Tooltip("The maximum distance at which a vault can occur from a vaultable object. The distance accounts for the players collider radius")]
    [SerializeField]
    private float _vaultRange = 0.5f;
    /// <summary>
    /// Is true if the player is within the roll window and wants to rolls
    /// </summary>
    private bool _doRoll = false;
    /// <summary>
    /// Stores if the player is currently in a roll, value or slide to avoid animations interrupting each other
    /// </summary>
    private bool m_inAnimation = false;

    #region Input Storage
    /// <summary>
    /// Stores if the player pressed the slide key in an update loop between Fixed Updates
    /// </summary>
    private bool _slide = false;
    /// <summary>
    /// Stores if the player pressed the jump key in an update loop between Fixed Updates
    /// </summary>
    private bool _jump = false;
    /// <summary>
    /// Stores if the lane swap keys were pressed between Fixed Updates
    /// </summary>
    private int _swapLane = 0;
    #endregion
    /// <summary>
    /// The score multiplier for the points gained due to time
    /// </summary>
    [Tooltip("The base multiplier for the points gained due to time")]
    [SerializeField]
    private float _baseScoreMultiplier = 1;
    /// <summary>
    /// The score multiplier for the points gained due to time
    /// </summary>
    public float BaseScoreMultiplier => _baseScoreMultiplier;
    /// <summary>
    /// The players current score
    /// </summary>
    private float _score = 0;
    /// <summary>
    /// The players current score
    /// </summary>
    public float Score
    {
        get => _score;
        set => _score = value; 
    }
    /// <summary>
    /// Stores if the player is jumping
    /// </summary>
    private bool jumping = false;
    /// <summary>
    /// The gravity the player accumulates over coyoteTime
    /// </summary>
    private float trueGravity = 0;
    /// <summary>
    /// The time the player began running
    /// </summary>
    private float startTime = 0;
    /// <summary>
    /// Returns true if the player is dead
    /// </summary>
    public bool IsDead => _r.RagdollOn;
    /// <summary>
    /// Stores information about the base model of which all generated avatars are built off of
    /// </summary>
    [Tooltip("The inital model to load with the avatar the skeleton originally used")]
    [SerializeField]
    private ModelInfo defaultModelInfo = new ModelInfo();
    /// <summary>
    /// The model that should be swapped to. Root bone is not necessary
    /// </summary>
    [Tooltip("The model that should be swapped to when calling SwapModel")]
    public static ModelInfo modelToSwapTo = new ModelInfo();
    /// <summary>
    /// The index of the players current model
    /// </summary>
    public static int s_modelIndex = 0;
    /// <summary>
    /// Gets references to components
    /// </summary>
    private void Awake()
    {
        _a = transform.GetChild(0).GetComponent<Animator>();
        _r = _a.GetComponent<Ragdoll>();
        _cc = transform.GetChild(1).GetComponent<CameraController>();
        _pc = GetComponent<PlayerController>();
        _lg = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelGenerator>();
        _particleTransform = GetComponentInChildren<ParticleSystem>().transform;
    }
    /// <summary>
    /// Sets the static player reference to be this player. Sets the player to move forward and resets their position and variables
    /// </summary>
    private void Start()
    {
        player = this;
        //Clear the models so we don't swap to the same model again
        SwapModel();
        //Reset the players position
        ResetPlayer();
    }
    /// <summary>
    /// Moves the player forwards and checks for player input.
    /// If there is input, does the corresponding action
    /// </summary>
    private void FixedUpdate()
    {   //Store the vertical velocity of the player. This is needed for fall damage deaths in Move
        float fallingSpeed = _move.y;
        //Apply gravity
        if (!_pc.OnGround)
        {   //If we started falling this frame, begin counting down the coyote timer
            if (fallingSpeed == 0)
            {
                t_coyoteTimer += Time.fixedDeltaTime;
                //Store the accumulated gravity that we skipped out on
                //This is done to remove floatyness that coyote time brings
                //trueGravity += (Physics.gravity * Time.fixedDeltaTime).magnitude;
            }
            //If the coyoteTimer has finished or if we are not on the ground due to a jump, apply gravity
            if (jumping || t_coyoteTimer > _coyoteTime)
            {   //Apply the gravity that would have been accumulated over coyote time
                _move += trueGravity * Physics.gravity.normalized;
                //Set the gravity accumulated to 0 so that subsequent applications apply nothing
                trueGravity = 0;
                _move += Physics.gravity * Time.fixedDeltaTime;
            }
        }
        else
        {   //Keep coyote timer at zero
            t_coyoteTimer = 0;
            //And make sure we are not moving down
            _move.y = 0;
            jumping = false;
        }
        //Continue performing lane changes
        ChangeLane();
        //Perform any parkour moves necessary
        ParkourMove(!(jumping || t_coyoteTimer > _coyoteTime));
        //Move the player and check if they have died
        Move(fallingSpeed);
    }

    private void Update()
    {   //Get the inputs
        _slide = _slide || Input.GetKeyDown(SlideKey);
        _jump = _jump || Input.GetKeyDown(JumpKey);
        //Get the lane swap input
        if (_swapLane == 0 && Input.GetKeyDown(DodgeLeftKey))
            _swapLane = -1;
        else if (_swapLane == 0 && Input.GetKeyDown(DodgeRightKey))
            _swapLane = 1;
    }
    /// <summary>
    /// Moves the player forward
    /// </summary>
    /// <param name="fallingSpeed">The speed the player was falling at the very start of this cycle. Before gravity</param>
    private void Move(float fallingSpeed = 0)
    {   //If we are ragdolled. don't move
        if (_r.RagdollOn == true)
            return;
        //Store if we are ragdolled, becayse otherwise we don't want to move
        bool doRagdoll = false;

        //Cast the players movement
        //If it hits something, make sure its not a valid surface. Otherwise ragdoll
        if (ColliderInfo.CastWithOffset(_pc.colInfo, _move * Time.fixedDeltaTime, out RaycastHit hit)
            //Are we supposed to ignore the collider. This is to avoid colliding with ourselves or an obstacle we are currently vaulting over
            && !_pc.collidersToIgnore.Contains(hit.collider) 
            //If its a ramp, we don't need to check the tolerance as the direction won't be up.
            //If it IS a ramp, we need to check that we aren't colliding with its left or right sides
            && (!hit.transform.CompareTag("Ramp") || (InToleraceNorm(hit.normal, Vector3.right, 0.01f) || InToleraceNorm(hit.normal, -Vector3.right, 0.01f))) 
            //Is the normal not pointing upwards, if its not, its not ground
            && !InToleraceNorm(hit.normal, Vector3.up, 0.01f)
            //Check if we can step up this height, if we can't then its a collision we have to deal with
            && hit.point.y - _pc.colInfo.GetLowestPoint().y > _stepHeight
            //If the player does not roll
            || _pc.OnGround && Mathf.Abs(fallingSpeed) > Mathf.Abs(_fallKillVelocity) && t_rollTimer > _rollReactionTime)
        {   //If the collision was on a left or right wall, rather than killing the player, move them back into their original lane
            //Reguardless as to whether the player swaps lanes or dies, move's x component should probably be 0
            _move.x = 0;
            //Repeat for lane swap timer, same reasons, you either die or swap lanes 
            t_laneSwapTimer = 0;
            if (InToleraceNorm(hit.normal, Vector3.right, 0.01f))
                _lane++;
            else if (InToleraceNorm(hit.normal, -Vector3.right, 0.01f))
                _lane--;
            //We must be dead otherwise
            else
            {
                doRagdoll = true;
                //Dissable the speed lines
                _particleTransform.gameObject.SetActive(false);
                //Store the time the run lasted for
                startTime = Time.time - startTime;
                //Store the highScore
                Highscore.AddScore(new Highscore((uint)s_modelIndex, (int)Score, startTime));
            }
            OnLaneChange.Invoke();
        }

        //If the player died, ragdoll them and don't move
        if (doRagdoll)
            _r.RagdollOn = true;
        //Otherwise move and increment the score
        else
        {
            _pc.MoveTo(_move * Time.fixedDeltaTime);
            //We want to cut out the x component of the movement vector for the particles so it doesn't flick to
            //the left or right when changing lanes, just up and down.
            Vector3 moveVec = _move;
            moveVec.x = 0;
            //Rotate the particle system to fire particles in the direction of movement
            _particleTransform.LookAt(_particleTransform.position - moveVec.normalized, Vector3.up);
            _score += _baseScoreMultiplier * Time.fixedDeltaTime;
        }
    }
    /// <summary>
    /// Moves the player into the correct lane
    /// </summary>
    private void ChangeLane()
    {
        //Check if the player can change lane this frame
        //If we are in a slide or vault, the player cannot change lanes
        if (!m_inAnimation && t_laneSwapTimer > _laneSwapTime)
        {
            _prevLane = _lane;
            //Check if the player wants to change lanes this frame and that the lane change is a valid lane change
            if (_lane != 0 && _swapLane < 0)
            {
                _lane--;
                t_laneSwapTimer = 0;
                OnLaneChange.Invoke();
            }
            else if (_lane != _lg.NumberOfLanes - 1 && _swapLane > 0)
            {
                _lane++;
                t_laneSwapTimer = 0;
                OnLaneChange.Invoke();
            }
        }
        //Increment the timer
        t_laneSwapTimer += Time.fixedDeltaTime;
        //We don't need to clamp the value since we ensured it was valid when initially changing it.
        //Lerp the player over to the new lane. We calculate the change in lane, then convert that lane, which may be a decimal into re-world cords
        //Before finally subtracting the players x position to get a vector to that point
        _move.x = Mathf.Lerp(_prevLane, _lane, t_laneSwapTimer / _laneSwapTime) * _lg.LaneWidth + _lg.GenerateOffset.x - transform.position.x;
        //Divide by time to ensure this is applied as pure velocity. When we call move on the player, we multiply it by time so we need to do this otherwise the x component 
        //Gets affected by time twice
        _move.x /= Time.fixedDeltaTime;
        //Set the player to not be swapping lanes 
        _swapLane = 0;
    }
    /// <summary>
    /// Checks and performs a parkour move if possible
    /// </summary>
    /// <param name="onGround">Wether the player is on the ground/in coyote time</param>
    private void ParkourMove(bool onGround)
    {   //Jump or Vault
        if (onGround && !m_inAnimation && _jump)
        {
            //Perform a raycast forwards to see if we detect a vaultable object                                                     //Vault obstacles exist on the default layer
            if (Physics.Raycast(_pc.colInfo.GetLowerPoint(), Vector3.forward, out RaycastHit hit, _pc.colInfo.TrueRadius + _vaultRange, LayerMask.GetMask("Default"))
                //If the hit object is vaultable, do a vault
                && hit.transform.CompareTag("Vaultable"))
            {   //Ignore the collider of the vaultable object as otherwise we need to freeze the players Y and change their collider
                //Which was the previous method. This in turn caused the player to have clipping issues
                _pc.collidersToIgnore.Add(hit.collider);
                _a.SetTrigger("Vault");
            }
            else
            {
                //Otherwise, do a jump
                _move.y = _jumpVelocity;
                jumping = true;
            }
        }

        //Slide or Roll
        if (!m_inAnimation && _slide)
        {
            //Slide but only if we aren't planning on rolling
            if (onGround && !_doRoll)
            {
                //Set the camera to ignore the Y rotation of the players head for the sake of mkaing things look good
                _cc.IgnoreYAxisOnHeadFollow = true;
                _a.SetTrigger("Crouch");
            }
            else
            {   //Otherwise do a roll
                t_rollTimer = 0;
                _doRoll = true;
            }
        }
        //If we have met the roll conditions, do the roll
        if (_doRoll && onGround && t_rollTimer <= _rollReactionTime)
        {
            _doRoll = false;
            _a.SetTrigger("Roll");
        }
        //If we run out of time for a roll, set us to not be doing a roll any more
        else if (t_rollTimer > _rollReactionTime)
            _doRoll = false;
        //Increment the roll timer and reset the input keys
        t_rollTimer += Time.deltaTime;
        _jump = false;
        _slide = false;
    }
    /// <summary>
    /// Resets the players collider and sets them not to be in an animation
    /// </summary>
    public void GetUp()
    {
        _pc.colInfo.UpperHeight = 1;
        m_inAnimation = false;
    }
    /// <summary>
    /// Lowers the players collider
    /// </summary>
    public void EnterSlide() => _pc.colInfo.UpperHeight = 0;
    /// <summary>
    /// Returns the camera to normal for regular running
    /// </summary>
    public void ReleaseCamera()
    {
        _cc.FollowHead = false;
        _cc.IgnoreYAxisOnHeadFollow = false;
    }
    /// <summary>
    /// Sets us to not be in an animation and remove the collider of the vaultable object from the colliders to ignore
    /// </summary>
    public void ExitVault()
    {
        m_inAnimation = false;
        _pc.collidersToIgnore.RemoveAt(_pc.collidersToIgnore.Count - 1);
    }
    /// <summary>
    /// Set the camera to follow the head and state that we are in an animation
    /// </summary>
    public void EnterAnim()
    {
        _cc.FollowHead = true;
        m_inAnimation = true;
    }

    public void EnterDoor() => m_inAnimation = true;

    public void ExitDoor() => m_inAnimation = false;
    /// <summary>
    /// Resets the players position, score, ragdoll state, animation and input variables
    /// </summary>
    public void ResetPlayer()
    {   //Stop us from ragdolling
        _r.RagdollOn = false;
        //Make sure the particles are enabled
        _particleTransform.gameObject.SetActive(true);
        //Reset our animation
        _a.Play("Base Layer.Blend Tree", 0);
        //Reset the camera rotation
        _cc.ResetRotation();
        //Reset the move vector
        _move = Vector3.forward * _runSpeed;
        //Just a backup check to ensure the player is playing the running animation
        _a.SetFloat("Forwards", 1);
        //Reset the score
        _score = 0;
        //Set the starting lane for the player
        _prevLane = _lane = _lg.NumberOfLanes / 2; ;
        //Reset the players layer masks for moving as they will need to be redone if they die indoors
        _pc.colInfo.ground = LayerMask.GetMask("Default", "Indoors", "OutDoors");
        //Set the position of the player to spawn ever so slightly above the ground
        Vector3 pos;
        pos.y = _pc.colInfo.LowerHeight + _pc.colInfo.CollisionOffset * 2 + _lg.LayerHeight * 1.5f + _lg.GenerateOffset.y;
        pos.z = _lg.GenerateOffset.z;
        pos.x = _lg.LaneWidth * _lane + _lg.GenerateOffset.x;
        transform.position = pos;
        //Reset the input values and other variables for keeping track on animations
        _doRoll = false;
        _jump = false;
        _slide = false;
        m_inAnimation = false;
        _cc.FollowHead = false;
        _swapLane = 0;
        //Store the time the player started running
        startTime = Time.time;
    }
    /// <summary>
    /// Swaps the model to the model in Model To Swap To
    /// </summary>
    [ContextMenu("Swap Model")]
    public void SwapModel()
    {
        //Swap the player models
        SkinnedMeshBoneRebinder.SwapModel(defaultModelInfo, modelToSwapTo, null);
    }
    /// <summary>
    /// Determines if a given vector3 is pointing in the same direction as another
    /// </summary>
    /// <param name="target">The Vector to compare</param>
    /// <param name="expected">The vector to compare to</param>
    /// <param name="tolerance">The difference in dot product from 1 to return true</param>
    /// <returns>Returns true if the Dot of Target normalized onto expectred normalized is > the 1 - tolerance </returns>
    private bool InToleraceNorm(Vector3 target, Vector3 expected, float tolerance)
    {
        float dot = Vector3.Dot(target.normalized, expected.normalized);
        if (dot > 1 - tolerance)
            return true;
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_pc != null)
        {
            Vector3 p = _pc.colInfo.GetLowestPoint();
            Gizmos.DrawLine(p, p + Vector3.forward * (_pc.colInfo.TrueRadius + _vaultRange));
        }
    }
#endif
}
