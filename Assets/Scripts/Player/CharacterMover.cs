using UnityEngine;
/// <summary>
/// Moves and controls the character. The true movement is done by the Character controller
/// </summary>
public class CharacterMover : MonoBehaviour
{
    /// <summary>
    /// The maximum speed of the player
    /// </summary>
    [Tooltip("The maximum speed of the player")]
    public float speed = 10;
    /// <summary>
    /// The vertical velocity applied to the player upon jumping
    /// </summary>
    [Tooltip("The vertical velocity applied to the player upon jumping")]
    public float jumpForce = 10;
    /// <summary>
    /// Storages the direction of movement
    /// </summary>
    protected Vector2 moveInput = new Vector2();
    /// <summary>
    /// Stores the velocity
    /// </summary>
    protected Vector3 velocity = new Vector3();
    /// <summary>
    /// Stores the direction to the nearest collsison with the character controller
    /// </summary>
    protected Vector3 hitDir = new Vector3();
    /// <summary>
    /// Determines if we should jump
    /// </summary>
    protected bool jumpInput = false;
    /// <summary>
    /// Stores a reference to the animator
    /// </summary>
    protected Animator animator = null;
    /// <summary>
    /// A reference to the character controller.
    /// The character controller is used to me the player
    /// </summary>
    protected PlayerController cc = null;
    /// <summary>
    /// Gets references to other components
    /// </summary>
    void Awake()
    {
        cc = GetComponent<PlayerController>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }
    /// <summary>
    /// Updates moveInput and jump input
    /// </summary>
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        jumpInput = jumpInput == false ? Input.GetButton("Jump") : true;

        animator.SetBool("Crouch", Input.GetButton("Crouch"));
    }
    /// <summary>
    /// Moves the player and causes them to jump
    /// </summary>
    void FixedUpdate()
    {
        bool isGrounded = cc.OnGround;
        Vector3 forwards = Vector3.Cross(transform.right, Vector3.up);
        Vector3 delta = (moveInput.x * transform.right + moveInput.y * forwards) * speed;

        if (isGrounded || moveInput.x !=0 || moveInput.y != 0)
        {
            velocity.x = delta.x;
            velocity.z = delta.z;
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = 0;

        if (jumpInput && isGrounded)
            velocity.y = jumpForce;

        animator.SetBool("Jump", jumpInput || !isGrounded);

        animator.SetFloat("Forwards", velocity != Vector3.zero ? 1 : 0);

        jumpInput = false;

        velocity += Physics.gravity * Time.fixedDeltaTime;

        if (!isGrounded)
            hitDir = Vector3.zero;

        if (moveInput.x == 0 && moveInput.y == 0)
        {
            Vector3 hozHitDir = hitDir;
            hozHitDir.y = 0;
            float displacement = hozHitDir.magnitude;
            if (displacement > 0)
                velocity -= 0.2f * hozHitDir / displacement;
        }

        cc.MoveTo(velocity * Time.fixedDeltaTime);
    }
    /// <summary>
    /// Get hitDir
    /// </summary>
    /// <param name="hit">The hit info from the collision</param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitDir = hit.point - transform.position;
    }
}
