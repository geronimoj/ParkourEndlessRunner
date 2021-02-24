using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Moves and controls the character. The true movement is done by the Character controller
/// </summary>
public class CharacterMover : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 10;

    protected CharacterController cc = null;
    protected Vector2 moveInput = new Vector2();
    protected Vector3 velocity = new Vector3();
    protected Vector3 hitDir = new Vector3();

    protected bool jumpInput = false;
    protected Animator animator = null;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        jumpInput = jumpInput == false ? Input.GetButton("Jump") : true;
    }

    void FixedUpdate()
    {
        bool isGrounded = cc.isGrounded;
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

        cc.Move(velocity * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitDir = hit.point - transform.position;
    }
}
