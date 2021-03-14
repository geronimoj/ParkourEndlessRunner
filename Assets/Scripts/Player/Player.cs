using UnityEngine;
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
    /// The current lane of the player, for position calculations
    /// </summary>
    private uint _lane = 1;
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
    /// Gets references to components
    /// </summary>
    private void Awake()
    {
        _a = transform.GetChild(0).GetComponent<Animator>();
        _r = _a.GetComponent<Ragdoll>();
        _cc = transform.GetChild(1).GetComponent<CameraController>();
        _pc = GetComponent<PlayerController>();
        _lg = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelGenerator>();
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
            _move += Physics.gravity * Time.fixedDeltaTime;
        else
            _move.y = 0;
        //Continue performing lane changes
        ChangeLane();
        //Perform any parkour moves necessary
        ParkourMove();
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
            //If the player does not roll
            || _pc.OnGround && Mathf.Abs(fallingSpeed) > Mathf.Abs(_fallKillVelocity) && t_rollTimer > _rollReactionTime)
        {
            doRagdoll = true;
        }

        //If the player died, ragdoll them and don't move
        if (doRagdoll)
            _r.RagdollOn = true;
        //Otherwise move and increment the score
        else
        {
            _pc.MoveTo(_move * Time.fixedDeltaTime);
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
            //Check if the player wants to change lanes this frame and that the lane change is a valid lane change
            if (_lane != 0 && _swapLane < 0)
            {
                _lane--;
                t_laneSwapTimer = 0;
            }
            else if (_lane != _lg.NumberOfLanes - 1 && _swapLane > 0)
            {
                _lane++;
                t_laneSwapTimer = 0;
            }
        }
        //Increment the timer
        t_laneSwapTimer += Time.fixedDeltaTime;
        //We don't need to clamp the value since we ensured it was valid when initially changing it.
        //Lerp the player over to the new lane
        //Calculate the total move vector we need to move along
        _move.x = (_lane * _lg.LaneWidth) - transform.position.x + _lg.GenerateOffset.x;
        //Scale it by the time until we reach the lane swap completion
        _move.x *= Mathf.Clamp(t_laneSwapTimer, 0, _laneSwapTime) / _laneSwapTime;
        //Divide by time to ensure this is applied as pure velocity
        _move.x /= Time.fixedDeltaTime;
        //Set the player to not be swapping lanes 
        _swapLane = 0;
    }
    /// <summary>
    /// Checks and performs a parkour move if possible
    /// </summary>
    private void ParkourMove()
    {   //Jump or Vault
        if (_pc.OnGround && !m_inAnimation && _jump)
        {
            //Perform a raycast forwards to see if we detect a vaultable object
            if (Physics.Raycast(_pc.colInfo.GetLowerPoint(), Vector3.forward, out RaycastHit hit, _pc.colInfo.TrueRadius + _vaultRange)
                //If the hit object is vaultable, do a vault
                && hit.transform.CompareTag("Vaultable"))
            {   //Ignore the collider of the vaultable object as otherwise we need to freeze the players Y and change their collider
                //Which was the previous method. This in turn caused the player to have clipping issues
                _pc.collidersToIgnore.Add(hit.collider);
                _a.SetTrigger("Vault");
            }
            else
                //Otherwise, do a jump
                _move.y = _jumpVelocity;
        }

        //Slide or Roll
        if (!m_inAnimation && _slide)
        {
            //Slide but only if we aren't planning on rolling
            if (_pc.OnGround && !_doRoll)
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
        if (_doRoll && _pc.OnGround && t_rollTimer <= _rollReactionTime)
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
    public void EnterSlide()
    {
        _pc.colInfo.UpperHeight = 0;
    }
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
    /// <summary>
    /// Resets the players position, score, ragdoll state, animation and input variables
    /// </summary>
    public void ResetPlayer()
    {   //Stop us from ragdolling
        _r.RagdollOn = false;
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
        _lane = _lg.NumberOfLanes / 2;

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
    }
    /// <summary>
    /// Swaps the model to the model in Model To Swap To
    /// </summary>
    [ContextMenu("Swap Model")]
    public void SwapModel()
    {
        //Load the players model
        SkinnedMeshBoneRebinder.SwapModel(defaultModelInfo, modelToSwapTo);
        modelToSwapTo.Clear();
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
}
